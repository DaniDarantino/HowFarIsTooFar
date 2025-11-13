using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public abstract class UserStudyTask : MonoBehaviour
{
    
    // create trials
    public virtual void StartTask()
    {
        
    }
    
    public virtual void OnTrialBegin(Trial trial)
    {
        
    }
    
    public virtual void OnTrialEnd(Trial trial)
    {
    }

    
     // return: true, if task ended correctly or false if task ended prematurely
    public virtual bool EndTask()
    {
        return true;
    }
}
