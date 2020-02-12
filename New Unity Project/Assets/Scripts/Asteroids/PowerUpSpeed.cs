using System;
using UnityEngine;

[Serializable]
public class PowerUpSpeed: PowerUp
{
    [Min(0)]
    public float percentSpeedBoost;

    public override ShipModifications Modifications => new ShipModifications() { percentSpeedBoost = percentSpeedBoost };
}
