using System;
using TMPro;
using UnityEngine;

public class TeleportUI : MonoBehaviour
{
    public GameObject txtHigherObject;
    public GameObject txtCurrentObject;
    public GameObject txtLowerObject;
    
    // is set by the AdjustableParabola script
    public float MaxVelocity = 25.0f;
    public float MinVelocity = 25.0f;
    public float SingleStepSize = 1.0f;

    private TextMeshProUGUI txtHigher;
    private TextMeshProUGUI txtCurrent;
    private TextMeshProUGUI txtLower;

    private void OnEnable()
    {
        txtHigher = txtHigherObject.GetComponent<TextMeshProUGUI>();
        txtCurrent = txtCurrentObject.GetComponent<TextMeshProUGUI>();
        txtLower = txtLowerObject.GetComponent<TextMeshProUGUI>();
        if (!txtHigher || !txtCurrent || !txtLower)
        {
            Debug.LogError("Could not find text component from children of TeleportUI prefab. ");
        }
    }

    public void UpdateVelocityLabel(float newVelocity)
    {
        float higherValue = Math.Clamp(newVelocity + SingleStepSize, MinVelocity, MaxVelocity);
        float currentValue = newVelocity;
        float lowerValue = Math.Clamp(newVelocity - SingleStepSize, MinVelocity, MaxVelocity);
        
        txtHigher.SetText(higherValue.ToString("F1"));
        txtCurrent.SetText(currentValue.ToString("F1"));
        txtLower.SetText(lowerValue.ToString("F1"));

        if (newVelocity > MaxVelocity-SingleStepSize && newVelocity <= MaxVelocity)
        {
            txtHigher.SetText("\u221E");
        }
        
        if (newVelocity > MaxVelocity)
        {
            txtHigher.SetText("MAX");
            txtCurrent.SetText("\u221E");
            txtLower.SetText(lowerValue.ToString("F1"));
        }

        if (newVelocity <= MinVelocity)
        {
            txtLower.SetText("MIN");
        }
     

    }
}
