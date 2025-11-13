using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UXF;

public class SettingsTask : UserStudyTask
{
    
    public GameObject XROrigin;
    public GameObject Camera;
    
    public override void StartTask()
    {
        //position user starting position
        Vector3 cameraOffsetLocal = XROrigin.transform.InverseTransformPoint(Camera.transform.position);
        cameraOffsetLocal = new Vector3(cameraOffsetLocal.x, 0, cameraOffsetLocal.z);
        // y is not at zero since ground geometry is slightly above
        XROrigin.transform.position =  new Vector3(0,0.484f,-4) - cameraOffsetLocal;
        
        TextUI = Instantiate(TextUI);
        TextUI.SetActive(false);
        BasicTextUIScript = TextUI.GetComponentInChildren<BasicTextUI>();
        
        Session.instance.NextTrial.Begin();
    }
    
    [SerializeField] private InputActionReference IA_FaceButtonsRight;
    [SerializeField] private InputActionReference IA_FaceButtonsLeft;
    [SerializeField] private InputActionReference IA_RevertButton;

    public GameObject HandednessUI;
    public GameObject UserIDUI;
    public GameObject TextUI;
    private BasicTextUI BasicTextUIScript;
    
    private bool TaskReverted = false;
    
    public override void OnTrialBegin(Trial trial)
    {
        HandednessUI.SetActive(true);
        UserIDUI.SetActive(false);
    }

    public override void OnTrialEnd(Trial trial)
    {
  
        Session.instance.CurrentTrial.result["Handedness"] =
            UserStudyManager.Instance.IsRightHanded ? "Right" : "Left";
            
        Session.instance.CurrentTrial.result["UserID"] = UserStudyManager.Instance.IDPrefix+UserStudyManager.Instance.ParticipantID;
        
    }

    public override bool EndTask()
    {
        // in case we reverted the task, do not switch to the next scene when EndTask
        // is called after last trial finished
        if (TaskReverted)
        {
            TaskReverted = false;
            return false;
        }
        TaskReverted = false;
        
        Vector3 uiPos = Camera.transform.position + XROrigin.transform.forward * 3.0f - XROrigin.transform.up * 0.30f;
        TextUI.transform.position = uiPos;
        TextUI.SetActive(true);
        BasicTextUIScript.SetText("Loading Trial Scene...");
        
        return true;
    }

    private void Update()
    {
        // if we are not in trial anymore return (e.g. if we already ended the trial and the new map is loading,
        // do not listen to button inputs anymore)
        if (!Session.instance.InTrial)
        {
            return;
        }
        if (IA_FaceButtonsLeft.action.WasPerformedThisFrame() || IA_FaceButtonsRight.action.WasPerformedThisFrame())
        {
            if (UserIDUI.activeSelf)
            {
                Session.instance.EndCurrentTrial();
                
                HandednessUI.SetActive(false);
                UserIDUI.SetActive(false);
            }
            else
            {
                HandednessUI.SetActive(false);
                UserIDUI.SetActive(true);
            }
        }

        if (IA_RevertButton.action.WasPerformedThisFrame())
        {
            HandednessUI.SetActive(true);
            UserIDUI.SetActive(false);
        }
    }
}
