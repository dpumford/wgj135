using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : CelestialBody
{
    public int ringCount = 2;
    public int segmentCount = 3;
    public int segmentLimit = 3;

    public float timePerSegment = 10;
    public float segmentTimer;

    void Start()
    {
        ParentStart();
        segmentTimer = 0;
        damageToPlayerOnCollision = 999;
    }

    void FixedUpdate()
    {
        ParentFixedUpdate();
        UpdateSegments();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleAsteroids(collision);
    }

    private void UpdateSegments()
    {
        segmentTimer += Time.deltaTime;

        if (segmentTimer > timePerSegment)
        {
            segmentTimer = 0;
            segmentCount--;

            if (segmentCount <= 0)
            {
                segmentCount = segmentLimit;
                ringCount--;

                if (ringCount <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void HandleAsteroids(Collider2D collision)
    {
        var asteroid = collision.GetComponent<AsteroidController>();

        if (asteroid != null)
        {
            segmentCount++;

            if (segmentCount > segmentLimit)
            {
                segmentCount = 1;
                ringCount++;
            }

            segmentTimer = 0;
            Destroy(collision.gameObject);
        }
    }
}
