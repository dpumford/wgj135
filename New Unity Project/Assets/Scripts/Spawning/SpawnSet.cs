using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSet<T> : MonoBehaviour where T: SpawnPoint
{
    public T[] spawnPoints;

    public virtual void Setup()
    {
        spawnPoints = GetComponentsInChildren<T>();
    }
}
