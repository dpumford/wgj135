using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceFollower : MonoBehaviour
{
    RectTransform rectTransform;
    Camera parentCamera;
    RectTransform myCanvasRect;
    Transform follow;
    Vector2 offset;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        myCanvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }
    
    public void Init(Camera c, Transform f, Vector2 o)
    {
        parentCamera = c;
        follow = f;
        offset = o;
    }

    // Update is called once per frame
    void Update()
    {
        if (follow != null && parentCamera != null)
        {
            rectTransform.anchoredPosition = UIController.WorldPositionToCanvasAnchor(parentCamera, myCanvasRect, follow.position + (Vector3)offset);
        }
    }
}
