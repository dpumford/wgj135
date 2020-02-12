using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public int maxTurretShots = 2;
    public int turretShots = 2;

    CircleCollider2D shootRadius;

    public SpriteRenderer aoe;
    public MissileController missilePrefab;

    HashSet<AsteroidController> targetedAsteroids;

    private void Start()
    {
        shootRadius = GetComponent<CircleCollider2D>();
        targetedAsteroids = new HashSet<AsteroidController>();
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

        if (asteroid != null && !targetedAsteroids.Contains(asteroid))
        {
            targetedAsteroids.Add(asteroid);
            var missile = Instantiate(missilePrefab.gameObject, transform.position, Quaternion.identity);
            missile.GetComponent<MissileController>().SetTarget(asteroid);
            turretShots--;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        AsteroidController asteroid = other.gameObject.GetComponent<AsteroidController>();

        if (asteroid != null && targetedAsteroids.Contains(asteroid))
        {
            targetedAsteroids.Remove(asteroid);
        }
    }

    public void Reload()
    {
        turretShots = maxTurretShots;
        aoe.enabled = true;
        shootRadius.enabled = true;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
