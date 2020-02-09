using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Need
{
    public Material material;
    public int max;

    public float Percent(LinkedList<Material> gatheredMaterials)
    {
        return (from material in gatheredMaterials where material == this.material select material).Count() / (float)max;
    }
}

[Serializable]
public class Needs
{
    public Need[] neededMaterials;
    public LinkedList<Material> gatheredMaterials;

    public List<float> percentages{
        get {
            return (from need in neededMaterials select need.Percent(gatheredMaterials)).ToList();
        }
    }

    public override string ToString()
    {
        return string.Join(", ", from need in neededMaterials select $"{need.material}: {String.Format("{0:P2}", need.Percent(gatheredMaterials))}");
    }
}

public class NeederController : MonoBehaviour
{
    public Needs needs;

    public float timePerDecay = 10;
    public float incorrectMaterialTimePenalty = 3;
    public float decayTimer;

    public bool complete
    {
        get
        {
            return (from percent in needs.percentages where percent < 1f select percent).Count() == 0;
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
        needs = new Needs
        {
            neededMaterials = new Need[materialNumber],
        };

        var gatheredMaterials = new List<Material>();

        for (int i = 0; i < materialNumber; i++)
        {
            needs.neededMaterials[i] = new Need
            {
                material = availableMaterials[i],
                max = UnityEngine.Random.Range(minRequired, maxRequired)
            };

            var numberOfMaterial = (int)(needs.neededMaterials[i].max * startingPercent);

            for (int m = 0; m < numberOfMaterial; m++)
            {
                gatheredMaterials.Add(availableMaterials[i]);
            }
        }

        gatheredMaterials.Shuffle();
        needs.gatheredMaterials = new LinkedList<Material>(gatheredMaterials);
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

            if (needs.gatheredMaterials.Count == 0)
            {
                Die();
            }
            else
            {
                needs.gatheredMaterials.RemoveLast();
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

                foreach (var need in needs.neededMaterials)
                {
                    if (need.material == asteroid.material)
                    {
                        if (need.Percent(needs.gatheredMaterials) < 1f)
                        {
                            needs.gatheredMaterials.AddLast(asteroid.material);
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
