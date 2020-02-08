﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitQueue : MonoBehaviour
{
    LinkedList<CelestialBody> bodies;
    public int capacity = 10;

    public int spinFrames = 240;
    public int spinDistance = 1;
    int currentFrame = 0;

    Transform parentTransform;
    Transform shootPosition;

    CelestialBody orbiterToFire = null;

    void Start()
    {
        bodies = new LinkedList<CelestialBody>();
        parentTransform = GetComponentInParent<Transform>();
        shootPosition = GameObject.Find("ShootPoint").GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        UpdateOrbiterToFirePosition();
        UpdateOrbiterPositions();
    }

    void UpdateOrbiterToFirePosition()
    {
        if (orbiterToFire != null)
        {
            orbiterToFire.SetOrbitingPosition(shootPosition.position);
        }
    }

    void UpdateOrbiterPositions()
    {
        float degreeInterval = 360.0f / bodies.Count;
        float spinProgress = 360 * currentFrame / spinFrames;

        int slot = 0;
        Vector2 orbiterPosition = Vector2.zero;

        foreach (var orbiter in bodies)
        {
            float angle = (degreeInterval * slot + spinProgress) % 360;

            orbiterPosition.x = Mathf.Cos(Mathf.Deg2Rad * angle);
            orbiterPosition.y = Mathf.Sin(Mathf.Deg2Rad * angle);

            orbiterPosition *= spinDistance;

            orbiter.SetOrbitingPosition((Vector2)parentTransform.position + orbiterPosition);

            slot++;
        }

        currentFrame++;
        currentFrame %= spinFrames;
    }

    public void CollectOrbiters(LinkedList<CelestialBody> newOrbiters)
    {
        while (newOrbiters.Count > 0 && bodies.Count < capacity)
        {
            CelestialBody orbiter = newOrbiters.Last.Value;
            orbiter.Collect();
            bodies.AddLast(orbiter);
            newOrbiters.RemoveLast();
        }
    }

    public void PrepareFire()
    {
        if (bodies.Count > 0 && orbiterToFire == null)
        {
            var orbiter = bodies.First.Value;
            bodies.RemoveFirst();

            orbiterToFire = orbiter;
        }
    }

    public void Fire(Vector2 velocity)
    {
        if (orbiterToFire != null)
        {
            orbiterToFire.Fire(velocity);
            orbiterToFire = null;
        }
    }
}
