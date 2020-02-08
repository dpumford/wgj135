using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : CelestialBody
{
    public SpriteRenderer halo;

    private void Start()
    {
        ParentStart();
        damageToPlayerOnCollision = 1;
    }

    void FixedUpdate()
    {
        ParentFixedUpdate();
        halo.enabled = state == CelestialState.Selected;
    }

    public override void HandlePlayerCollision()
    {
        GameObject.Destroy(gameObject);
    }
}
