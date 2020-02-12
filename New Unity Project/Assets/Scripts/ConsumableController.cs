using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ShipModifications
{
    public float percentSpeedBoost;
    public float percentLazerRangeIncrease;
    public bool invincible;
    public float percentageAsteroidSpeedIncrease;
}

public class ConsumableController : MonoBehaviour
{
    private PowerUp currentPowerup;
    public ShipModifications CurrentShipModifications
    {
        get
        {
            if (currentPowerup == null)
            {
                return new ShipModifications();
            }
            else
            {
                return currentPowerup.Modifications;
            }
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (currentPowerup != null)
        {
            currentPowerup.UpdateLifetimeTimer(Time.deltaTime);

            if (!currentPowerup.ShouldApply)
            {
                currentPowerup = null;
            }
        }
    }

    public void ConsumeAsteroid(AsteroidController asteroid)
    {
        currentPowerup = asteroid.SelectedPowerUp;

        if (asteroid != null)
        {
            asteroid.Die();
        }
    }
}
