using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    public float speed;
    AsteroidController target;
    Rigidbody2D myBody;
    public int damage = 1;
    public int planetSafetyFrames = 8;

    // Start is called before the first frame update
    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        planetSafetyFrames--;

        if (planetSafetyFrames < 0)
        {
            planetSafetyFrames = 0;
        }

        if (target != null)
        {
            Vector2 directionToTarget = target.transform.position - transform.position;

            float rotationAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

            myBody.MoveRotation(rotationAngle);

            myBody.velocity = Vector2.Lerp(myBody.velocity, directionToTarget.normalized * speed, 0.3f);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        AsteroidController asteroid = other.gameObject.GetComponent<AsteroidController>();

        if (asteroid != null)
        {
            asteroid.Die();
            Die();
        }

        if (planetSafetyFrames <= 0)
        {
            Die();
        }
    }

    public bool DamagesPlanets()
    {
        return planetSafetyFrames <= 0;
    }

    public void SetTarget(AsteroidController t)
    {
        target = t;
    }
    
    public void Die()
    {
        Destroy(gameObject);
    }
}
