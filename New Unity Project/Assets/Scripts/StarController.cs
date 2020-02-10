using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : CelestialBody
{
    public float destinationForce = 1000000;

    public int ringSegmentSize = 3;
    public float baseScale = 20;
    public float scalePerRing = 10;

    public int initialOrbits = 2;
    public int[] orbitCapacity = new[] { 2, 4 };
    public float[] orbitDistance = new float[] { 5, 10 };
    public int[] orbitFrames = new[] { 600, 900 };

    public int initialNumberOfPlanets = 5;

    private NeederController needer;
    private OrbitGroup orbits;

    private Vector2 initialPosition;

    public bool Alive
    {
        get
        {
            return gameObject.activeSelf;
        }
    }

    void Start()
    {
        ParentStart();
        damageToPlayerOnCollision = 999;

        needer = GetComponent<NeederController>();
        orbits = GetComponent<OrbitGroup>();

        initialPosition = transform.position;
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
        if (!needer.complete)
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
        if (needer.needs.gatheredMaterials.Count == 0)
        {
            Die();
        }
    }

    private void UpdateWidth()
    {
        var scale = baseScale + (needer.needs.gatheredMaterials.Count / ringSegmentSize) * scalePerRing;
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(scale, scale, transform.localScale.z), 0.2f);
    }

    public void Spawn()
    {
        transform.position = initialPosition;

        for (int i = 0; i < initialOrbits; i++)
        {
            orbits.AddOrbit(transform, orbitCapacity[i], orbitFrames[i], orbitDistance[i]);
        }

        for (int i = 0; i < initialNumberOfPlanets; i++)
        {
            orbits.AddOrbiter();
        }

        needer.Reset(3, 2, 5, 0.5f);
        gameObject.SetActive(true);
    }

    public override void Die()
    {
        orbits.Die();

        gameObject.SetActive(false);
    }
}
