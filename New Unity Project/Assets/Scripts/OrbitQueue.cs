using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitQueue : MonoBehaviour
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

    void Start()
    {
        parentTransform = GetComponentInParent<Transform>();
    }

    void FixedUpdate()
    {
        UpdateOrbiterPositions();
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

        currentFrame++;
        currentFrame %= spinFrames;
    }

    public void AddOrbiter(CelestialBody body)
    {
        if (orbiters.Count < capacity)
        {
            body.Collect();
            orbiters.Add(body);
        }
    }

    public void Clear()
    {
        orbiters.Clear();
    }
}
