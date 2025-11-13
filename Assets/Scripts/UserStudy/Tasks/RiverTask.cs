using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UXF;

public class RiverTask : UserStudyTask
{
    // Player References
    public GameObject RightHand;
    public GameObject LeftHand;
    public GameObject XROrigin;
    public GameObject Camera;

    // Input Actions
    public InputActionReference IA_NextTrial;
    public InputActionReference IA_NextTrialOverride;
    public InputActionReference IA_PreviousTrial;
    
    // Sounds
    public AudioClip SplashSound;
    public AudioClip WrongAnswerSound;
    public AudioClip VictorySound;
    public AudioClip ConfirmSound;
    public AudioClip TeleportSound;

    // UI
    public GameObject ScoreUI;
    public GameObject TextUI;
    public GameObject ArrowUI;

    public bool UseDuck = true;
    
    public GameObject SelectionTarget;
    
    private StudyParabola TeleportScript;
    private RaySelector RaySelectorScript;

    private int NumberOfJumpsPerTrial = 15;

    private GameObject HittedPlatform;
    
    private PlatformSpawner PlatformSpawner;
    private EPlatformSize CurrentPlatformSize = EPlatformSize.Small;
    private GameObject CurrentSelectionTarget;
    private AudioSource AudioSourceComponent;

    private TextMeshProUGUI TxtScore;
    private TextMeshProUGUI TxtProgress;
    private float ScoreOfLastJump = 0.0f;
    private bool WaitForUserInput = false;
    private int RemainingJumps;

    private List<EPlatformSize> PlatformSizes = new List<EPlatformSize>();
    private BasicTextUI BasicTextUIScript;
    
    private int InfoTextIterator = 0;
    private bool ShowInfoTexts = false;

    private float FalseJumpPenalty = 15.0f;
    private bool WasLastTeleportValid = true;
    private bool InitialPlatformsSpawned = false;
    private float ControllerHoldTimeThreshold = 1.0f;
    private float ControllerHoldTimeCurrent = 0;
    private TextMeshProUGUI ArrowText;
    
    private void OnEnable()
    {
        // init Platform Spawner
        PlatformSpawner = GetComponent<PlatformSpawner>();
        if (!PlatformSpawner) { Debug.LogError("Could not find PlatformSpawner in UserStudyLogic"); return; }

        TextUI = Instantiate(TextUI);
        TextUI.SetActive(false);
        BasicTextUIScript = TextUI.GetComponentInChildren<BasicTextUI>();
        
        AudioSourceComponent = GetComponent<AudioSource>();
        if(!AudioSourceComponent) { Debug.LogError("Could not find AudioSourceComponent in UserStudyLogic"); return; }
    }
    
    public override void StartTask()
    {
        
        // move player back to origin
        Vector3 cameraOffsetLocal = XROrigin.transform.InverseTransformPoint(Camera.transform.position);
        cameraOffsetLocal = new Vector3(cameraOffsetLocal.x, 0, cameraOffsetLocal.z);
        XROrigin.transform.position =  Vector3.zero - cameraOffsetLocal;
        
        // settings file is not available in standalone
        /*int numMainTrials = session.settings.GetInt("n_main_trials",9);
        int minVelocity = session.settings.GetInt("minVelocity", 6);
        int velocityIncrement = session.settings.GetInt("velocityIncrement", 2);
        NumberOfJumpsPerTrial = session.settings.GetInt("n_jumps_per_trial",15);
        PlatformSpawner.SmallSize = session.settings.GetFloat("small_platform_diameter",0.4f);
        PlatformSpawner.MediumSize = session.settings.GetFloat("medium_platform_diameter",0.8f);
        PlatformSpawner.LargeSize = session.settings.GetFloat("large_platform_diameter",1.2f);
        PlatformSpawner.PlatformGap = session.settings.GetFloat("platform_gap",0.5f);
        PlatformSpawner.MaxLateralOffset = session.settings.GetFloat("platform_lateral_offset",1.0f);*/
        
        NumberOfJumpsPerTrial = 18;
        PlatformSpawner.SmallDiameter = 0.4f;
        PlatformSpawner.MediumDiameter = 0.8f;
        PlatformSpawner.LargeDiameter = 1.2f;
        PlatformSpawner.PlatformGap = 0.5f;
        PlatformSpawner.MaxLateralOffset = 1.0f;
        
        TeleportScript = RightHand.GetComponent<StudyParabola>();
        
        // Switch hand
        if (UserStudyManager.Instance.IsRightHanded)
        {
            TeleportScript = RightHand.GetComponent<StudyParabola>();
            RaySelectorScript = RightHand.GetComponent<RaySelector>();
            LeftHand.SetActive(false);
        }
        else
        {
            TeleportScript = LeftHand.GetComponent<StudyParabola>();
            RaySelectorScript = LeftHand.GetComponent<RaySelector>();
            RightHand.SetActive(false);
        }
        // spawn UI at controller
        ScoreUI = Instantiate(ScoreUI);
        ScoreUI.transform.SetParent(TeleportScript.transform);
        ScoreUI.transform.position = TeleportScript.gameObject.transform.position;
        ScoreUI.transform.rotation = TeleportScript.gameObject.transform.rotation;

        ArrowUI = Instantiate(ArrowUI);
        ArrowText = ArrowUI.GetComponentInChildren<TextMeshProUGUI>();
        if(!ArrowText) { Debug.LogError("ArrowUI does not have a text mesh pro component"); return; }
        ArrowUI.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        ArrowUI.gameObject.SetActive(false);
        
        TeleportScript.OnTryTeleport += OnTryTeleport;
        RaySelectorScript.OnSelect += OnResumeTeleport;
        
        //get reference to score text mesh
        foreach (TextMeshProUGUI txt in ScoreUI.GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (txt.name == "txtScore")
            {
                TxtScore = txt;
            }

            if (txt.name == "txtProgress")
            {
                TxtProgress = txt;
            }
        }
        if(!TxtScore) Debug.LogError("Could not find txtScore at ScoreUI in UserStudyLogic");
        TxtScore.SetText("0.00");
        UserStudyManager.Instance.FinalScore = 0.0f;
        
        if(!TxtProgress) Debug.LogError("Could not find txtProgress at ScoreUI in UserStudyLogic");
        TxtProgress.SetText("0/"+NumberOfJumpsPerTrial);
        
        // must stay at the end
        Session.instance.NextTrial.Begin();
        
        // Add and activate tracker script as they are disabled by default
        Session.instance.trackedObjects.Add(TeleportScript.TeleportTracker);
    }
    
    public override void OnTrialBegin(Trial trial)
    {
        if (Session.instance.CurrentBlock.settings.GetBool("Practice"))
        {
            NumberOfJumpsPerTrial = 6; // during practice task only do 6 jumps
        }
        
        CreatePlatformSizeArray();
        InitialPlatformsSpawned = false;
        
        RemainingJumps = NumberOfJumpsPerTrial;
        WaitForUserInput = false;
        // move player back to origin
        Vector3 cameraOffsetLocal = XROrigin.transform.InverseTransformPoint(Camera.transform.position);
        cameraOffsetLocal = new Vector3(cameraOffsetLocal.x, 0, cameraOffsetLocal.z);
        XROrigin.transform.position =  Vector3.zero - cameraOffsetLocal;
        
        float velocity = Session.instance.CurrentTrial.settings.GetFloat("Velocity");
        
        TeleportScript.EmissionVelocity = velocity;
        Session.instance.CurrentTrial.result["Velocity"] = velocity;
        
        TxtScore.SetText("0.00");
        TxtProgress.SetText("0/"+NumberOfJumpsPerTrial);
        
        if (Session.instance.CurrentBlock.settings.GetBool("Practice"))
        {
            Session.instance.CurrentTrial.result["Practice"] = true;
            
            TeleportScript.TeleportTracker.StopRecording();
            
            // disable teleport script for now and spawn text panels to explain the task
            TeleportScript.enabled = false;
            ShowInfoTexts = true;
            ShowNextInfoText();
        }
        else
        {
            Session.instance.CurrentTrial.result["Practice"] = false;
            
            // if we are not in practice mode start tracker
            TeleportScript.TeleportTracker.StartRecording();
            SwitchToSelectionPart();
        }
    }
    
    private void CreatePlatformSizeArray()
    {
        for (int i = 0; i < NumberOfJumpsPerTrial / 3; i++)
        {
            PlatformSizes.Add(EPlatformSize.Small);
            PlatformSizes.Add(EPlatformSize.Medium);
            PlatformSizes.Add(EPlatformSize.Large);
        }
        PlatformSizes.Shuffle();
    }

    private void OnTryTeleport(bool validTeleport,GameObject hittedPlatform, float JumpDistance, Vector3 targetLocation)
    {
        HittedPlatform = hittedPlatform? hittedPlatform: null;
        TeleportScript.TeleportTracker.PlatformSize = CurrentPlatformSize;
        WasLastTeleportValid = validTeleport;
        if (validTeleport)
        {
            ScoreOfLastJump = JumpDistance;
            UserStudyManager.Instance.FinalScore += ScoreOfLastJump;
            RemainingJumps--;
            TxtProgress.SetText((NumberOfJumpsPerTrial-RemainingJumps)+"/"+NumberOfJumpsPerTrial);
            AudioSourceComponent.PlayOneShot(TeleportSound,0.7f);
        }
        else
        {
            AudioSourceComponent.PlayOneShot(SplashSound);
            AudioSourceComponent.PlayOneShot(WrongAnswerSound,0.6f);
            ScoreOfLastJump = -FalseJumpPenalty;
            UserStudyManager.Instance.FinalScore -=  ScoreOfLastJump;
            
            TeleportScript.TeleportTracker.DistanceToTarget = GetDistanceToNearestValidHitLocation(targetLocation);
        }
        
        // store tracker data (one row per jump/selection)
        if (Session.instance.InTrial && !Session.instance.CurrentBlock.settings.GetBool("Practice"))
        {
            // record data of last jump
            TeleportScript.TeleportTracker.RecordRow();
        }
        UpdateScore();
        if (RemainingJumps <= 0)
        {
            AllJumpsCompleted();
        }
        else
        {
            SwitchToSelectionPart();
        }
    }

    private float GetDistanceToNearestValidHitLocation(Vector3 targetLocation)
    {
        // init distance to large number, update distance if we find lower number
        float dist = 10000;
        Vector2 targetPosition2D = new Vector2(targetLocation.x, targetLocation.z);
        foreach (var p in PlatformSpawner.PlatformStack)
        {
            Vector2 platformPosition2D = new Vector2(p.transform.position.x, p.transform.position.z);
            float newDist = (platformPosition2D - targetPosition2D).magnitude;
            if (newDist < dist)
            {
                dist = newDist;
            }
        }

        // To get the distance to the closest valid hit location, remove the platform radius
        dist -= PlatformSpawner.GetPlatformSizeAsFloat(CurrentPlatformSize) / 2;
        
        return dist;
    }

    private void UpdateScore()
    {
        // JumpDistance will be 0 in case of a missed jump
        TxtScore.SetText(ScoreOfLastJump.ToString("0.00"));
    }

    private void AllJumpsCompleted()
    {
        // Spawn UI to let user know to press a button to continue with the next trial
        Vector3 uiPos = Camera.transform.position + XROrigin.transform.forward * 3.0f - XROrigin.transform.up * 0.30f;
        
        if (Session.instance.CurrentBlock.settings.GetBool("Practice"))
        {

            BasicTextUIScript.SetText("This is the end of the practice trial.\n"+
                                        "If you don't have any further questions, press a face button to start with the first of " +
                                      Session.instance.NextTrial.block.trials.Count+" trials.");
        }
        else
        {
            BasicTextUIScript.SetText("Task " + Session.instance.CurrentTrial.numberInBlock +
                                      " of " + Session.instance.CurrentBlock.trials.Count +
                                      " completed.\n" +
                                      "Your current score is: " + UserStudyManager.Instance.FinalScore.ToString("F1")+".\n"+
                                      "Take a break or continue with the next task by pressing a face button.");
        }
        
        TextUI.SetActive(true);
        TextUI.transform.position = uiPos;
        WaitForUserInput = true;

        AudioSourceComponent.PlayOneShot(VictorySound,0.5f);
        // deactivate teleport
        TeleportScript.enabled = false;
    }

    private void SwitchToSelectionPart()
    {
        // show arrow and info text to remind user to select target
        ArrowText.SetText("Point to the ground!");
        ArrowUI.gameObject.SetActive(true);
        ArrowUI.gameObject.transform.position = Camera.transform.position + XROrigin.transform.forward * 2.0f - XROrigin.transform.up * 0.30f;

        if (UseDuck)
        {
            // Spawn selection target 20 cm to the right/left of the user, depending on the handedness
            Vector3 SelectionTargetPosition = new Vector3(Camera.transform.position.x, XROrigin.transform.position.y,
                Camera.transform.position.z);
            if (UserStudyManager.Instance.IsRightHanded)
            {
                SelectionTargetPosition.x += 0.2f;
            }
            else
            {
                SelectionTargetPosition.x -= 0.2f;
            }
        
            CurrentSelectionTarget = Instantiate(SelectionTarget, SelectionTargetPosition, Quaternion.identity);
            // disable Teleport, Enable ray selector, for distraction task
            RaySelectorScript.enabled = true;
        } 

        TeleportScript.enabled = false;
    }

    // called after ray selection task
    public void OnResumeTeleport(RaycastHit hit)
    {
        if (Session.instance.InTrial)
        {
            // set controller parameters for selection task
            TeleportScript.TeleportTracker.SelectionHorizontalControllerAngle =
                TeleportScript.GetHorizontalControllerAngle();
            TeleportScript.TeleportTracker.SelectionControllerHeightAboveGround =
                TeleportScript.gameObject.transform.localPosition.y;
        }
        
        // ray cast will be done on own layer, therefore, a valid hit should already be enough
        // just to be on the save side, we also compare the name
        ArrowUI.SetActive(false);
        TeleportScript.enabled = true;
        
        if (UseDuck)
        {
            RaySelectorScript.enabled = false;
            var target = CurrentSelectionTarget.GetComponent<SelectionTarget>();
            if (target)
            {
                target.PlaySound();
            }
            else
            {
                Debug.Log("Cannot find component Selection target at current selection target");
            }
            // destroy object delayed so sound can play
            Destroy(CurrentSelectionTarget,2.0f);
            // make it immediately invisible
            CurrentSelectionTarget.transform.Find("Duck").gameObject.SetActive(false);
        }
        else
        {
            AudioSourceComponent.PlayOneShot(ConfirmSound,1.0f);
        }
        
        if (!InitialPlatformsSpawned)
        {
            //Spawn initial Platforms
            CurrentPlatformSize = GetNextPlatformSize();
            PlatformSpawner.SpawnInitialPlatforms(CurrentPlatformSize);
            InitialPlatformsSpawned = true;
        } else if (WasLastTeleportValid)
        {
            //Spawn next Platforms
            CurrentPlatformSize = GetNextPlatformSize();
            PlatformSpawner.SpawnNextPlatforms(CurrentPlatformSize,HittedPlatform);
        }
    }
    
    public override void OnTrialEnd(Trial trial)
    {
        // log final values from teleport script
        Session.instance.CurrentTrial.result["FailedJumps"] = TeleportScript.FailedJumps;
        Session.instance.CurrentTrial.result["Successful"] = TeleportScript.SuccessfulJumps;
        Session.instance.CurrentTrial.result["Score"] = UserStudyManager.Instance.FinalScore;
        // store additional results of data stored in the tracker
        TeleportScript.TeleportTracker.SaveAverageResultData();
        // reset values
        TeleportScript.FailedJumps = 0;
        TeleportScript.SuccessfulJumps = 0;
        TeleportScript.TeleportTracker.StopRecording();
        TeleportScript.enabled = true;
        TextUI.SetActive(false);
    }
    
    private EPlatformSize GetNextPlatformSize()
    {
        if (PlatformSizes.Count > 0)
        {
            EPlatformSize Size = PlatformSizes[0];
            PlatformSizes.RemoveAt(0);
            return Size;
        }
        else
        {
            Debug.Log("No more Platform sizes in array, please make sure number of jumps is a multitude of three");
            return EPlatformSize.Large; // return something so nothing crashes
        }
    }
    
    private void Update()
    {
        
        
        if (IA_NextTrialOverride.action.WasPressedThisFrame())
        {
            Session.instance.EndCurrentTrial();
            Session.instance.BeginNextTrialSafe();
        }
        
        if (IA_PreviousTrial.action.WasPressedThisFrame())
        {
            Session.instance.EndCurrentTrial();
            int prevTrialNum = 1;
            if (Session.instance.currentTrialNum > 1)
            {
                prevTrialNum = Session.instance.currentTrialNum - 1;
            }
            Session.instance.CurrentBlock.GetRelativeTrial(prevTrialNum).Begin();
        }

        // next trial action is also the button to continue with info texts
        if (WaitForUserInput && IA_NextTrial.action.WasPressedThisFrame())
        {
            // in the practice task, display info texts before the user starts teleporting therefore return do not end the task at this point
            if (ShowInfoTexts)
            {
                // Show info text before trial starts (currently only done during the practice trial)
                ShowNextInfoText();
                return;
            }
            Session.instance.EndCurrentTrial();
        
            // if we are not at the last trial, start the next one
            if (Session.instance.CurrentTrial != Session.instance.CurrentBlock.lastTrial)
            {
                Session.instance.BeginNextTrialSafe();
            }

        }
        
        // check controller angle 
        if (ArrowUI.activeSelf)
        {
            if (UseDuck) return;
            float angle = TeleportScript.GetHorizontalControllerAngle();
            
            if (angle < -80 && angle > -100)
            {
                ControllerHoldTimeCurrent += Time.deltaTime;
                ArrowText.SetText("Hold!");
                if (ControllerHoldTimeCurrent > ControllerHoldTimeThreshold)
                {
                    ControllerHoldTimeCurrent = 0;
                    RaycastHit hit = new RaycastHit(); // TODO temporary solution until it is clear which mode we choose
                    OnResumeTeleport(hit);
                }
            }
            else
            {
                ArrowText.SetText("Point to the ground!");
            }
        }
        
    }

    private void ShowNextInfoText()
    {
        ShowInfoTexts = true;
        Vector3 uiPos = Camera.transform.position + XROrigin.transform.forward * 3.0f - XROrigin.transform.up * 0.30f;;
        if (InfoTextIterator == 0)
        {
            // Spawn UI to let user know to press a button to continue with the next trial
            BasicTextUIScript.SetText("You will now do a series of teleports onto the green water lilies in front of you.\n \n" +
                                      "Press and hold the trigger button to aim at a platform and let go of the trigger to teleport.");
        
            TextUI.SetActive(true);
            TextUI.transform.position = uiPos;
            WaitForUserInput = true;
            
        } else if (InfoTextIterator == 1)
        {
            BasicTextUIScript.SetText("Before each jump you have to aim your controller towards the ground to be able to teleport again.");
        
            TextUI.SetActive(true);
            TextUI.transform.position = uiPos;
            WaitForUserInput = true;

        } else if (InfoTextIterator == 2)
        {
            BasicTextUIScript.SetText("Each teleport will give you a score which can be seen on the panel attached to your controller.\n \n" +
                                      "Time is not important but try to maximize the score for each jump.");
        
            TextUI.SetActive(true);
            TextUI.transform.position = uiPos;
            WaitForUserInput = true;
        } else if (InfoTextIterator == 3)
        {
            BasicTextUIScript.SetText("The farther you jump, the greater the score, however, missing a platform will give a heavy point penalty.\n" +
                                      "So make sure to keep a balance between far and accurate jumps.");
        
            TextUI.SetActive(true);
            TextUI.transform.position = uiPos;
            WaitForUserInput = true;

        }
        else
        {
            // no new info text
            WaitForUserInput = false;
            TextUI.SetActive(false);
            ShowInfoTexts = false;
            SwitchToSelectionPart();
        }
        
        InfoTextIterator++;
    }
}
