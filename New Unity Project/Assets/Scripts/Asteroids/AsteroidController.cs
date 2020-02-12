using UnityEngine;
using System.Linq;
using MyBox;

public class AsteroidController : CelestialBody
{
    public Sprite[] asteroidImages;
    public SpriteRenderer halo;
    public Material material;
    public Transform miningProgressPosition;

    RadialProgress miningProgress;

    ParticleSystem myParticles;

    public float closestFudge = .5f;
    public float startTorqueMin = -.5f;
    public float startTorqueMax = .5f;

    public int miningFrames = 60;
    int currentMiningFrame = 0;

    public int safeFireFrames = 5;
    int currentFireFrames = 0;

    [ConditionalField("material", false, Material.Helium)]
    public PowerUpShield shieldOptions;

    [ConditionalField("material", false, Material.Hydrogen)]
    public PowerUpSpeed speedOptions;

    [ConditionalField("material", false, Material.Lithium)]
    public PowerUpLazer lazerOptions;

    [ConditionalField("material", false, Material.Boron)]
    public PowerUpShootSpeed shootSpeedOptions;

    public PowerUp SelectedPowerUp
    {
        get
        {
            switch (material)
            {
                case Material.Hydrogen:
                    return speedOptions;
                case Material.Helium:
                    return shieldOptions;
                case Material.Lithium:
                    return lazerOptions;
                case Material.Boron:
                    return shootSpeedOptions;
                default:
                    return null;
            }
        }
    }

    private void Start()
    {
        ParentStart();
        damageToPlayerOnCollision = 1;

        halo.enabled = false;

        if (asteroidImages != null)
        {
            GetComponent<SpriteRenderer>().sprite = asteroidImages[Random.Range(0, asteroidImages.Length)];
        }

        myParticles = GetComponentInChildren<ParticleSystem>();
        var main = myParticles.main;
        main.startColor = material.MaterialColor();
        
        myBody.AddTorque(Random.Range(startTorqueMin, startTorqueMax));
    }

    void FixedUpdate()
    {
        if (state == CelestialState.Collected)
        {
            myParticles.Pause();
            return;
        }

        if (state == CelestialState.StartingMining)
        {
            myBody.velocity = Vector2.zero;
            
            miningProgress = uiControl.CreateRadialProgress(transform, new Vector2(.3f, .3f), 0f, 1f);
            state = CelestialState.Mining;
            return;
        }

        if (state == CelestialState.Mining)
        {
            myBody.velocity = Vector2.zero;
            currentMiningFrame++;

            miningProgress.PercentOfFrames(currentMiningFrame, miningFrames);

            if (currentMiningFrame > miningFrames)
            {
                currentMiningFrame = miningFrames;
                state = CelestialState.Mined;
                GetComponent<SpriteRenderer>().color = material.MaterialColor();
            }
            return;
        }

        if (state == CelestialState.Mined)
        {
            miningProgress = null;
            return;
        }

        if (Mathf.Abs(transform.position.x) > 100 || Mathf.Abs(transform.position.y) > 100)
        {
            Die();
        }

        if (state == CelestialState.Firing)
        {
            if (currentFireFrames == 0)
            {
                myParticles.Play();
                myCollider.enabled = true;
                state = CelestialState.MinedFired;
            }
            currentFireFrames--;
        }

        halo.enabled = state == CelestialState.Selected;
        ParentFixedUpdate();
    }

    public void StartMining()
    {
        currentMiningFrame = 0;
        state = CelestialState.StartingMining;
    }

    public void StopMining()
    {
        state = CelestialState.Collectible;
    }

    public bool IsMinedOut()
    {
        return state == CelestialState.Mined;
    }

    public bool GivesMaterial()
    {
        return state == CelestialState.MinedFired;
    }

    public void SetTurretAsteroid()
    {
        material = Material.Turret;
        GetComponent<SpriteRenderer>().color = material.MaterialColor();
    }

    protected override void RunGravity()
    {
        // Asteroids run gravity for everything
        var forces = (from star in FindObjectsOfType<StarController>() select star.GetComponent<Rigidbody2D>())
            .Concat(from planet in FindObjectsOfType<PlanetController>() select planet.GetComponent<Rigidbody2D>())
            .Concat(from blackHole in FindObjectsOfType<BlackHoleController>() select blackHole.GetComponent<Rigidbody2D>())
            .Concat(from asteroid in FindObjectsOfType<AsteroidController>() where asteroid != this && asteroid.state == CelestialState.Collectible select asteroid.GetComponent<Rigidbody2D>())
            .OrderBy(body => (body.transform.position - transform.position).sqrMagnitude)
            .Select(body => (body.transform.position - transform.position).normalized * gravityMultiplier * myBody.mass * body.mass / (body.transform.position - transform.position).sqrMagnitude);

        var first = true;
        // TODO: Maybe only take the first N forces?
        foreach (var force in forces)
        {
            // give the closest thing a little boost in pulling power
            if (first)
            {
                myBody.AddForce(force * closestFudge);
                first = false;
            }
            else
            {
                myBody.AddForce(force);
            }
        }
    }

    public override void OnFire()
    {
        currentFireFrames = safeFireFrames;
    }

    public override void HandlePlayerCollision()
    {
        Die();
    }

    public override void Die()
    {
        Destroy(gameObject);
    }
}
