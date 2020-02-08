using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public float asteroidSpawnSeconds = 10;
    public int maxNumberOfAsteroids = 4;
    public GameObject asteroidPrefab;
    public GameObject[] possiblePositions;

    public GameObject starPrefab;

    public GameObject blackHolePrefab;

    public Sprite LossSprite;

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
                Debug.Log("At menu!");
                StartGame();
                state = GameState.Playing;
                break;
            case GameState.Playing:
                spriteRenderer.sprite = null;
                Debug.Log("Playing game!");
                UpdatePlayState();
                break;
            case GameState.Dead:
                spriteRenderer.sprite = LossSprite;

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
        int starPosition = Random.Range(0, possiblePositions.Length);
        int blackHolePosition = (starPosition + 2) % possiblePositions.Length;
        Instantiate(starPrefab, possiblePositions[starPosition].transform.position, Quaternion.identity);
        Instantiate(blackHolePrefab, possiblePositions[blackHolePosition].transform.position, Quaternion.identity);
    }

    void UpdatePlayState()
    {
        HandleLossCondition();
        HandleAsteroidSpawning();
    }

    private void HandleLossCondition()
    {
        if (FindObjectsOfType<StarController>().Length == 0 || player.CurrentHealth() == 0)
        {
            player.Die();

            foreach (var body in FindObjectsOfType<CelestialBody>())
            {
                Destroy(body.gameObject);
            }

            state = GameState.Dead;
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
        switch (Mathf.Floor(Random.value * 4))
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
