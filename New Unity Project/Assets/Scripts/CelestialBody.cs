﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public float initialSpeed;
    public float initialAngle;

    private Rigidbody2D myBody;

    public CelestialState state = CelestialState.Collectible;

    // Start is called before the first frame update
    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();

        initialAngle = Random.Range(0, Mathf.PI * 2);
        myBody.velocity = initialSpeed * new Vector2(Mathf.Cos(initialAngle), Mathf.Sin(initialAngle));
    }

    void FixedUpdate()
    {
        if (state == CelestialState.Collectible)
        {
            RunGravity();
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

        Debug.Log($"{transform.name}: {myBody.velocity}");
    }

    public void SetOrbitingPosition(Vector2 newPosition)
    {
        if (state != CelestialState.Collectible)
        {
            //TODO: Lerp into position?
            myBody.position = newPosition;
        }
    }

    public void Collect()
    {
        state = CelestialState.Collected;
        myBody.velocity = Vector2.zero;
    }

    public void Fire(Vector2 velocity)
    {
        state = CelestialState.Collectible;
        myBody.velocity = velocity;
    }

    public bool IsCollectible()
    {
        return state == CelestialState.Collectible;
    }
}