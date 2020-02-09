using System.Collections;
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
    OrbitQueue orbit;
    ShootController shooter;

    public int speed = 10;
    public float shootSpeed = 5;
    public int maxHealth = 3;

    int currentHealth;
    PlayerState state;

    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<PolygonCollider2D>();
        laser = GetComponentInChildren<Laser>();
        orbit = GetComponent<OrbitQueue>();
        shooter = GetComponent<ShootController>();
        renderer = GetComponent<SpriteRenderer>();
    }

    public void Spawn(Vector2 spawnPoint)
    {
        Debug.Log("Spawning!");
        state = PlayerState.Alive;
        renderer.enabled = true;
        myCollider.enabled = true;

        transform.position = spawnPoint;
        direction = Vector2.zero;
        rotation = Vector2.zero;
        currentHealth = maxHealth;

        Debug.Log("Health: " + currentHealth);
    }

    public void Die()
    {
        Debug.Log("Dying!");
        state = PlayerState.Dead;
        renderer.enabled = false;
        myCollider.enabled = false;
        myBody.velocity = Vector2.zero;
        orbit.Clear();
    }

    void Update()
    {
        if (state == PlayerState.Dead)
        {
            return;
        }

        CheckLaserFire();
        CheckOrbitalFire();
        CheckOrbitQueue();
    }

    void FixedUpdate()
    {
        if (state == PlayerState.Dead)
        {
            return;
        }

        SetShipDirection();
        SetShipRotation();

        if (state == PlayerState.Alive)
        {
            myBody.AddForce(direction * speed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        AsteroidController asteroid = collision.gameObject.GetComponent<AsteroidController>();
        StarController star = collision.gameObject.GetComponent<StarController>();
        BlackHoleController blackHole = collision.gameObject.GetComponent<BlackHoleController>();

        CelestialBody body = (CelestialBody)asteroid ?? (CelestialBody)star ?? (CelestialBody)blackHole;

        if (body != null)
        {
            currentHealth -= body.damageToPlayerOnCollision;

            if (currentHealth < 0)
            {
                currentHealth = 0;
            }

            Debug.Log("Health: " + currentHealth);
            body.HandlePlayerCollision();
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
            state = PlayerState.Aiming;
            shooter.PrepareFire();
        } 
        else if (Input.GetMouseButtonUp(1))
        {
            state = PlayerState.Alive;
            shooter.Fire(rotation.normalized * shootSpeed);
        }
    }

    void CheckOrbitQueue()
    {
        orbit.CollectOrbiters(laser.GetColliders());
    }

    public int CurrentHealth()
    {
        return currentHealth;
    }
}
