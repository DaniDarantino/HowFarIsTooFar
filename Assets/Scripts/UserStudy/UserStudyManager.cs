using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UXF;

public class UserStudyManager : MonoBehaviour
{
    // Store first instance of manager to make sure only one instance is ever created (singleton)
    public static UserStudyManager Instance;

    // Settings that are persistent during the whole user study
    public bool IsRightHanded = true;
    public int ParticipantID = 0;
    public float FinalScore = 0.0f;
    
    [SerializeField] private InputActionReference IA_CloseApplication;
    
    // Prefix of Participant ID, e.g. T for Trier and A for Aachen
    public String IDPrefix;
    
    private UserStudyTask CurrentTask;
    
    private void Awake()
    {
        // If there is a game object containing a user study manager script, but another one already exists
        // delete it
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // This will keep the game object loaded even if we switch scenes
        DontDestroyOnLoad(gameObject);
        
    }

    // Called by UXF Session script, once, when the game is started.
    public void OnSessionBegin(Session session)
    {
        // Create Blocks and Trials for all tasks
        
        // Settings map has two trials (handedness and user id)
        Block settingsBlock = Session.instance.CreateBlock(1);
        settingsBlock.settings.SetValue("Map","SettingsScene");
        
        // River task
        int numMainTrials = 9;
        int minVelocity = 6; 
        int velocityIncrement = 2;
        // velocity 6 bis 24
        
        int velocity = minVelocity;
        Block riverBlock = Session.instance.CreateBlock(numMainTrials);
        riverBlock.settings.SetValue("Map","RiverWater");
        riverBlock.settings.SetValue("Practice",false);
        
        foreach (Trial t in riverBlock.trials)
        {
            t.settings.SetValue("Velocity", velocity);
            velocity += velocityIncrement;
        }

        Trial infTrial = riverBlock.CreateTrial();
        infTrial.settings.SetValue("Velocity",10000);

        riverBlock.trials.Shuffle();
        
        // River test
        Block riverTestBlock = Session.instance.CreateBlock(1);
        riverTestBlock.firstTrial.settings.SetValue("Velocity", riverBlock.firstTrial.settings.GetFloat("Velocity"));
        riverTestBlock.settings.SetValue("Practice",true);
        riverTestBlock.settings.SetValue("Map","RiverWater");
        
        // create empty block for final map
        Block finalBlock = Session.instance.CreateBlock(0);
        finalBlock.settings.SetValue("Map","FinalScene");
        
        // Recreate blocks array so blocks are in correct order
        // This is what defines your user study procedure
        session.blocks = new List<Block> {settingsBlock, riverTestBlock, riverBlock, finalBlock};
        
        // to enable data saving
        session.saveData = true;
        
        // find the UserStudyTask class (that defines the logic for the current block)
        CurrentTask = GameObject.FindGameObjectWithTag("UserStudyTask").GetComponent<UserStudyTask>();
        if (!CurrentTask)
        {
            Debug.LogError("Could not find game object with tag 'UserStudyTask' in current scene.");
            return;
        }
        
        CurrentTask.StartTask();
    }

    // Event called by UXF Session script
    public void OnTrialBegin(Trial trial)
    {
        CurrentTask.OnTrialBegin(trial);
    }
    
    // Event called by UXF Session script
    public void OnTrialEnd(Trial trial)
    {
        CurrentTask.OnTrialEnd(trial);
        
        // if this is the last trial in the current block end the current task to e.g. switch to a new scene
        if (trial.numberInBlock == trial.block.lastTrial.numberInBlock)
        {
            NextTask();
        }
    }

    private void NextTask()
    {
        Debug.Log("Netx Task!");
        // in case task did not end properly, do not do anything (UserStudyTask child classes should deal with this case)
        if (!CurrentTask.EndTask()) return;
        
        // is there a next block available?
        if (Session.instance.CurrentBlock.number < Session.instance.blocks.Count)
        {
            Block nextBlock = Session.instance.blocks[Session.instance.CurrentBlock.number];
            Debug.Log("Load Map: " +nextBlock.settings.GetString("Map"));
            AsyncOperation loadScene = SceneManager.LoadSceneAsync(nextBlock.settings.GetString("Map"));
            if (loadScene != null) loadScene.completed += (op) => { OnSceneLoaded(); };
        }
    }

    // when transitioning to the next task, wait until the scene for the next task is loaded,
    // find the task script and call the start task function
    private void OnSceneLoaded()
    {
        CurrentTask = GameObject.FindGameObjectWithTag("UserStudyTask").GetComponent<UserStudyTask>();
        if (!CurrentTask)
        {
            Debug.LogError("Could not find game object with tag 'UserStudyTask' in current scene.");
            return;
        }
        
        CurrentTask.StartTask();
    }

    private void Update()
    {
        if (IA_CloseApplication.action.WasPerformedThisFrame())
        {
            Session.instance.End();
            Application.Quit();
        }
    }
}
