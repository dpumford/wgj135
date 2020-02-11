using System;
using UnityEngine;

[Serializable]
public class PowerUpSpeed: PowerUp
{
    [Range(0, 1)]
    public float percentSpeedBoost;
}
