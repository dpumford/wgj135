using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : CelestialBody
{
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

    private void UpdatePosition()
    {
        if (!needer.complete)
        {
            var destination = Vector2.zero;

            foreach (var star in FindObjectsOfType<StarController>())
            {
                if (star != this)
                {
                    destination += (Vector2)star.transform.position;
                }
            }

            myBody.AddForce((destination - (Vector2)transform.position).normalized * destinationForce);
        }
    }
}
