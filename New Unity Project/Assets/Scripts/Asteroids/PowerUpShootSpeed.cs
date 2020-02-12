using System;
using UnityEngine;

[Serializable]
public class PowerUpShootSpeed : PowerUp
{
    [Min(0)]
    public float percentageAsteroidSpeedIncrease;

    public override ShipModifications Modifications => new ShipModifications() { percentageAsteroidSpeedIncrease = percentageAsteroidSpeedIncrease };
}
