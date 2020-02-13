using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIController : MonoBehaviour
{
    public RadialProgress radialProgressPrototype;
    Canvas myCanvas;
    Camera myCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        myCanvas = GetComponentInChildren<Canvas>();
        myCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public RadialProgress CreateRadialProgress(Transform follow, Vector2 offset, Vector2 scale, Nullable<Color> recolor, float startingPercent, float deathPercent, bool active, bool isRinged)
    {
        if (follow == null)
        {
            Debug.LogError("Can't create at null transform!!");
        }

        var rGameObject = Instantiate(radialProgressPrototype.gameObject, new Vector3(100,100,1), Quaternion.identity, myCanvas.transform).GetComponent<RadialProgress>();

        rGameObject.GetComponent<WorldSpaceFollower>().Init(myCamera, follow, offset);
        rGameObject.GetComponent<UIScaler>().Init(scale);
        
        var r = rGameObject.GetComponent<RadialProgress>();
        r.Init(startingPercent, deathPercent, recolor, active, isRinged);

        return r;
    }

    public static Vector2 WorldPositionToCanvasAnchor(Camera c, RectTransform canvasRect, Vector2 position)
    {
        Vector2 viewportPosition = c.WorldToViewportPoint(position);
        Vector2 screenPosition = new Vector2(
        ((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
        ((viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

        return screenPosition;
    }
}
