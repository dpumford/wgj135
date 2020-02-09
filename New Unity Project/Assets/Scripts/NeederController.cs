using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Need
{
    public Material material;
    public int count;
    public int max;

    public override string ToString()
    {
        return $"{material}: {count} / {max}";
    }
}

public class NeederController : MonoBehaviour
{
    public Need[] needs;

    public float timePerDecay = 10;
    public float incorrectMaterialTimePenalty = 3;
    public float decayTimer;

    public bool complete
    {
        get
        {
            return (from need in needs where need.count < need.max select need).Count() == 0;
        }
    }

    void Start()
    {
        decayTimer = 0;
        Reset(3, 2, 5, 0.5f);
    }

    public void Reset(int materialNumber, int minRequired, int maxRequired, float startingPercent)
    {
        var availableMaterials = Enum.GetValues(typeof(Material)).Cast<Material>().ToList();
        materialNumber = Math.Min(materialNumber, availableMaterials.Count);
        needs = new Need[materialNumber];

        for (int i = 0; i < materialNumber; i++)
        {
            needs[i] = new Need
            {
                material = availableMaterials[i],
                max = UnityEngine.Random.Range(minRequired, maxRequired)
            };

            needs[i].count = (int)(needs[i].max * startingPercent);
        }
    }

    void FixedUpdate()
    {
        UpdateSegments();
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleAsteroids(collision);
    }

    private void UpdateSegments()
    {
        if (complete)
        {
            return;
        }

        decayTimer += Time.deltaTime;

        if (decayTimer > timePerDecay)
        {
            decayTimer = 0;

            var availableNeeds = (from need in needs where need.count > 0 select need).ToList();

            if (availableNeeds.Count == 0 || (availableNeeds.Count == 1 && availableNeeds[0].count <= 1))
            {
                Die();
            }
            else
            {
                availableNeeds[UnityEngine.Random.Range(0, availableNeeds.Count)].count--;
            }
        }
    }

    private void HandleAsteroids(Collision2D collision)
    {
        var asteroid = collision.gameObject.GetComponent<AsteroidController>();

        if (asteroid != null)
        {
            if (!complete)
            {
                var fulfilled = false;

                foreach (var need in needs)
                {
                    if (need.material == asteroid.material)
                    {
                        if (need.count < need.max)
                        {
                            need.count++;
                        }

                        decayTimer = 0;
                        fulfilled = true;

                        break;
                    }
                }

                if (!fulfilled)
                {
                    decayTimer -= incorrectMaterialTimePenalty;
                }
            }

            Destroy(collision.gameObject);
        }
    }
}
