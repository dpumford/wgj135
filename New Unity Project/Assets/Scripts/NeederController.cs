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

[Serializable]
public class NeederOptions
{
    public int materialNumber, minRequired, maxRequired;

    [Range(0, 1)]
    public float startingPercent;
}

public class NeederController : MonoBehaviour
{
    public Dictionary<Material, Need> needs = new Dictionary<Material, Need>();
    List<Material> gatherOrder = new List<Material>();
    List<RadialProgress> radialProgressTrackers = new List<RadialProgress>();

    public float timePerDecay = 10;
    public float incorrectMaterialTimePenalty = 3;
    public float decayTimer;

    public float xOffsetOfTrackerSet = 0f;
    public float yOffsetOfTrackerSet = 0f;
    public float trackerScale = .2f;
    public float xOffsetWithinLine = 1f;
    public float yOffsetBetweenLines = -1f;
    public int maxTrackersPerNeed = 5;

    public ExplosionController correctExplosionPrefab;
    public ExplosionController inCorrectExplosionPrefab;

    int maxNeededMaterials = 0;

    UIController uiControl;

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

    public void Die()
    {
        foreach(var tracker in radialProgressTrackers)
        {
            tracker.Die();
        }
        radialProgressTrackers.Clear();
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

    public void Reset(NeederOptions options, UIController ui)
    {
        uiControl = ui;
        var availableMaterials = Enum.GetValues(typeof(Material)).Cast<Material>().ToList();
        availableMaterials.Shuffle();
        // This isn't a material, duh
        availableMaterials.Remove(Material.Turret);

        options.materialNumber = Math.Min(options.materialNumber, availableMaterials.Count);

        needs = new Dictionary<Material, Need>();

        gatherOrder = new List<Material>();

        for (int i = 0; i < options.materialNumber; i++)
        {
            var need = new Need
            {
                name = availableMaterials[i],
                max = UnityEngine.Random.Range(options.minRequired, options.maxRequired),
                current = 0
            };

            maxNeededMaterials += need.max;

            Debug.Log("Need " + need.max + " " + availableMaterials[i]);

            need.current = (int)(need.max * options.startingPercent);

            for (int j = 0; j < need.current; j++)
            {
                gatherOrder.Add(availableMaterials[i]);
            }

            for (int j = 0; j < need.max; j++)
            {
                var materialColor = availableMaterials[i].MaterialColor();
                materialColor.a = 0.6f;

                var xOffsetStart = -(xOffsetWithinLine * (need.max - 1) / 2.0f);
                var yOffsetStart = (yOffsetBetweenLines * (i - 1));

                var xOffset = xOffsetOfTrackerSet + xOffsetStart + (j * xOffsetWithinLine);
                var yOffset = yOffsetOfTrackerSet + yOffsetStart;

                var offset = new Vector2(xOffset, yOffset);
                var scale = new Vector2(trackerScale, trackerScale);

                uiControl.CreateRadialProgress(transform, offset, scale, materialColor, 1f, -1f, false);
            }

            needs.Add(availableMaterials[i], need);
        }

        gatherOrder.Shuffle();

        for (int i = 0; i < gatherOrder.Count; i++)
        {
            AddTracker(gatherOrder[i]);
        }

        radialProgressTrackers.Last().SetActive();
    }

    void AddTracker(Material m)
    {
        var materialColor = m.MaterialColor();

        var xOffsetStart = -(xOffsetWithinLine * (needs[m].max - 1) / 2.0f);
        var yOffsetStart = (yOffsetBetweenLines * (needs.Keys.ToList().IndexOf(m) - 1));

        var xOffset = xOffsetOfTrackerSet + xOffsetStart + ((needs[m].current - 1) * xOffsetWithinLine);
        var yOffset = yOffsetOfTrackerSet + yOffsetStart;

        var offset = new Vector2(xOffset, yOffset);
        var scale = new Vector2(trackerScale, trackerScale);

        var progress = uiControl.CreateRadialProgress(transform, offset, scale, materialColor, 1f, 0f, false);
        radialProgressTrackers.Add(progress);
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

        if (radialProgressTrackers.Count > 0)
        {
            radialProgressTrackers.Last().PercentOfDuration(timePerDecay - decayTimer, timePerDecay);
        }

        if (decayTimer > timePerDecay)
        {
            decayTimer = 0;

            if (gatherOrder.Count > 0)
            {
                int lastGatherIndex = gatherOrder.Count - 1;
                needs[gatherOrder[lastGatherIndex]].Decay(1);
                gatherOrder.RemoveAt(lastGatherIndex);

                if (radialProgressTrackers.Count > 0)
                {
                    int lastTrackerIndex = radialProgressTrackers.Count - 1;
                    radialProgressTrackers.RemoveAt(lastTrackerIndex);

                    if (lastTrackerIndex > 0)
                    {
                        radialProgressTrackers[lastTrackerIndex - 1].SetActive();
                    }
                }
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
                if (needs.ContainsKey(asteroid.material) && asteroid.GivesMaterial())
                {
                    if (!needs[asteroid.material].Complete())
                    {
                        needs[asteroid.material].Gather(1);
                        gatherOrder.Add(asteroid.material);
                        
                        if (radialProgressTrackers.Count > 0)
                        {
                            radialProgressTrackers.Last().percentFilled = 1f;
                            radialProgressTrackers.Last().SetInactive();
                        }

                        AddTracker(asteroid.material);
                        radialProgressTrackers[radialProgressTrackers.Count - 1].SetActive();
                    }

                    decayTimer = 0;

                    Instantiate(correctExplosionPrefab, asteroid.transform.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(inCorrectExplosionPrefab, asteroid.transform.position, Quaternion.identity);
                    decayTimer += incorrectMaterialTimePenalty;
                }
            }
            else
            {
                Instantiate(inCorrectExplosionPrefab, asteroid.transform.position, Quaternion.identity);
            }

            asteroid.Die();
        }
    }
}
