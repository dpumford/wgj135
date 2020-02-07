using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float initialSpeed;
    public float angle;

    private Rigidbody2D rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        angle = Random.Range(0, Mathf.PI * 2);
    }

    // Update is called once per frame
    void Update()
    {
        //var otherAsteroids = FindObjectsOfType<Asteroid>();

        rigidbody.velocity = initialSpeed * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
}
