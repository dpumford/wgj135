using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public float initialSpeed;
    public float initialAngle;
    public SpriteRenderer halo;

    private Rigidbody2D myBody;

    public CelestialState state = CelestialState.Collectible;

    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();

        initialAngle = Random.Range(0, Mathf.PI * 2);
        myBody.velocity = initialSpeed * new Vector2(Mathf.Cos(initialAngle), Mathf.Sin(initialAngle));

        if (halo == null)
        {
            Debug.LogError("I'm a celestial body with no halo! " + transform.name);
        }
    }

    void FixedUpdate()
    {
        if (state == CelestialState.Collectible || state == CelestialState.Fired)
        {
            RunGravity();
        }

        if (halo != null)
        {
            halo.enabled = state == CelestialState.Selected;
        }
    }

    void RunGravity()
    {
        var others = FindObjectsOfType<CelestialBody>();

        foreach (var other in others)
        {
            if (other != this && other.IsCollectible())
            {
                var directionToOther = other.transform.position - transform.position;
                myBody.AddForce(directionToOther.normalized * other.GetComponent<Rigidbody2D>().mass / directionToOther.sqrMagnitude);
            }
        }
    }

    public void SetOrbitingPosition(Vector2 newPosition)
    {
        if (state != CelestialState.Collectible)
        {
            myBody.position = newPosition;
        }
    }

    public void Collect()
    {
        if (state == CelestialState.Collectible)
        {
            state = CelestialState.Collected;
            myBody.velocity = Vector2.zero;
        }
    }

    public void Select()
    {
        state = CelestialState.Selected;
    }

    // This should only be called on orbiters that have been collected
    public void Deselect()
    {
        state = CelestialState.Collected;
    }

    public void PrepareFire()
    {
        state = CelestialState.PrepareFire;
    }

    public void Fire(Vector2 velocity)
    {
        state = CelestialState.Fired;
        myBody.velocity = velocity;
    }

    public bool IsCollectible()
    {
        return state == CelestialState.Collectible;
    }
}
