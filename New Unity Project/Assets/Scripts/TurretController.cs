using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public int MaxTurretShots = 2;
    public int turretShots = 2;

    CircleCollider2D shootRadius;

    public SpriteRenderer aoe;

    private void Start()
    {
        shootRadius = GetComponent<CircleCollider2D>();
    }

    private void FixedUpdate()
    {
        if (turretShots == 0)
        {
            aoe.enabled = false;
            shootRadius.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        AsteroidController asteroid = other.gameObject.GetComponent<AsteroidController>();

        if (asteroid != null)
        {
            asteroid.Die();
            turretShots--;
        }
    }

    public void Reload()
    {
        turretShots = MaxTurretShots;
        aoe.enabled = true;
        shootRadius.enabled = true;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
