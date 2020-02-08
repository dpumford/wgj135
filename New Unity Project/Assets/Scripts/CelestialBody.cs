using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public float initialSpeed;
    public float initialAngle;

    private Rigidbody2D myBody;

    public bool collectible = true;

    // Start is called before the first frame update
    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();

        initialAngle = Random.Range(0, Mathf.PI * 2);
        myBody.velocity = initialSpeed * new Vector2(Mathf.Cos(initialAngle), Mathf.Sin(initialAngle));
    }

    void FixedUpdate()
    {
        if (collectible)
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
                myBody.AddForce((other.transform.position - transform.position) * other.GetComponent<Rigidbody2D>().mass);
            }
        }

        Debug.Log($"{transform.name}: {myBody.velocity}");
    }

    public void SetOrbitingPosition(Vector2 newPosition)
    {
        if (!collectible)
        {
            //TODO: Lerp into position?
            myBody.position = newPosition;
        }
    }

    public void Collect()
    {
        Debug.Log("I've been collected");
        collectible = false;
        myBody.velocity = Vector2.zero;
    }

    public bool IsCollectible()
    {
        return collectible;
    }
}
