using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableController : MonoBehaviour
{
    public PowerUp Current
    {
        get;
        private set;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (Current != null)
        {
            Current.UpdateLifetimeTimer(Time.deltaTime);

            if (!Current.ShouldApply)
            {
                Current = null;
            }
        }
    }

    public void ConsumeAsteroid(AsteroidController asteroid)
    {
        Current = asteroid.SelectedPowerUp;

        if (asteroid != null)
        {
            asteroid.Die();
        }
    }
}
