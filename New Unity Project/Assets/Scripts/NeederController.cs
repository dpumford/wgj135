using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Need
{
    public Material name;
    public int max;
    public int current;

    public void Gather(int amount)
    {
        current = Mathf.Min(max, current + amount);
    }

    public void Decay(int amount)
    {
        current = Mathf.Max(0, current - amount);
    }

    public bool Complete()
    {
        return current == max;
    }

    public float Percent()
    {
        return (float)current / (float)max;
    }
}

public class NeederController : MonoBehaviour
{
    public Dictionary<Material, Need> needs = new Dictionary<Material, Need>();
    List<Material> gatherOrder = new List<Material>();

    public float timePerDecay = 10;
    public float incorrectMaterialTimePenalty = 3;
    public float decayTimer;

    public bool IsComplete()
    {
        foreach (var need in needs.Values) {
            if (!need.Complete())
            {
                return false;
            }
        }

        return true;
    }

    public bool IsDead()
    {
        return GatheredCount() == 0;
    }

    public int GatheredCount()
    {
        return gatherOrder.Count;
    }

    void Start()
    {
        decayTimer = 0;
    }

    public void Reset(int materialNumber, int minRequired, int maxRequired, float startingPercent)
    {
        var availableMaterials = Enum.GetValues(typeof(Material)).Cast<Material>().ToList();
        availableMaterials.Shuffle();

        materialNumber = Math.Min(materialNumber, availableMaterials.Count);

        needs = new Dictionary<Material, Need>();

        gatherOrder = new List<Material>();

        for (int i = 0; i < materialNumber; i++)
        {
            var need = new Need
            {
                name = availableMaterials[i],
                max = UnityEngine.Random.Range(minRequired, maxRequired),
                current = 0
            };

            need.current = (int)(need.max * startingPercent);

            for (int j = 0; j < need.current; j++)
            {
                gatherOrder.Add(availableMaterials[i]);
            }

            needs.Add(availableMaterials[i], need);
        }

        gatherOrder.Shuffle();
    }

    void FixedUpdate()
    {
        UpdateSegments();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleAsteroids(collision);
    }

    private void UpdateSegments()
    {
        if (IsComplete())
        {
            return;
        }

        decayTimer += Time.deltaTime;

        if (decayTimer > timePerDecay)
        {
            decayTimer = 0;

            if (gatherOrder.Count > 0)
            {
                int lastGatherIndex = gatherOrder.Count - 1;
                needs[gatherOrder[lastGatherIndex]].Decay(1);
                gatherOrder.RemoveAt(lastGatherIndex);
            }
        }
    }

    private void HandleAsteroids(Collision2D collision)
    {
        var asteroid = collision.gameObject.GetComponent<AsteroidController>();

        if (asteroid != null)
        {
            if (!IsComplete())
            {
                var fulfilled = false;

                if (needs.ContainsKey(asteroid.material))
                {
                    needs[asteroid.material].Gather(1);
                    gatherOrder.Add(asteroid.material);

                    decayTimer = 0;
                    fulfilled = true;
                }

                if (!fulfilled)
                {
                    decayTimer -= incorrectMaterialTimePenalty;
                }
            }

            asteroid.Die();
        }
    }
}
