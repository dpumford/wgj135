using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleController : CelestialBody
{
    void Start()
    {
        damageToPlayerOnCollision = 999;
        ParentStart();
    }

    void FixedUpdate()
    {
        ParentFixedUpdate();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        AsteroidController asteroid = collision.gameObject.GetComponent<AsteroidController>();

        if (asteroid != null)
        {
            HandleAsteroids(asteroid);
        }
    }

    private void HandleAsteroids(AsteroidController asteroid)
    {
        Destroy(asteroid.gameObject);
    }
}
