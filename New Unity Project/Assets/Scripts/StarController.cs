using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : CelestialBody
{
    public float destinationForce = 1000000;

    public int ringSegmentSize = 3;
    public float baseScale = 20;
    public float scalePerRing = 10;

    public int initialNumberOfPlanets = 2;
    public CelestialBody planetPrefab;

    private NeederController needer;
    private OrbitQueue orbiter;

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
        orbiter = GetComponent<OrbitQueue>();

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
        transform.localScale = new Vector3(scale, scale, transform.localScale.z);
    }

    public void Spawn()
    {
        transform.position = initialPosition;

        for (int i = 0; i < initialNumberOfPlanets; i++)
        {
            orbiter.AddOrbiter(Instantiate(planetPrefab.gameObject, transform.position + Vector3.one * orbiter.spinDistance, Quaternion.identity).GetComponent<CelestialBody>());
        }

        needer.Reset(3, 2, 5, 0.5f);
        gameObject.SetActive(true);
    }

    public override void Die()
    {
        foreach (var planet in orbiter.orbiters)
        {
            planet.Die();
        }

        orbiter.Clear();

        gameObject.SetActive(false);
    }
}
