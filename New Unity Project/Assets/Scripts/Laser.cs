using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public OrbitQueue destinationQueue;

    LineRenderer line;
    BoxCollider2D myCollider;

    public int duration_frames = 10;
    int frames_remaining = 0;
    
    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.enabled = false;

        myCollider = GetComponent<BoxCollider2D>();
        myCollider.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        CelestialBody body = other.gameObject.GetComponent<CelestialBody>();

        if (body != null && body.IsCollectible())
        {
            destinationQueue.AddOrbiter(body);
        }
    }

    public void Fire()
    {
        if (!line.enabled)
        {
            frames_remaining = duration_frames;
        }
    }

    void FixedUpdate()
    {
        if (frames_remaining > 0)
        {
            myCollider.enabled = true;
            line.enabled = true;
            frames_remaining -= 1;
        }
        else if (frames_remaining == 0)
        {
            myCollider.enabled = false;
            line.enabled = false;
        }
    }
}
