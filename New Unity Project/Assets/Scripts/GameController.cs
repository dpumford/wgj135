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
    public GameObject asteroidPrefab;
    public Transform[] possiblePositions;

    public Transform[] playerSpawnPoints;

    public Sprite lossSprite;
    public Sprite winSprite;

    ShipController player;
    BlackHoleController[] holes;
    StarController[] stars;
    SpriteRenderer spriteRenderer;
    PlanetController[] planets;

    GameState state;
    float asteroidSpawnTimer;

    void Start()
    {
        state = GameState.MainMenu;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        player = FindObjectOfType<ShipController>();
        
        if (player == null)
        {
            Debug.LogError("Missing a player!!");
        }

        holes = FindObjectsOfType<BlackHoleController>();
        stars = FindObjectsOfType<StarController>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state) {
            case GameState.MainMenu:
                StartGame();
                state = GameState.Playing;
                break;
            case GameState.Playing:
                spriteRenderer.sprite = null;
                UpdatePlayState();
                break;
            case GameState.Dead:
                spriteRenderer.sprite = lossSprite;

                if (Input.anyKey)
                {
                    state = GameState.MainMenu;
                }
                break;
            case GameState.Win:
                spriteRenderer.sprite = winSprite;

                if (Input.anyKey)
                {
                    state = GameState.MainMenu;
                }
                break;
            default:
                break;
        }
    }

    void StartGame()
    {
        player.Spawn(playerSpawnPoints[Random.Range(0, playerSpawnPoints.Length)].position);

        foreach (var blackHole in holes)
        {
            blackHole.Spawn();
        }

        foreach (var star in stars)
        {
            star.Spawn();
        }

        planets = FindObjectsOfType<PlanetController>();
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
        if ((from planet in planets where planet.IsAlive() select planet).Count() == 0 || player.CurrentHealth() == 0)
        {
            Cleanup();
            state = GameState.Dead;
        }
    }

    private void Cleanup()
    {
        player.Die();

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
            var asteroid = Instantiate(asteroidPrefab, PickAsteroidSpawnPoint(), Quaternion.identity);
            AsteroidController aController = asteroid.GetComponent<AsteroidController>();
            aController.Init(RandomMaterial());
        }
    }
    
    Material RandomMaterial()
    {
        switch (Random.Range(0, 4))
        {
            case 0:
                return Material.Hydrogen;
            case 1:
                return Material.Helium;
            case 2:
                return Material.Lithium;
            case 3:
                return Material.Boron;
            default:
                return Material.Hydrogen;
        }
    }

    Vector2 PickAsteroidSpawnPoint()
    {
        if (possiblePositions.Length == 0)
        {
            return Vector2.zero;
        }

        var bodies = (from star in FindObjectsOfType<StarController>() select star.gameObject)
            .Concat(from asteroid in FindObjectsOfType<AsteroidController>() select asteroid.gameObject)
            .Concat(from blackHole in FindObjectsOfType<BlackHoleController>() select blackHole.gameObject)
            .ToList();

        var maxDistance = 0f;
        Transform bestSpawnPoint = null;

        foreach (var point in possiblePositions)
        {
            var totalDistance = 0f;

            foreach (var body in bodies)
            {
                totalDistance += (body.transform.position - point.transform.position).magnitude;
            }

            if (totalDistance > maxDistance)
            {
                maxDistance = totalDistance;
                bestSpawnPoint = point;
            }
        }

        return (Vector2)bestSpawnPoint.transform.position + 
            new Vector2(Random.Range(-AsteroidSpawnFuzz, AsteroidSpawnFuzz), Random.Range(-AsteroidSpawnFuzz, AsteroidSpawnFuzz));
    }
}
