using System;
using UnityEngine;

[Serializable]
public class PowerUpShield: PowerUp
{
    //TODO: Maybe limit the number of free hits

    public override ShipModifications Modifications => new ShipModifications() { invincible = true };
}
