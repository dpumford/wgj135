using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public float asteroidSpawnSeconds = 10;
    public GameObject asteroidPrefab;

    float asteroidSpawnTimer;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        asteroidSpawnTimer += Time.deltaTime;

        if (asteroidSpawnTimer > asteroidSpawnSeconds)
        {
            asteroidSpawnTimer = 0;
            Instantiate(asteroidPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}
