using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleController : CelestialBody
{
    private Vector2 initialPosition;

    void Start()
    {
        damageToPlayerOnCollision = 999;
        ParentStart();

        initialPosition = transform.position;
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
        asteroid.Die();
    }

    public void Spawn()
    {
        gameObject.SetActive(true);
        transform.position = initialPosition;
    }

    public override void Die()
    {
        gameObject.SetActive(false);
    }
}
