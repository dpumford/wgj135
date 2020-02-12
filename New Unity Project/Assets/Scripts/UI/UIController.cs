using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public RadialProgress CreateRadialProgress(Transform follow, Vector2 size, float startingPercent, float deathPercent)
    {
        if (follow == null)
        {
            Debug.LogError("Can't create at null transform!!");
        }

        RadialProgress r = Instantiate(radialProgressPrototype.gameObject, new Vector3(100,100,1), Quaternion.identity, myCanvas.transform).GetComponent<RadialProgress>();

        r.Init(myCamera, follow, size, startingPercent, deathPercent);

        return r;
    }
}
