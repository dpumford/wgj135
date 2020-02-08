using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    Vector2 direction;
    Vector2 rotation;

    Rigidbody2D myBody;
    Laser laser;
    OrbitQueue orbit;

    public int speed = 10;
    public float shootSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        direction = Vector2.zero;
        rotation = Vector2.zero;

        myBody = GetComponent<Rigidbody2D>();
        laser = GetComponentInChildren<Laser>();
        orbit = GetComponent<OrbitQueue>();
    }

    void Update()
    {
        CheckLaserFire();
        CheckOrbitalFire();
        CheckOrbitQueue();
    }

    void FixedUpdate()
    {
        SetShipDirection();
        SetShipRotation();

        myBody.AddForce(direction * speed);
    }

    void SetShipDirection()
    {
        Vector2 newDirection = Vector2.zero;

        newDirection += Input.GetAxis("Vertical") * Vector2.up;

        newDirection += Input.GetAxis("Horizontal") * Vector2.right;

        direction = newDirection;
    }

    void SetShipRotation()
    {
        rotation = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - myBody.position;

        float rotationAngle = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        myBody.MoveRotation(rotationAngle);
    }

    void CheckLaserFire()
    {
        if (Input.GetMouseButtonUp(0))
        {
            laser.Fire();
        }
    }

    void CheckOrbitalFire()
    {
        if (Input.GetMouseButton(1))
        {
            orbit.PrepareFire();
        } 
        else if (Input.GetMouseButtonUp(1))
        {
            orbit.Fire(rotation.normalized * shootSpeed);
        }
    }

    void CheckOrbitQueue()
    {
        orbit.CollectOrbiters(laser.GetColliders());
    }
}
