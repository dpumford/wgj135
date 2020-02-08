using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitQueue : MonoBehaviour
{
    List<CelestialBody> orbiters;
    public int capacity = 10;
    int selectedOrbiter;

    public int spinFrames = 240;
    public int spinDistance = 1;
    int currentFrame = 0;

    Transform parentTransform;
    Transform shootPosition;

    CelestialBody orbiterToFire = null;

    void Start()
    {
        orbiters = new List<CelestialBody>();
        parentTransform = GetComponentInParent<Transform>();
        shootPosition = GameObject.Find("ShootPoint").GetComponent<Transform>();
    }

    void Update()
    {
        UpdateSelectedOrbiter();
    }

    void FixedUpdate()
    {
        UpdateOrbiterToFirePosition();
        UpdateOrbiterPositions();
    }

    void UpdateOrbiterToFirePosition()
    {
        if (orbiterToFire != null)
        {
            orbiterToFire.SetOrbitingPosition(shootPosition.position);
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

    void UpdateSelectedOrbiter()
    {
        int oldSelection = selectedOrbiter;

        float scrollValue = Input.GetAxis("Mouse ScrollWheel");

        if (scrollValue > 0)
        {
            if (selectedOrbiter == orbiters.Count - 1)
            {
                selectedOrbiter = 0;
            } 
            else
            {
                selectedOrbiter++;
            }
        } 
        else if (scrollValue < 0)
        {
            if (selectedOrbiter == 0)
            {
                selectedOrbiter = orbiters.Count - 1;
            }
            else
            {
                selectedOrbiter--;
            }
        }
        
        if (orbiters.Count > 0)
        {
            orbiters[oldSelection].Deselect();
            orbiters[selectedOrbiter].Select();
        }
    }

    public void CollectOrbiters(List<CelestialBody> newOrbiters)
    {
        while (newOrbiters.Count > 0 && orbiters.Count < capacity)
        {
            CelestialBody orbiter = newOrbiters[0];
            orbiter.Collect();
            orbiters.Add(orbiter);
            newOrbiters.RemoveAt(0);
        }
    }

    public void PrepareFire()
    {
        if (orbiters.Count > 0 && orbiterToFire == null)
        {
            orbiterToFire = orbiters[selectedOrbiter];
            orbiters.RemoveAt(selectedOrbiter);

            orbiterToFire.PrepareFire();

            selectedOrbiter--;

            if (selectedOrbiter < 0)
            {
                selectedOrbiter = 0;
            }

            if (orbiters.Count > 0)
            {
                orbiters[selectedOrbiter].Select();
            }
        }
    }

    public void Fire(Vector2 velocity)
    {
        if (orbiterToFire != null)
        {
            orbiterToFire.Fire(velocity);
            orbiterToFire = null;
        }
    }
}
