using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : CelestialBody
{
    void Start()
    {
        ParentStart();
    }

    void FixedUpdate()
    {
        ParentFixedUpdate();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var asteroid = collision.gameObject.GetComponent<AsteroidController>();

        if (asteroid != null)
        {
            asteroid.Die();
        }
    }
}
