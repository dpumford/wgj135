using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public float initialSpeed;

    protected Rigidbody2D myBody;
    protected CircleCollider2D myCollider;

    public CelestialState state = CelestialState.Collectible;
    public int damageToPlayerOnCollision;

    public float gravityMultiplier = 0.5f;
    protected Vector2 firedPosition = Vector2.zero;

    protected void ParentStart()
    {
        myBody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CircleCollider2D>();

        var initialAngle = Random.Range(0, Mathf.PI * 2);
        myBody.velocity = initialSpeed * new Vector2(Mathf.Cos(initialAngle), Mathf.Sin(initialAngle));
    }

    protected void ParentFixedUpdate()
    {
        if (state == CelestialState.Collectible || state == CelestialState.Free)
        {
            RunGravity();
        }
    }

    protected virtual void RunGravity()
    {
        var others = FindObjectsOfType<CelestialBody>();

        foreach (var other in others)
        {
            Rigidbody2D body = other.GetComponent<Rigidbody2D>();

            if (other != this && body != null && (other.state == CelestialState.Collectible || other.state == CelestialState.Free))
            {
                var directionToOther = other.transform.position - transform.position;

                myBody.AddForce(directionToOther.normalized * gravityMultiplier * myBody.mass * body.mass / directionToOther.sqrMagnitude);
            }
        }
    }

    public virtual void HandlePlayerCollision()
    {

    }

    public void SetOrbitingPosition(Vector2 newPosition)
    {
        if (state != CelestialState.Collectible || state != CelestialState.PrepareFire)
        {
            myBody.position = Vector2.Lerp(myBody.position, newPosition, 0.2f);
        }
    }

    public void Collect()
    {
        if (state == CelestialState.Mined)
        {
            state = CelestialState.Collected;
            myBody.velocity = Vector2.zero;
            myCollider.enabled = false;
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

    public virtual void OnFire()
    {

    }

    public void Fire(Vector2 velocity)
    {
        OnFire();
        state = CelestialState.Firing;
        myBody.velocity = velocity;
        firedPosition = transform.position;
    }

    public bool IsCollectible()
    {
        return state == CelestialState.Collectible || state == CelestialState.MinedFired;
    }

    public virtual void Die()
    {
        
    }
}
