using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : CelestialBody
{
    public float destinationForce = 1000000;

    public int ringSegmentSize = 3;
    public float baseScale = 20;
    public float scalePerRing = 10;

    private NeederController needer;

    void Start()
    {
        ParentStart();
        damageToPlayerOnCollision = 999;

        needer = GetComponent<NeederController>();
    }

    void FixedUpdate()
    {
        ParentFixedUpdate();
    }

    void Update()
    {
        UpdatePosition();
        UpdateWidth();
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

    private void UpdateWidth()
    {
        var scale = baseScale + (needer.needs.gatheredMaterials.Count / ringSegmentSize) * scalePerRing;
        transform.localScale = new Vector3(scale, scale, transform.localScale.z);
    }
}
