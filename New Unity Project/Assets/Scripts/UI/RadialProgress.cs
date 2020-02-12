using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialProgress : MonoBehaviour
{
    public float percentFilled;
    public Image image;
    
    float deathPercent = -1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(float startingPercent, float dp)
    {
        percentFilled = startingPercent;
        deathPercent = dp;
    }

    // Update is called once per frame
    void Update()
    {
        if (deathPercent != -1f)
        {
            if (percentFilled == deathPercent)
            {
                Die();
            }
        }

        image.fillAmount = percentFilled;
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

    public void Die()
    {
        Destroy(gameObject);
    }
}
