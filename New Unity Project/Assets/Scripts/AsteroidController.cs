using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : CelestialBody
{
    public SpriteRenderer halo;

    private void Start()
    {
        damageToPlayerOnCollision = 1;
    }

    void FixedUpdate()
    {
        halo.enabled = state == CelestialState.Selected;
    }

    public override void HandlePlayerCollision()
    {
        GameObject.Destroy(gameObject);
    }
}
