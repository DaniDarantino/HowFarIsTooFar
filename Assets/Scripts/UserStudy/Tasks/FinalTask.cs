using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UXF;

public class FinalTask : UserStudyTask
{
    
    public float TimeUntilShutdownInSeconds = 10.0f;
    public TextMeshProUGUI TxtFinal;
    private float ElapsedTime = 0.0f;
    
    
    public override void StartTask()
    {
        Session.instance.End();
        TxtFinal.SetText("You have completed the last trial and can remove the headset.\n \n " +
                         "Your final score is: "+UserStudyManager.Instance.FinalScore.ToString("F1")+"!");
    }

    // Update is called once per frame
    void Update()
    {
        ElapsedTime += Time.deltaTime;
        if (ElapsedTime > TimeUntilShutdownInSeconds)
        {
            Application.Quit();
        }
    }
}
