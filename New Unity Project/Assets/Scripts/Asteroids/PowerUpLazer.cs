using System;
using UnityEngine;

[Serializable]
public class PowerUpLazer : PowerUp
{
    [Range(0, 1)]
    public float percentLazerRangeIncrease;

    public override float PercentLazerRangeIncrease => percentLazerRangeIncrease;
}