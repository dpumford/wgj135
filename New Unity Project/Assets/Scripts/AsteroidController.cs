using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AsteroidController : CelestialBody
{
    public SpriteRenderer halo;
    public Material material;

    public float closestFudge = .5f;

    private void Start()
    {
        ParentStart();
        damageToPlayerOnCollision = 1;
    }

    public void Init(Material m)
    {
        material = m;
        GetComponent<SpriteRenderer>().color = m.MaterialColor();
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(transform.position.x) > 100 || Mathf.Abs(transform.position.y) > 100)
        {
            Die();
        }

        ParentFixedUpdate();
        halo.enabled = state == CelestialState.Selected;
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
