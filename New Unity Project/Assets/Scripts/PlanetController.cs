using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : CelestialBody
{
    PlanetState planetState;
    int spawnSafetyFrames = 10;

    public Sprite[] liveSprites;
    public Sprite[] deadSprites;
    SpriteRenderer spriteRenderer;

    public int maxHealth = 0;
    int currentHealth = 0;

    public int lifePercentage = 90;

    public int heatHealthLossFrames = 60;
    int currentHealthLossFrame = 0;

    public int heatHeathLossFuzz = 40;

    public float explosionSpawnDistance = 3;
    public float explosionForceScale = 2;
    public int explosionSpreadMaxDeg = 120;
    public int explosionParticles = 3;
    public AsteroidController[] exploderPrefabs;
    public ExplosionController explosionPrefab;
    public ExplosionController asteroidExplosionPrefab;

    public TurretController turretPrefab;
    TurretController turret;
    bool hasTurret = false;

    RadialProgress healthProgress;

    void Start()
    {
        damageToPlayerOnCollision = 3;
        heatHealthLossFrames += Random.Range(-heatHeathLossFuzz, heatHeathLossFuzz);

        ParentStart();
    }

    public void Init(PlanetState state, int health, bool spawnsWithTurret)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        var uiControl = FindObjectOfType<UIController>();
        var color = Color.green;
        color.a = .65f;
        healthProgress = uiControl.CreateRadialProgress(transform, Vector2.zero, Vector2.one * .4f, color, 1f, 0f, true, true);

        planetState = state;
        maxHealth = health;
        currentHealth = health;

        ChangeSprite();

        if (spawnsWithTurret)
        {
            turret = Instantiate(turretPrefab.gameObject, transform).GetComponent<TurretController>();
            hasTurret = true;
        }
    }

    void FixedUpdate()
    {
        ParentFixedUpdate();

        if (spawnSafetyFrames > 0)
        {
            spawnSafetyFrames--;
            return;
        }

        if (planetState == PlanetState.Fallow)
        {
            return;
        }

        if (planetState == PlanetState.LosingHeat)
        {
            currentHealthLossFrame++;

            if (currentHealthLossFrame == heatHealthLossFrames)
            {
                currentHealth--;
                currentHealthLossFrame = 0;
            }

            planetState = currentHealth == 0 ? PlanetState.Dead : planetState;

            if (planetState == PlanetState.Dead)
            {
                if (healthProgress != null)
                {
                    healthProgress.Die();
                }
                ChangeSprite();
            }
        }
        else if (planetState == PlanetState.Alive)
        {
            if (healthProgress != null)
            {
                healthProgress.PercentOfFrames(currentHealth, maxHealth);
                if (healthProgress.percentFilled < .33f)
                {
                    var badColor = Color.red;
                    badColor.a = 0.65f;

                    healthProgress.Recolor(badColor);
                }
                else if (healthProgress.percentFilled < .66f)
                {
                    var mediumColor = Color.yellow;
                    mediumColor.a = 0.65f;

                    healthProgress.Recolor(mediumColor);
                }
            }

            planetState = currentHealth == 0 ? PlanetState.Dead : planetState;

            if (planetState == PlanetState.Dead || planetState == PlanetState.LosingHeat)
            {
                if (healthProgress != null)
                {
                    healthProgress.Die();
                }
                ChangeSprite();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (spawnSafetyFrames > 0)
        {
            return;
        }

        var asteroid = collision.gameObject.GetComponent<AsteroidController>();

        if (asteroid != null)
        {
            if (asteroid.GivesMaterial() && asteroid.material == Material.Turret)
            {
                if (!hasTurret)
                {
                    turret = Instantiate(turretPrefab.gameObject, transform).GetComponent<TurretController>();
                    hasTurret = true;
                } 
                else
                {
                    turret.Reload();
                }
            }
            else if (planetState == PlanetState.Alive)
            {
                currentHealth -= asteroid.damageToPlayerOnCollision;
                currentHealth = currentHealth > 0 ? currentHealth : 0;

                Instantiate(asteroidExplosionPrefab, asteroid.transform.position, Quaternion.identity);
            }

            if (currentHealth == 0)
            {
                Explode((collision.collider.transform.position - transform.position).normalized);
            }

            asteroid.Die();
            return;
        }

        MissileController missile = collision.gameObject.GetComponent<MissileController>();

        if (missile != null && missile.DamagesPlanets())
        {
            if (planetState == PlanetState.Alive)
            {
                currentHealth -= missile.damage;
                currentHealth = currentHealth > 0 ? currentHealth : 0;

                if (currentHealth == 0)
                {
                    Explode((collision.collider.transform.position - transform.position).normalized);
                }
            } 
            else
            {
                Explode((collision.collider.transform.position - transform.position).normalized);
            }
            return;
        }

        var star = collision.gameObject.GetComponent<StarController>();

        if (star != null)
        {
            Explode((collision.collider.transform.position - transform.position).normalized);
            return;
        }

        var planet = collision.gameObject.GetComponent<PlanetController>();

        if (planet != null)
        {
            Explode((collision.collider.transform.position - transform.position).normalized);
            return;
        }
    }

    public bool IsAlive()
    {
        return planetState != PlanetState.Dead && planetState != PlanetState.Fallow;
    }

    public override void Die()
    {
        if (healthProgress != null)
        {
            healthProgress.Die();
        }

        Destroy(gameObject);
    }

    public override void OnFire()
    {
        planetState = PlanetState.LosingHeat;
    }

    public void Explode(Vector2 direction)
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        GetComponent<CircleCollider2D>().enabled = false;
        state = CelestialState.Collected;

        for (int i = 0; i < explosionParticles; i++)
        {
            float rotAngle = Random.Range(-explosionSpreadMaxDeg / 2, explosionSpreadMaxDeg / 2) * Mathf.Deg2Rad;

            direction.x = Mathf.Cos(direction.x * rotAngle);
            direction.y = Mathf.Sin(direction.y * rotAngle);

            AsteroidController asteroid = Instantiate(exploderPrefabs[Random.Range(0, exploderPrefabs.Length)].gameObject, transform.position + (Vector3)(direction * 3), Quaternion.identity)
                .GetComponent<AsteroidController>();

            direction *= explosionForceScale;
            asteroid.GetComponent<Rigidbody2D>().AddForce(direction);
        }

        if (healthProgress != null)
        {
            healthProgress.Die();
        }

        //TODO: Make this so that Die() can be called
        gameObject.SetActive(false);
    }

    private void ChangeSprite()
    {
        if (planetState == PlanetState.Dead || planetState == PlanetState.Fallow)
        {
            spriteRenderer.sprite = deadSprites[Random.Range(0, deadSprites.Length)];
        }
        else
        {
            spriteRenderer.sprite = liveSprites[Random.Range(0, liveSprites.Length)];
        }
    }
}
