﻿using UnityEngine;
using System.Linq;
using MyBox;

public class AsteroidController : CelestialBody
{
    public Sprite[] asteroidImages;
    public SpriteRenderer halo;
    public Material material;

    ParticleSystem myParticles;

    public float closestFudge = .5f;
    public float startTorqueMin = -.5f;
    public float startTorqueMax = .5f;

    public int miningFrames = 60;
    int currentMiningFrame = 0;

    public int safeFireFrames = 5;

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
        }

        if (state == CelestialState.Mining || state == CelestialState.Mined)
        {
            myBody.velocity = Vector2.zero;
            currentMiningFrame--;

            if (currentMiningFrame < 0)
            {
                currentMiningFrame = 0;
                state = CelestialState.Mined;
                GetComponent<SpriteRenderer>().color = material.MaterialColor();
            }
        }
        else
        {
            ParentFixedUpdate();
        }

        if (Mathf.Abs(transform.position.x) > 100 || Mathf.Abs(transform.position.y) > 100)
        {
            Die();
        }

        if (state == CelestialState.Firing)
        {
            if (safeFireFrames == 0)
            {
                myParticles.Play();
                myCollider.enabled = true;
                state = CelestialState.MinedFired;
            }
            safeFireFrames--;
        }

        halo.enabled = state == CelestialState.Selected;
    }

    public void StartMining()
    {
        currentMiningFrame = miningFrames;
        state = CelestialState.Mining;
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

    public override void HandlePlayerCollision()
    {
        Die();
    }

    public override void Die()
    {
        Destroy(gameObject);
    }
}
