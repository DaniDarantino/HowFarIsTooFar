
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;
using UXF;

public abstract class TeleportBase : MonoBehaviour
{
    
    public GameObject TargetLocationVisualization;
    [Space(10)]
    public bool DisableVibration = true;
    public bool TwoStepMode = false;
    // special mode if the device simulator is used as input is binary (pressed or not pressed and cannot be half pressed etc.)
    protected bool XRDeviceSimulatorMode = false;
    
    // trigger deadzone's
    public const float LowerDeadzone = 0.1f;
    public const float UpperDeadzone = 0.95f;
    
    [Header("Bindings")]
    // button bindings
    [SerializeField] private InputActionReference IA_TeleportActivate;
    [SerializeField] private InputActionReference IA_TeleportCancel;
    
    protected Vector3 TargetLocation = Vector3.one;
    protected bool TargetLocationIsValid;
    protected GameObject XROrigin;
    protected GameObject Camera;
    
    // Abstract classes
    protected abstract void CleanupTeleportVisualization();
    protected abstract void ShowTeleportVisualization();
    protected abstract Vector3 GetTargetLocation();
    
    private bool WaitUntilFullyReleased = false;
    private float OldTriggerValue = 0.0f;
    protected void InitParameter()
    {
        // if we do not manually set a target location visualization, spawn an empty game object
        if(!TargetLocationVisualization)
        {
            TargetLocationVisualization = new GameObject();
        }
        // init parameters
        XROrigin = GameObject.Find("XR Origin (XR Rig)");
        if (!XROrigin) { Debug.LogError("Could not find XR Origin"); return; }
        Camera = GameObject.Find("Main Camera");
        if (!Camera) { Debug.LogError("Could not find Main Camera"); return; }
    }

    private void Start()
    {
        // if device simulator mode is used, this component exists somewhere in the scene
        XRDeviceSimulator deviceSimulator = FindFirstObjectByType<XRDeviceSimulator>();
        XRDeviceSimulatorMode = deviceSimulator != null && deviceSimulator.isActiveAndEnabled;
    }
    
    // Update is called once per frame
    protected void Update()
    {
        CheckButtonState();
    }
    
    protected virtual void CheckButtonState()
    {
        float currentTriggerValue = IA_TeleportActivate.action.ReadValue<float>();
        
        // if we let go of the button but not fully
        if (OldTriggerValue >= UpperDeadzone && currentTriggerValue < UpperDeadzone)
        {
            OnTriggerSlightlyReleased();
            if (XRDeviceSimulatorMode) // XR device simulator simulates only half released and not fully released, so we call it here already
            {
                OnTriggerFullyReleased();
            }
        }
        // if we slightly press the button
        else if(currentTriggerValue > LowerDeadzone && currentTriggerValue < UpperDeadzone)
        {
            WhileTriggerSlightPress();
        }
        // if we fully press the button
        else if (OldTriggerValue < UpperDeadzone && currentTriggerValue >= UpperDeadzone)
        {
            if (XRDeviceSimulatorMode) // XR device simulator cannot simulate half press, so we call it here
            {
                WhileTriggerSlightPress();
            }
            OnTriggerFullyPressed();
        }
        // while trigger is fully pressed
        else if (currentTriggerValue >= UpperDeadzone)
        {
            WhileTriggerFullyPressed();
        }
        // if we fully let go of the button
        else if (OldTriggerValue > LowerDeadzone && currentTriggerValue <= LowerDeadzone)
        {
            OnTriggerFullyReleased();
        }
        

        // check if we want to cancel current teleportation
        if (IA_TeleportCancel.action.triggered)
        {
            CancelTeleport();
        }

        OldTriggerValue = currentTriggerValue;
    }
    
    protected virtual void WhileTriggerSlightPress()
    {
        if (WaitUntilFullyReleased) return;
        TargetLocation = GetTargetLocation();
        TargetLocationVisualization.transform.position = TargetLocation;
    }
    
    protected virtual void OnTriggerFullyPressed()
    {
        if (WaitUntilFullyReleased) return;
        
        if (TwoStepMode)
        {
            // switch to bezier curve
            if (!DisableVibration)
            {
                // vibrate to indicate mode switch
            }
        }
    }
    
    private void WhileTriggerFullyPressed()
    {
        if (TwoStepMode)
        {
            // do something   
        }
        else
        {
            WhileTriggerSlightPress();
        }
    }

    protected virtual void OnTriggerSlightlyReleased()
    {
        if (WaitUntilFullyReleased) return; // is true if we cancelled the teleport
        
        if (!DisableVibration)
        {
            // vibrate
        }
        CleanupTeleportVisualization();
        TryTeleport();
        WaitUntilFullyReleased = true;
    }
    
    protected virtual void OnTriggerFullyReleased()
    {
        CleanupTeleportVisualization();
        WaitUntilFullyReleased = false;
    }
    
    protected virtual void CancelTeleport()
    {
        CleanupTeleportVisualization();
        WaitUntilFullyReleased = true;
    }

    protected virtual bool TryTeleport()
    {
        if (TargetLocationIsValid)
        {
            TeleportPlayerToTargetLocation();
            return true;
        }
        
        return false;
    }
    
    protected void TeleportPlayerToTargetLocation()
    {
        Vector3 cameraOffsetLocal = XROrigin.transform.InverseTransformPoint(Camera.transform.position);
        cameraOffsetLocal = new Vector3(cameraOffsetLocal.x, 0, cameraOffsetLocal.z);
        XROrigin.transform.position =  TargetLocation - cameraOffsetLocal;
    }
}
