using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RadialProgress : MonoBehaviour
{
    public float percentFilled;
    public Image image;
    public Sprite filled;
    public Sprite ringed;
    
    float deathPercent = -1f;
    bool active;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(float startingPercent, float dp, Nullable<Color> recolor, bool a, bool isRing)
    {
        percentFilled = startingPercent;
        deathPercent = dp;
        active = a;
        if (recolor.HasValue)
        {
            image.color = recolor.Value;
        }

        if (!active)
        {
            SetInactive();
        }

        if (isRing)
        {
            image.sprite = ringed;
        } else
        {
            image.sprite = filled;
        }
    }

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = percentFilled;

        if (deathPercent != -1f)
        {
            if (percentFilled == deathPercent)
            {
                Die();
            }
        }
    }

    public void Recolor(Color c)
    {
        image.color = c;
    }

    public void SetActive()
    {
        active = true;
        var c = image.color;
        image.color = new Color(c.r, c.g, c.b, 1f);
    }

    public void SetInactive()
    {
        var c = image.color;
        image.color = new Color(c.r, c.g, c.b, c.a - 0.35f);
        active = false;
    }

    // convenience method for stuff that measures its duration in frames
    public void PercentOfFrames(int currentFrame, int maxFrame)
    {
        if (currentFrame < 0)
        {
            currentFrame = 0;
        }

        percentFilled = (float)currentFrame / (float)maxFrame;
    }

    // convenince method for stuff that measures its duration in real time
    public void PercentOfDuration(float currentTime, float duration)
    {
        if (currentTime > duration)
        {
            currentTime = duration;
        }

        percentFilled = currentTime / duration;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
