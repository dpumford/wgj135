using System;
using UnityEngine;

[Serializable]
public class PowerUp
{
    public float duration;
    private float lifetimeTimer;

    public virtual bool ShouldApply => lifetimeTimer < duration;
    public virtual float PercentSpeedBoost => 0;
    public virtual float PercentLazerRangeIncrease => 0;
    public virtual bool Invincible => false;

    public void UpdateLifetimeTimer(float deltaTime)
    {
        lifetimeTimer += deltaTime;
    }
}
