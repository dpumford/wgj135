using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StarOptions
{
    public int[] orbitCapacity, orbitFrames;
    public float[] orbitDistance;

    public int initialNumberOfPlanets;

    public int orbiterHealth;

    public int minimumLiveOrbiters;
    
    [Range(0, 1)]
    public float orbiterLifeChance;
}

public class StarController : CelestialBody
{
    public float destinationForce = 1000000;

    public int ringSegmentSize = 3;
    public float baseScale = 20;
    public float scalePerRing = 10;

    private NeederController needer;
    private StarStatusController statusController;

    private OrbitGroup orbits;

    void Start()
    {
        ParentStart();
        damageToPlayerOnCollision = 999;
    }

    void FixedUpdate()
    {
        ParentFixedUpdate();
    }

    void Update()
    {
        UpdateHealth();
        UpdatePosition();
        UpdateWidth();
    }

    private void UpdatePosition()
    {
        if (!needer.IsComplete())
        {
            var destination = Vector2.zero;

            foreach (var star in FindObjectsOfType<StarController>())
            {
                if (star != this)
                {
                    destination += (Vector2)star.transform.position;
                }
            }

            myBody.AddForce((destination - (Vector2)transform.position).normalized * destinationForce);
        }
    }

    private void UpdateHealth()
    {
        if (needer.IsDead())
        {
            Die();
        }
    }

    private void UpdateWidth()
    {
        // TODO: Make this based on needs filled rather than materials held -- possible to make star huge with only one material
        var scale = baseScale + (needer.GatheredCount() / ringSegmentSize) * scalePerRing;
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(scale, scale, transform.localScale.z), 0.2f);
    }

    public void Spawn(NeederOptions options, StarOptions starOptions)
    {
        needer = GetComponent<NeederController>();
        orbits = GetComponent<OrbitGroup>();
        statusController = GetComponent<StarStatusController>();

        for (int i = 0; i < starOptions.orbitDistance.Length; i++)
        {
            orbits.AddOrbit(transform, starOptions.orbitCapacity[i], starOptions.orbitFrames[i], starOptions.orbitDistance[i]);
        }

        var liveOrbitersRemaining = starOptions.minimumLiveOrbiters;

        for (int i = 0; i < starOptions.initialNumberOfPlanets; i++)
        {
            var state = PlanetState.Fallow;
            var health = 0;
            
            if (liveOrbitersRemaining > 0)
            {
                state = PlanetState.Alive;
                health = starOptions.orbiterHealth;
            }
            else if (UnityEngine.Random.Range(0, 100) < starOptions.orbiterLifeChance)
            {
                state = PlanetState.Alive;
                health = starOptions.orbiterHealth;
            }

            orbits.AddOrbiter(state, health);
        }

        needer.Reset(options);

        statusController.Spawn();
    }

    public override void Die()
    {
        orbits.Die();

        statusController.Die();

        Destroy(gameObject);
    }
}
