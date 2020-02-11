using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitGroup : MonoBehaviour
{
    public Orbit orbitPrefab;
    public CelestialBody planetPrefab;

    List<Orbit> orbits = new List<Orbit>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddOrbit(Transform follow, int capacity, int spinFrames, float spinDistance)
    {
        Orbit o = Instantiate(orbitPrefab.gameObject, transform).GetComponent<Orbit>();
        o.Init(follow, capacity, spinFrames, spinDistance);
        orbits.Add(o);
    }

    public void AddOrbiter(PlanetState state, int health)
    {
        foreach (var orbit in orbits)
        {
            if (!orbit.IsFull())
            {
                // TODO: Change this if we want to use an orbit group for the player's asteroids at any point
                PlanetController body = Instantiate(planetPrefab.gameObject, transform.position + Vector3.one * orbit.spinDistance, Quaternion.identity).GetComponent<PlanetController>();
                body.Init(state, health);
                orbit.AddOrbiter(body);
                break;
            }
        }
    }

    public void Clear()
    {
        foreach (var orbit in orbits)
        {
            orbit.Clear();
        }
    }

    public void Die()
    {
        foreach (var orbit in orbits)
        {
            orbit.Die();
        }

        orbits.Clear();
    }
}
