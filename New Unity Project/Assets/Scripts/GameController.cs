using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public float asteroidSpawnSeconds = 10;
    public int maxNumberOfAsteroids = 4;
    public float AsteroidSpawnFuzz = .5f;
    public AsteroidController[] asteroidPrefabs;
    public AsteroidSpawnSet asteroidSpawnPoints;

    public ShipController shipPrefab;
    public PlayerSpawnSet playerSpawnPoints;

    public StarController starPrefab;
    public StarSpawnSet starSpawnPoints;

    public BlackHoleController blackHolePrefab;
    public BlackHoleSpawnSet holeSpawnPoints;

    public Sprite lossSprite;
    public Sprite winSprite;
    public string nextLevel;
    
    SpriteRenderer spriteRenderer;

    GameState state;
    float asteroidSpawnTimer;

    void Start()
    {
        state = GameState.New;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state) {
            case GameState.New:
                StartGame();
                state = GameState.Playing;
                Debug.Log("Game started!");
                break;
            case GameState.Playing:
                spriteRenderer.sprite = null;
                UpdatePlayState();
                break;
            case GameState.Dead:
                spriteRenderer.sprite = lossSprite;

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    state = GameState.New;
                }
                break;
            case GameState.Win:
                spriteRenderer.sprite = winSprite;

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    state = GameState.New;
                }
                else if (Input.GetKeyUp(KeyCode.Return))
                {
                    SceneManager.LoadScene(nextLevel);
                }
                break;
            default:
                break;
        }
    }

    void StartGame()
    {
        Debug.Log("Starting game");
        var player = Instantiate(shipPrefab);

        if (player == null)
        {
            Debug.LogError("Missing a player!!");
        }
        
        playerSpawnPoints.Setup();
        asteroidSpawnPoints.Setup();
        starSpawnPoints.Setup();
        holeSpawnPoints.Setup();

        player.Spawn(playerSpawnPoints.spawnPoints[0].transform.position);

        foreach (var point in holeSpawnPoints.spawnPoints)
        {
            var blackHole = Instantiate(blackHolePrefab, point.transform.position, Quaternion.identity).GetComponent<BlackHoleController>();
            blackHole.Spawn();
        }

        foreach (var point in starSpawnPoints.spawnPoints)
        {
            var star = Instantiate(starPrefab, point.transform.position, Quaternion.identity).GetComponent<StarController>();
            star.Spawn(point.options, point.starOptions);
        }
    }

    void UpdatePlayState()
    {
        HandleWinCondition();
        HandleLossCondition();
        HandleAsteroidSpawning();
    }

    private void HandleWinCondition()
    {
        var needers = FindObjectsOfType<NeederController>();

        if (needers.Length > 0 && (from needer in needers where needer.IsComplete() select needer).Count() == needers.Length)
        {
            Cleanup();
            state = GameState.Win;
        }
    }

    private void HandleLossCondition()
    {
        var player = FindObjectOfType<ShipController>();
        var planets = FindObjectsOfType<PlanetController>();

        if (player == null || (from planet in planets where planet.IsAlive() select planet).Count() == 0)
        {
            Cleanup();
            state = GameState.Dead;
        }
    }

    private void Cleanup()
    {
        foreach (var ship in FindObjectsOfType<ShipController>())
        {
            ship.Die();
        }

        foreach (var asteroid in FindObjectsOfType<AsteroidController>())
        {
            asteroid.Die();
        }

        foreach (var blackHole in FindObjectsOfType<BlackHoleController>())
        {
            blackHole.Die();
        }

        foreach (var star in FindObjectsOfType<StarController>())
        {
            star.Die();
        }

        foreach (var planet in FindObjectsOfType<PlanetController>())
        {
            planet.Die();
        }
    }

    private void HandleAsteroidSpawning()
    {
        if (FindObjectsOfType<AsteroidController>().Length < maxNumberOfAsteroids)
        {
            asteroidSpawnTimer += Time.deltaTime;
        }

        if (asteroidSpawnTimer > asteroidSpawnSeconds)
        {
            asteroidSpawnTimer = 0;
            var asteroid = Instantiate(asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)].gameObject, PickAsteroidSpawnPoint(), Quaternion.identity);
        }
    }

    Vector2 PickAsteroidSpawnPoint()
    {
        if (asteroidSpawnPoints.spawnPoints.Length == 0)
        {
            return Vector2.zero;
        }

        var bodies = (from star in FindObjectsOfType<StarController>() select star.gameObject)
            .Concat(from asteroid in FindObjectsOfType<AsteroidController>() select asteroid.gameObject)
            .Concat(from blackHole in FindObjectsOfType<BlackHoleController>() select blackHole.gameObject)
            .ToList();

        var maxDistance = 0f;
        Transform bestSpawnPoint = null;

        foreach (var point in asteroidSpawnPoints.spawnPoints)
        {
            var totalDistance = 0f;

            foreach (var body in bodies)
            {
                totalDistance += (body.transform.position - point.transform.position).magnitude;
            }

            if (totalDistance > maxDistance)
            {
                maxDistance = totalDistance;
                bestSpawnPoint = point.transform;
            }
        }

        return (Vector2)bestSpawnPoint.position + 
            new Vector2(Random.Range(-AsteroidSpawnFuzz, AsteroidSpawnFuzz), Random.Range(-AsteroidSpawnFuzz, AsteroidSpawnFuzz));
    }
}
