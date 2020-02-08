using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public float initialSpeed;
    public float initialAngle;

    private Rigidbody2D rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        initialAngle = Random.Range(0, Mathf.PI * 2);
        rigidbody.velocity = initialSpeed * new Vector2(Mathf.Cos(initialAngle), Mathf.Sin(initialAngle));
    }

    // Update is called once per frame
    void Update()
    {
        var others = FindObjectsOfType<CelestialBody>();

        foreach (var other in others) {
            if (other != this) {
                rigidbody.AddForce((other.transform.position - transform.position) * other.rigidbody.mass);
            }
        }

        Debug.Log($"{transform.name}: {rigidbody.velocity}");
    }
}
