using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StarStatusController : MonoBehaviour
{
    TextMesh textMesh;
    NeederController star;

    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        star = GetComponentInParent<NeederController>();
    }

    // Update is called once per frame
    void Update()
    {
        textMesh.text = $"{string.Join(", ", from need in star.needs select need.ToString())}, Decay: {star.timePerDecay - star.decayTimer}s";
    }
}
