using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour
{
    public TextMesh textField;

    public void SetText(string s)
    {
        textField.text = s;
    }

    public void SetColor(Color c)
    {
        textField.color = c;
    }

    public void SetAlignment(TextAnchor a, TextAlignment t)
    {
        textField.alignment = t;
        textField.anchor = a;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
