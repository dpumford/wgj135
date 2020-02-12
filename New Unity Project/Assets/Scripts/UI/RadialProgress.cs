using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialProgress : MonoBehaviour
{

    public float percentFilled;
    public Image image;
    public RectTransform rectTransform;
    Camera parentCamera;
    RectTransform myCanvasRect;

    Transform follow;
    float deathPercent = -1f;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        myCanvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    public void Init(Camera c, Transform f, Vector2 size, float startingPercent, float dp)
    {
        parentCamera = c;

        follow = f;
        percentFilled = startingPercent;
        deathPercent = dp;

        var r = rectTransform.rect;

        rectTransform.localScale = size;
    }

    // Update is called once per frame
    void Update()
    {
        if (follow != null)
        {
            WorldPositionToCanvasAnchor(follow.position);
        }

        if (deathPercent != -1f)
        {
            if (percentFilled == deathPercent)
            {
                Die();
            }
        }

        image.fillAmount = percentFilled;
    }

    public void WorldPositionToCanvasAnchor(Vector2 position)
    {
        if (parentCamera != null)
        {
            Vector2 viewportPosition = parentCamera.WorldToViewportPoint(position);
            Vector2 screenPosition = new Vector2(
            ((viewportPosition.x * myCanvasRect.sizeDelta.x) - (myCanvasRect.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * myCanvasRect.sizeDelta.y) - (myCanvasRect.sizeDelta.y * 0.5f)));

            rectTransform.anchoredPosition = screenPosition;
        }
    }

    // convenience method for stuff that measures its duration in frames
    public void PercentOfFrames(int currentFrame, int maxFrame)
    {
        if (currentFrame < 0)
        {
            currentFrame = 0;
        }

        percentFilled = (float)currentFrame / (float)maxFrame;
        Debug.Log("Changing percent amount! " + percentFilled);
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
