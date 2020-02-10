using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSet : MonoBehaviour
{
    public List<Transform> spawnPoints;

    public virtual void Setup()
    {
        spawnPoints = new List<Transform>();

        foreach (var t in GetComponentsInChildren<SpawnPoint>())
        {
            Debug.Log("Setting up with :" + t.transform);
            spawnPoints.Add(t.transform);
        }
    }
}
