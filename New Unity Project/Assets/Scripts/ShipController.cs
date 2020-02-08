using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    Vector2 direction;
    Vector2 rotation;
    Rigidbody2D myBody;

    public int speed = 10;

    // Start is called before the first frame update
    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        direction = Vector2.zero;
        rotation = Vector2.zero;
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
}
