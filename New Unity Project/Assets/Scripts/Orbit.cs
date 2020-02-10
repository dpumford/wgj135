using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public List<CelestialBody> orbiters
    {
        get;
        private set;
    } = new List<CelestialBody>();

    public int capacity = 10;

    public int spinFrames = 240;
    public float spinDistance = 1;
    int currentFrame = 0;

    Transform parentTransform;

    public void Init(Transform parentTransform, int capacity, int spinFrames, float spinDistance)
    {
        this.parentTransform = parentTransform;
        this.capacity = capacity;
        this.spinFrames = spinFrames;
        this.spinDistance = spinDistance;
        this.orbiters = new List<CelestialBody>();
    }

    void Start()
    {
        //parentTransform = GetComponentInParent<Transform>();
    }

    void FixedUpdate()
    {
        UpdateOrbiterPositions();
        currentFrame++;
        currentFrame %= spinFrames;
    }

    void OnDestroy()
    {
        foreach (var orbiter in orbiters)
        {
            orbiter.state = CelestialState.Free;
        }
    }

    void UpdateOrbiterPositions()
    {
        float degreeInterval = 360.0f / orbiters.Count;
        float spinProgress = 360 * currentFrame / spinFrames;

        int slot = 0;
        Vector2 orbiterPosition = Vector2.zero;

        foreach (var orbiter in orbiters)
        {
            float angle = (degreeInterval * slot + spinProgress) % 360;

            orbiterPosition.x = Mathf.Cos(Mathf.Deg2Rad * angle);
            orbiterPosition.y = Mathf.Sin(Mathf.Deg2Rad * angle);

            orbiterPosition *= spinDistance;

            orbiter.SetOrbitingPosition((Vector2)parentTransform.position + orbiterPosition);

            slot++;
        }
    }

    public void AddOrbiter(CelestialBody body)
    {
        if (orbiters.Count < capacity)
        {
            body.Collect();
            orbiters.Add(body);
        }
    }

    public bool IsFull()
    {
        Debug.Log("Orbiters " + orbiters.Count + " Max " + capacity);
        return orbiters.Count == capacity;
    }

    public void Clear()
    {
        orbiters.Clear();
    }

    public void Die()
    {
        foreach (var orbiter in orbiters)
        {
            Vector2 directionToCenter = orbiter.transform.position - transform.position;

            Vector2 normalToCenter = Vector2.Perpendicular(directionToCenter);

            // Assuming game runs at 60 FPS
            float orbitalVelocity = Mathf.PI * directionToCenter.magnitude * 2 / spinFrames * 60;

            orbiter.Fire(normalToCenter.normalized * orbitalVelocity);
        }

        Destroy(gameObject);
    }
}
