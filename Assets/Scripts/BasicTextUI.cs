using System;
using System.Net.Mime;
using TMPro;
using UnityEngine;

using UnityEngine.UIElements;

public class BasicTextUI : MonoBehaviour
{

    public String Text = "Text";
    public GameObject ImagePanel;

    private TextMeshProUGUI TextMesh;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        TextMesh = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        TextMesh.text = Text;
    }

    public void SetText(String text)
    {
        Text = text;
        TextMesh.text = text;
    }
}
