using System;
using UnityEngine;

[Serializable]
public class PowerUp
{
    public float duration;
    private float lifetimeTimer;

    public virtual bool ShouldApply
    {
        get
        {
            return lifetimeTimer < duration;
        }
    }

    public virtual float PercentSpeedBoost
    {
        get
        {
            return 0;
        }
    }

    public virtual bool Invincible
    {
        get
        {
            return false;
        }
    }

    public void UpdateLifetimeTimer(float deltaTime)
    {
        lifetimeTimer += deltaTime;
    }
}
