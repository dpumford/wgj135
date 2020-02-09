using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : CelestialBody
{
    public Vector2 destination;
    public float destinationForce = 1000000;

    private NeederController needer;
    private Rigidbody2D myBody;

    void Start()
    {
        ParentStart();
        damageToPlayerOnCollision = 999;

        needer = GetComponent<NeederController>();
        myBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        ParentFixedUpdate();
    }

    void Update()
    {
        UpdatePosition();
    }

    public void Reset(Vector2 destination)
    {
        this.destination = destination;
    }

    private void UpdatePosition()
    {
        if (!needer.complete)
        {
            myBody.AddForce((destination - (Vector2)transform.position).normalized * destinationForce);
        }
    }
}
