using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : CelestialBody
{
    PlanetState planetState;
    public int maxHealth = 3;
    int currentHealth = 0;

    public int lifePercentage = 40;

    public int heatHealthLossFrames = 60;
    int currentHealthLossFrame = 0;

    public TextMesh statusField;

    void Start()
    {
        if (Random.Range(0, 100) < lifePercentage)
        {
            planetState = PlanetState.Alive;
            currentHealth = maxHealth;
        } 
        else
        {
            planetState = PlanetState.Fallow;
        }

        statusField = GetComponentInChildren<TextMesh>();

        ParentStart();
    }

    void FixedUpdate()
    {
        ParentFixedUpdate();

        if (planetState == PlanetState.LosingHeat)
        {
            currentHealthLossFrame++;

            if (currentHealthLossFrame == heatHealthLossFrames)
            {
                currentHealth--;
                currentHealthLossFrame = 0;
            }

            planetState = currentHealth == 0 ? PlanetState.Dead : planetState;

            statusField.text = "Health: " + currentHealth + " Losing in " + ((float)currentHealthLossFrame / (float)heatHealthLossFrames * 100);
        }
        else if (planetState == PlanetState.Alive)
        {
            planetState = currentHealth == 0 ? PlanetState.Dead : planetState;
            statusField.text = "Health: " + currentHealth;
        }
        else if (planetState == PlanetState.Dead)
        {
            statusField.text = "Dead";
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var asteroid = collision.gameObject.GetComponent<AsteroidController>();

        if (asteroid != null)
        {
            if (planetState == PlanetState.Alive)
            {
                currentHealth -= asteroid.damageToPlayerOnCollision;
                currentHealth = currentHealth > 0 ? currentHealth : 0;
            }

            asteroid.Die();
        }
    }

    public void StartHeatHealthLoss()
    {
        if (planetState == PlanetState.Alive)
        {
            planetState = PlanetState.LosingHeat;
        }
    }

    public bool IsAlive()
    {
        return planetState == PlanetState.Dead;
    }

    public override void Die()
    {
        Destroy(gameObject);
    }

    public override void OnFire()
    {
        planetState = PlanetState.LosingHeat;
    }
}
