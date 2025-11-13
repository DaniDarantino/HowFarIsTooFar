using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AdjustableParabola : Parabola
{
    // button bindings
    public InputActionReference IA_AdjustVelocity;
    public InputActionReference IA_ResetVelocity;
    
    public TeleportUI TeleportUI;
    public float DefaultVelocity = 10.0f;
    public float MaxVelocity = 25.0f;
    public float MinVelocity = 1.0f;
    
    private float StickDeadzone = 0.1f;
    private Vector2 OldStickValue;
    
    // timings for velocity selection
    public float SingleStepSize = 1.0f;
    private float VelocityUpdateRate0 = 0.005f;
    private float VelocityUpdateRate1 = 0.01f;
    private float VelocityUpdateRate2 = 0.02f;
    private float VelocityUpdateRate3 = 0.03f;

    private float VelocityModeSlowThreshold = 1.5f;
    private float VelocityModeMediumThreshold = 3.0f;
    private float VelocityModeFastThreshold = 4.5f;
    
    private float StickHeldTime;
    private float ModeSwitchTime;
    
    // Start is called before the first frame update
    protected new void OnEnable()
    {
        base.OnEnable();
        OldStickValue=Vector2.zero;
    }

    private void Start()
    {
        TeleportUI.UpdateVelocityLabel(EmissionVelocity);
        TeleportUI.MaxVelocity = MaxVelocity;
        TeleportUI.MinVelocity = MinVelocity;
        TeleportUI.SingleStepSize = SingleStepSize;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        Vector2 currentStickValue = IA_AdjustVelocity.action.ReadValue<Vector2>();
        if (currentStickValue.magnitude > StickDeadzone && OldStickValue.magnitude < StickDeadzone)
        {
            OnStickStarted();
        }
        else if (currentStickValue.magnitude > StickDeadzone)
        {
            WhileStickOngoing();
        }

        else if (currentStickValue.magnitude < StickDeadzone && OldStickValue.magnitude > StickDeadzone)
        {
            OnStickFinished();
        }

        OldStickValue = currentStickValue;
        
        
        // reset velocity if stick is pressed
        if(IA_ResetVelocity.action.WasPressedThisFrame())
        {
            EmissionVelocity = DefaultVelocity;
            if (!UseGravity)
            {
                SetUseGravity(true);
            }
            TeleportUI.UpdateVelocityLabel(EmissionVelocity);
            
            
            /*// example uxf framework logic: 
            if (Session.instance.currentTrialNum == 0)
            {
                Debug.Log("Start First Trial");
                Session.instance.FirstTrial.Begin();
            }
            else
            {
                Session.instance.EndCurrentTrial();
                Debug.Log("End Current Trial");
                Session.instance.BeginNextTrial();
            }*/
        }
        
       
        
    }
    private void OnStickStarted()
    {
        StickHeldTime = 0.0f;
    }
    private void WhileStickOngoing()
    {
        StickHeldTime += Time.deltaTime;
        if (StickHeldTime > VelocityModeFastThreshold)
        {
            UpdateVelocity(VelocityUpdateRate3);
        } 
        else if (StickHeldTime > VelocityModeMediumThreshold)
        {
            UpdateVelocity(VelocityUpdateRate2);
        }
        else if (StickHeldTime > VelocityModeSlowThreshold)
        {
            UpdateVelocity(VelocityUpdateRate1);
        }
        else if (StickHeldTime > 0.5f)
        {
            UpdateVelocity(VelocityUpdateRate0);
        }
    }
    private void OnStickFinished()
    {
        if (StickHeldTime < 0.5f)
        {
            UpdateVelocity(SingleStepSize);
        }
        StickHeldTime = 0.0f;
    }

    private void UpdateVelocity(float updateRate)
    {
        if (OldStickValue.y > StickDeadzone)
        {
            // clamp velocity value, upper limit + 0.1 because we switch to infinity if value > maxVelocity

            EmissionVelocity += updateRate;
            // in case this update would go over maxVelocity set labels correctly and switch to noGravity
            if (EmissionVelocity > MaxVelocity)
            {
                // set use gravity to false if not done yet
                if (UseGravity)
                {
                    SetUseGravity(false);
                }
                
                // call function with value of maxVelocity + singleStepSize so labels are set correctly
                // since lower value label is newVelocity singleStepSize
                TeleportUI.UpdateVelocityLabel(MaxVelocity+SingleStepSize);
            }
            else
            {
                TeleportUI.UpdateVelocityLabel(EmissionVelocity);
            }
            EmissionVelocity = Math.Clamp(EmissionVelocity, MinVelocity, MaxVelocity);

        }
        if (OldStickValue.y < -StickDeadzone)
        {
            EmissionVelocity -= updateRate;
            
            // when we drop below maxVelocity for the first time, switch back to using gravity
            // and set value to maxVelocity, otherwise, when at max value and stick down is pressed we immediately
            // go below maxVelocity so we can never be AT maxVelocity
            if(!UseGravity && EmissionVelocity <= MaxVelocity)
            {
                SetUseGravity(true);
                EmissionVelocity = MaxVelocity;
            }
            
            // clamp velocity value, upper limit + 0.1 because we switch to infinity if value > maxVelocity 
            EmissionVelocity = Math.Clamp(EmissionVelocity, MinVelocity, MaxVelocity);
            TeleportUI.UpdateVelocityLabel(EmissionVelocity);
        }
        
    }
}