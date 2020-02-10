﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    Vector2 direction;
    Vector2 rotation;

    Rigidbody2D myBody;
    PolygonCollider2D myCollider;
    SpriteRenderer renderer;
    Laser laser;
    Orbit orbit;
    ShootController shooter;

    public int speed = 10;
    public float shootSpeed = 5;
    public int maxHealth = 3;

    public int orbitCapacity = 10;
    public int orbitFrames = 240;
    public float orbitDistance = 2.0f;

    int currentHealth;
    bool aiming;

    void Start()
    {
    }

    public void Spawn(Vector2 spawnPoint)
    {
        myBody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<PolygonCollider2D>();
        laser = GetComponentInChildren<Laser>();
        orbit = GetComponent<Orbit>();
        shooter = GetComponent<ShootController>();
        renderer = GetComponent<SpriteRenderer>();

        Debug.Log("Spawning!");
        orbit.Init(transform, orbitCapacity, orbitFrames, orbitDistance);

        transform.position = spawnPoint;
        direction = Vector2.zero;
        rotation = Vector2.zero;
        currentHealth = maxHealth;

        Debug.Log("Health: " + currentHealth);
    }

    public void Die()
    {
        Debug.Log("Dying!");
        orbit.Clear();
        Destroy(gameObject);
    }

    void Update()
    {
        CheckHealth();
        CheckLaserFire();
        CheckOrbitalFire();
    }

    void FixedUpdate()
    {
        SetShipDirection();
        SetShipRotation();

        myBody.AddForce(direction * speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        AsteroidController asteroid = collision.gameObject.GetComponent<AsteroidController>();
        StarController star = collision.gameObject.GetComponent<StarController>();
        BlackHoleController blackHole = collision.gameObject.GetComponent<BlackHoleController>();

        CelestialBody body = (CelestialBody)asteroid ?? (CelestialBody)star ?? (CelestialBody)blackHole;

        if (body != null && (body.state == CelestialState.Free || body.state == CelestialState.Collectible))
        {
            currentHealth -= body.damageToPlayerOnCollision;
            currentHealth = Mathf.Max(0, currentHealth);

            Debug.Log("Health: " + currentHealth);
            body.HandlePlayerCollision();
        }
    }

    void CheckHealth()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void SetShipDirection()
    {
        Vector2 newDirection = Vector2.zero;

        newDirection += Input.GetAxis("Vertical") * Vector2.up;

        newDirection += Input.GetAxis("Horizontal") * Vector2.right;

        direction = newDirection;
    }

    void SetShipRotation()
    {
        rotation = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - myBody.position;

        float rotationAngle = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        myBody.MoveRotation(rotationAngle);
    }

    void CheckLaserFire()
    {
        if (Input.GetMouseButtonUp(0))
        {
            laser.Fire();
        }
    }

    void CheckOrbitalFire()
    {
        if (Input.GetMouseButton(1))
        {
            aiming = true;
            shooter.PrepareFire();
        } 
        else if (Input.GetMouseButtonUp(1))
        {
            aiming = false;
            shooter.Fire(rotation.normalized * shootSpeed);
        }
    }

    public int CurrentHealth()
    {
        return currentHealth;
    }
}
