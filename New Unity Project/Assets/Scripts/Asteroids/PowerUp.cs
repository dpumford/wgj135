using System;
using UnityEngine;

[Serializable]
public class PowerUp
{
    public float duration;
    private float lifetimeTimer;

    public virtual bool ShouldApply => lifetimeTimer < duration;
    public virtual ShipModifications Modifications => new ShipModifications();

    public void UpdateLifetimeTimer(float deltaTime)
    {
        lifetimeTimer += deltaTime;
    }
}
