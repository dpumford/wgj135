using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeText : MonoBehaviour
{
    public string[] messages;
    int currentMessage = 0;

    public float timePerCharacter = .1f;
    float messageTimeBeforeFade = 0;
    float timeInMessage = 0f;

    public float fadeTime = 2f;

    float fadeEpsilon = 0.01f;

    public bool loop = false;

    public float timeBetweenMessages = 0.5f;
    float timeWaiting = 0f;

    TextState state = TextState.Starting;

    Text t;

    enum TextState
    {
        Starting,
        Displaying,
        Fading,
        Faded
    }

    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentMessage >= messages.Length)
        {
            if (loop)
            {
                currentMessage = 0;
            }
            else
            {
                enabled = false;
                return;
            }
        }

        if (state == TextState.Starting)
        {
            t.text = messages[currentMessage];
            t.color = Color.Lerp(t.color, Color.white, .01f);

            if (t.color.a >= 1f - fadeEpsilon)
            {
                messageTimeBeforeFade = messages[currentMessage].Length * timePerCharacter;
                timeInMessage = 0f;
                state = TextState.Displaying;
            }   
        } 
        else if (state == TextState.Displaying)
        {
            timeInMessage += Time.deltaTime;
            
            if (timeInMessage >= messageTimeBeforeFade)
            {
                state = TextState.Fading;
            }
        }
        else if (state == TextState.Fading)
        {
            t.color = Color.Lerp(t.color, Color.clear, .01f);

            if (t.color.a <= 0f + fadeEpsilon)
            {
                timeWaiting = 0f;
                state = TextState.Faded;
            }
        }
        else // Faded
        {
            timeWaiting += Time.deltaTime;

            if (timeWaiting >= timeBetweenMessages)
            {
                currentMessage++;
                state = TextState.Starting;
            }
        }
    }
}
