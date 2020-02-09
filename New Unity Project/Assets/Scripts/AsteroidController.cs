using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : CelestialBody
{
    public SpriteRenderer halo;
    public Material material;

    private void Start()
    {
        ParentStart();
        damageToPlayerOnCollision = 1;
    }

    public void Init(Material m)
    {
        material = m;
        GetComponent<SpriteRenderer>().color = m.MaterialColor();
    }

    void FixedUpdate()
    {
        ParentFixedUpdate();
        halo.enabled = state == CelestialState.Selected;
    }

    public override void HandlePlayerCollision()
    {
        Die();
    }

    public override void Die()
    {
        Destroy(gameObject);
    }
}
