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
    public GameObject[] possiblePositions;

    public GameObject starPrefab;
    public GameObject blackHolePrefab;

    public Sprite lossSprite;
    public Sprite winSprite;

    ShipController player;
    SpriteRenderer spriteRenderer;

    GameState state;
    float asteroidSpawnTimer;

    void Start()
    {
        state = GameState.MainMenu;
        player = FindObjectOfType<ShipController>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        if (player == null)
        {
            Debug.LogError("Missing a player!!");
        }
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
        int playerPosition = Random.Range(0, possiblePositions.Length);
        player.Spawn(possiblePositions[playerPosition].transform.position);

        int starPosition1 = (playerPosition + 2) % possiblePositions.Length;
        int starPosition2 = (playerPosition + 4) % possiblePositions.Length;
        int blackHolePosition = (playerPosition + 3) % possiblePositions.Length;

        Instantiate(starPrefab, possiblePositions[starPosition1].transform.position, Quaternion.identity);
        Instantiate(starPrefab, possiblePositions[starPosition2].transform.position, Quaternion.identity);

        Instantiate(blackHolePrefab, possiblePositions[blackHolePosition].transform.position, Quaternion.identity);
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

        if (needers.Length > 0 && (from needer in needers where needer.complete select needer).Count() == needers.Length)
        {
            Cleanup();
            state = GameState.Win;
        }
    }

    private void HandleLossCondition()
    {
        if (FindObjectsOfType<StarController>().Length == 0 || player.CurrentHealth() == 0)
        {
            Cleanup();
            state = GameState.Dead;
        }
    }

    private void Cleanup()
    {
        player.Die();

        foreach (var body in FindObjectsOfType<CelestialBody>())
        {
            Destroy(body.gameObject);
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
        GameObject bestSpawnPoint = null;

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
