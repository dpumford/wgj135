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
        textMesh.text = star.complete ? "Fulfilled!" : $"{star.needs}, Decay: {(int)(star.timePerDecay - star.decayTimer)}s";
    }
}
