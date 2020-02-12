using System;
using UnityEngine;

[Serializable]
public class PowerUpLazer : PowerUp
{
    [Min(0)]
    public float percentLazerRangeIncrease;

    public override ShipModifications Modifications => new ShipModifications() { percentLazerRangeIncrease = percentLazerRangeIncrease };
}