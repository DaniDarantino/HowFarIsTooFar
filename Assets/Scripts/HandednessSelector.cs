using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HandednessSelector : MonoBehaviour
{

    public RaySelector RightInteractorScript;
    public RaySelector LeftInteractorScript;

    public RawImage ImageRight;
    public RawImage ImageLeft;

    private void Start()
    {
        LeftInteractorScript.OnSelect += OnSelect;
        RightInteractorScript.OnSelect += OnSelect;;
    }

    private void OnSelect(RaycastHit hit)
    {
        if (hit.collider.gameObject == ImageRight.gameObject)
        {
            ImageRight.color = new Color(0.28f,0.80f,0.49f);
            ImageLeft.color = new Color(0,0,0);
            UserStudyManager.Instance.IsRightHanded = true;
        }
        if (hit.collider.gameObject == ImageLeft.gameObject)
        {
            ImageLeft.color = new Color(0.28f, 0.80f, 0.49f);
            ImageRight.color = new Color(0, 0, 0);
            UserStudyManager.Instance.IsRightHanded = false;
        }
    }
}
