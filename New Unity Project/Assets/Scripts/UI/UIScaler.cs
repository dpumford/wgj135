using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScaler : MonoBehaviour
{
    public RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Init(Vector2 scale)
    {
        rectTransform.localScale = scale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
