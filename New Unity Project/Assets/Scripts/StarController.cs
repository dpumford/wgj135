using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : CelestialBody
{
    void Start()
    {
        ParentStart();
        damageToPlayerOnCollision = 999;
    }

    void FixedUpdate()
    {
        ParentFixedUpdate();
    }
}
