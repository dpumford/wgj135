using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarStatusController : MonoBehaviour
{
    TextMesh textMesh;
    StarController star;

    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        star = GetComponentInParent<StarController>();
    }

    // Update is called once per frame
    void Update()
    {
        textMesh.text = $"Rings: {star.ringCount}, Segments: {star.segmentCount} / {star.segmentLimit}, Decay: {star.timePerSegment - star.segmentTimer}s";
    }
}
