using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : CelestialBody
{
    PlanetState planetState;
    public int maxHealth = 3;
    int currentHealth = 0;

    public int lifePercentage = 90;

    public int heatHealthLossFrames = 60;
    int currentHealthLossFrame = 0;

    public float explosionSpawnDistance = 3;
    public float explosionForceScale = 2;
    public int explosionSpreadMaxDeg = 120;
    public int explosionParticles = 3;
    public AsteroidController exploderPrefab;

    public TextMesh statusField;

    void Start()
    {
        damageToPlayerOnCollision = 3;

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

        state = CelestialState.Free;

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

            statusField.text = "Health: " + currentHealth + " Losing in " + (int)((float)currentHealthLossFrame / (float)heatHealthLossFrames * 100);
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

            if (currentHealth == 0)
            {
                Explode((collision.collider.transform.position - transform.position).normalized);
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
        return planetState != PlanetState.Dead && planetState != PlanetState.Fallow;
    }

    public override void Die()
    {
        Destroy(gameObject);
    }

    public override void OnFire()
    {
        planetState = PlanetState.LosingHeat;
    }

    public void Explode(Vector2 direction)
    {
        GetComponent<CircleCollider2D>().enabled = false;
        state = CelestialState.Collected;

        for (int i = 0; i < explosionParticles; i++)
        {
            float rotAngle = Random.Range(-explosionSpreadMaxDeg / 2, explosionSpreadMaxDeg / 2) * Mathf.Deg2Rad;

            direction.x = Mathf.Cos(direction.x * rotAngle);
            direction.y = Mathf.Sin(direction.y * rotAngle);

            AsteroidController asteroid = Instantiate(exploderPrefab.gameObject, transform.position + (Vector3)(direction * 3), Quaternion.identity).GetComponent<AsteroidController>();

            asteroid.Init(GameController.RandomMaterial());

            direction *= explosionForceScale;
            asteroid.GetComponent<Rigidbody2D>().AddForce(direction);
        }

        gameObject.SetActive(false);
    }
}
