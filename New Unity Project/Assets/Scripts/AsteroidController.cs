using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public SpriteRenderer halo;
    CelestialBody body;

    void Start()
    {
        body = GetComponent<CelestialBody>();
    }

    void FixedUpdate()
    {
        halo.enabled = body.state == CelestialState.Selected;
    }
}
