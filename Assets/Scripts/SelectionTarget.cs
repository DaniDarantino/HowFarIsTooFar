using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SelectionTarget : MonoBehaviour
{
    
    private AudioSource AudioSourceComponent;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        AudioSourceComponent = GetComponent<AudioSource>();
        if (!AudioSourceComponent)
        {
            Debug.Log("Could not find AudioSourceComponent at SelectionTarget");
        }
    }
    
    public void PlaySound()
    {
        AudioSourceComponent.PlayOneShot(AudioSourceComponent.clip, 0.8f);
    }
    
}
