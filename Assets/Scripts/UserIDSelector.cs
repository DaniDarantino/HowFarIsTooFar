using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserIDSelector : MonoBehaviour
{
    public RaySelector RightInteractorScript;
    public RaySelector LeftInteractorScript;

    public GameObject UpArrow;
    public GameObject DownArrow;
    public GameObject txtID;

    private TextMeshProUGUI TextMesh;

    private void Start()
    {
        LeftInteractorScript.OnSelect += OnSelect;
        RightInteractorScript.OnSelect += OnSelect;
        TextMesh = txtID.GetComponent<TextMeshProUGUI>();
        TextMesh.text = UserStudyManager.Instance.IDPrefix+UserStudyManager.Instance.ParticipantID.ToString("00");
    }

    private void OnEnable()
    {
        TextMesh = txtID.GetComponent<TextMeshProUGUI>();
        TextMesh.text = UserStudyManager.Instance.IDPrefix+UserStudyManager.Instance.ParticipantID.ToString("00");
    }

    private void OnSelect(RaycastHit hit)
    {
        if (hit.collider.gameObject == UpArrow.gameObject)
        {
            UserStudyManager.Instance.ParticipantID += 1;
            UserStudyManager.Instance.ParticipantID = Mathf.Clamp(UserStudyManager.Instance.ParticipantID, 0, 50);
            TextMesh.text = UserStudyManager.Instance.IDPrefix+UserStudyManager.Instance.ParticipantID.ToString("00");
        }
        if (hit.collider.gameObject == DownArrow.gameObject)
        {
            UserStudyManager.Instance.ParticipantID -= 1;
            UserStudyManager.Instance.ParticipantID = Mathf.Clamp(UserStudyManager.Instance.ParticipantID, 0, 50);
            TextMesh.text = UserStudyManager.Instance.IDPrefix+UserStudyManager.Instance.ParticipantID.ToString("00");
        }
    }
}
