using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    public Transform shootPosition;

    private int selectedOrbiter;
    private CelestialBody orbiterToFire = null;

    private Orbit orbiter;
    private AudioSource audioSource;

    private List<CelestialBody> orbiters
    {
        get
        {
            return orbiter.orbiters;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        orbiter = GetComponent<Orbit>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSelectedOrbiter();
    }

    void FixedUpdate()
    {
        UpdateOrbiterToFirePosition();
    }

    void UpdateSelectedOrbiter()
    {
        if (orbiterToFire != null)
        {
            return;
        }

        int oldSelection = selectedOrbiter;

        float scrollValue = Input.GetAxis("Mouse ScrollWheel");

        if (orbiters.Count == 0)
        {
            selectedOrbiter = 0;
        }
        else if (scrollValue > 0)
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

    void UpdateOrbiterToFirePosition()
    {
        if (orbiterToFire != null)
        {
            orbiterToFire.SetOrbitingPosition(shootPosition.position);
        }
    }

    public void PrepareFire()
    {
        if (orbiters.Count > 0 && orbiterToFire == null)
        {
            orbiterToFire = orbiters[selectedOrbiter];
            orbiters.RemoveAt(selectedOrbiter);

            orbiterToFire.PrepareFire();
        }
    }

    public void SetSelectedAsTurretPackage()
    {
        if (orbiters.Count > 0 && orbiterToFire == null)
        {
            (orbiters[selectedOrbiter] as AsteroidController).SetTurretAsteroid();
        }
    }

    private void resetSelectedOrbiter()
    {
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

    public void Fire(Vector2 velocity)
    {
        if (orbiterToFire != null)
        {
            orbiterToFire.Fire(velocity);
            orbiterToFire = null;

            resetSelectedOrbiter();

            audioSource.Play();
        }
    }

    public CelestialBody ReleaseSelectedBody()
    {
        if (orbiterToFire != null || orbiters.Count == 0)
        {
            return null;
        }

        var selected = orbiters[selectedOrbiter];
        orbiters.RemoveAt(selectedOrbiter);

        resetSelectedOrbiter();

        return selected;
    }
}
