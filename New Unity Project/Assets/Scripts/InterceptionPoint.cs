using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterceptionPoint : MonoBehaviour
{
    public SpriteRenderer reticle;
    public CircleCollider2D reticleCollider;
    public Laser myLaser;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        AsteroidController body = other.gameObject.GetComponent<AsteroidController>();

        if (body != null && body.IsCollectible())
        {
            body.StartMining();
            myLaser.state = LaserState.Mining;
            myLaser.minedBody = body;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        AsteroidController body = other.gameObject.GetComponent<AsteroidController>();

        if (body != null && body == myLaser.minedBody && !body.IsMinedOut())
        {
            body.StopMining();
            myLaser.minedBody = null;
        }
    }
}
