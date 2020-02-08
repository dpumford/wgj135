using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour
{
    public float asteroidSpawnSeconds = 10;
    public int maxNumberOfAsteroids = 4;
    public GameObject asteroidPrefab;
    public GameObject[] possiblePositions;

    float asteroidSpawnTimer;

    ShipController player;

    GameState state;

    void Start()
    {
        state = GameState.MainMenu;
        player = FindObjectOfType<ShipController>();
        
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
                Debug.Log("At menu!");
                StartGame();
                state = GameState.Playing;
                break;
            case GameState.Playing:
                Debug.Log("Playing game!");
                UpdatePlayState();
                break;
            case GameState.Dead:
                Debug.Log("Press key to continue!");
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
        player.Spawn(Vector2.zero);
    }

    void UpdatePlayState()
    {
        if (FindObjectsOfType<AsteroidController>().Length < maxNumberOfAsteroids)
        {
            asteroidSpawnTimer += Time.deltaTime;
        }

        if (asteroidSpawnTimer > asteroidSpawnSeconds)
        {
            asteroidSpawnTimer = 0;
            Instantiate(asteroidPrefab, PickAsteroidSpawnPoint(), Quaternion.identity);
        }

        if (player.CurrentHealth() == 0)
        {
            player.Die();
            state = GameState.Dead;
        }
    }

    Vector2 PickAsteroidSpawnPoint()
    {
        if (possiblePositions.Length == 0)
        {
            return Vector2.zero;
        }

        var stars = FindObjectsOfType<StarController>();
        var asteroids = FindObjectsOfType<AsteroidController>();

        var bodies = (from star in stars select star.gameObject)
            .Concat(from asteroid in asteroids select asteroid.gameObject)
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

        return bestSpawnPoint.transform.position;
    }
}
