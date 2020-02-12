using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public Orbit destinationQueue;
    public LaserState state;

    AsteroidController minedBody = null;

    LineRenderer line;
    BoxCollider2D myCollider;
    ConsumableController consumer;

    Vector3 originalScale;
    
    void Start()
    {
        state = LaserState.Free;
        line = GetComponent<LineRenderer>();
        line.enabled = false;

        myCollider = GetComponent<BoxCollider2D>();
        myCollider.enabled = false;
        originalScale = transform.localScale;

        consumer = GetComponentInParent<ConsumableController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        AsteroidController body = other.gameObject.GetComponent<AsteroidController>();

        if (body != null && body.IsCollectible())
        {
            body.StartMining();
            state = LaserState.Mining;
            minedBody = body;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        AsteroidController body = other.gameObject.GetComponent<AsteroidController>();

        if (body != null && body == minedBody && !body.IsMinedOut())
        {
            Debug.Log("Laser dropping body");
            body.StopMining();
            minedBody = null;
        }
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && state == LaserState.Mining)
        {
            if (minedBody != null)
            {
                if (minedBody.IsMinedOut())
                {
                    destinationQueue.AddOrbiter(minedBody);
                }
            }
        }
        else if (Input.GetMouseButton(0) && state == LaserState.Free)
        {
            transform.localScale = originalScale + consumer.CurrentShipModifications.percentLazerRangeIncrease * originalScale;

            myCollider.enabled = true;
            line.enabled = true;

            state = LaserState.Mining;
        }
        else
        {
            myCollider.enabled = false;
            line.enabled = false;

            if (minedBody != null)
            {
                Debug.Log("Player stopped mining");
                minedBody.StopMining();
                minedBody = null;
            }

            state = LaserState.Free;
        }
    }
}
