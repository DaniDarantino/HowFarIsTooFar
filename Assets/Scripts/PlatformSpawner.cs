using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum EPlatformSize
{
    Small,
    Medium,
    Large
}
public class PlatformSpawner : MonoBehaviour
{
    
    // this script assumes that it is starting at 0,0,0
    
    public GameObject PlatformPrefab;
    
    // parameters can be set in settings.json
    [HideInInspector] public float SmallDiameter = 0.40f;
    [HideInInspector] public float MediumDiameter = 0.80f;
    [HideInInspector] public float LargeDiameter = 1.20f;
    [HideInInspector] public float PlatformGap = 0.5f;
    [HideInInspector] public float MaxLateralOffset = 1.0f;
    public int PlatformsToSpawn = 50;
    public float AnimationDuration = 2.0f;

    public Stack<GameObject> PlatformStack = new Stack<GameObject>();
    // start at 1 meter
    private float NextPlatformDistanceZ = 1;
    private EPlatformSize OldSize = EPlatformSize.Large;
    private List<GameObject> NewPlatforms = new List<GameObject>();
    
    // Platform animation
    private float ElapsedTime = 0.0f;
    private float UpTime = 0.0f;
    private float DownTime = 0.0f;
    private float startY = -0.2f;
    private float MaxY = 0.03f;

    private void Update()
    {
        ElapsedTime += Time.deltaTime;

        // float up
        if (ElapsedTime <= 0.6 * AnimationDuration)
        {
            UpTime += Time.deltaTime;
            int i = 0;
            foreach(GameObject platform in NewPlatforms)
            {
                Vector3 pos = platform.transform.position;
                float height = Mathf.Lerp(startY, MaxY, UpTime/(0.6f*AnimationDuration));
                pos.y = height;
                platform.transform.position = pos;
                i++;
            }
        }
        // float down
        else if(ElapsedTime > 0.6f * AnimationDuration && ElapsedTime < AnimationDuration)
        {
            DownTime += Time.deltaTime;
            // lerp back to y = 0
            int i = 0;
            foreach(GameObject platform in NewPlatforms)
            {
                float height = Mathf.Lerp(MaxY, 0.00f, DownTime/(0.4f*AnimationDuration));
                Vector3 pos = platform.transform.position;
                pos.y = height;
                platform.transform.position = pos;
                i++;
            }
        }
    }

    // current platform
    public void SpawnNextPlatforms(EPlatformSize size, GameObject currentPlatform)
    {
        float platformSize = GetPlatformSizeAsFloat(size);
        
        // remove platforms until the current one we are standing at
        while (PlatformStack.Count>0)
        {
            if (currentPlatform.gameObject.Equals(PlatformStack.Peek()))
            {
                break;
            }
            Destroy(PlatformStack.Pop());
        }

        // remove tag from "old" platforms, to prevent player from teleporting back
        foreach (var o in PlatformStack)
        {
            o.tag = "RestrictTeleport";
        }
        
        // set Z of next platform to span
        NextPlatformDistanceZ = currentPlatform.transform.position.z + GetPlatformSizeAsFloat(OldSize)/2 + platformSize/2 + PlatformGap;
        
        Quaternion rotation = Quaternion.identity;
        
        int platformsAdded = 0;
        NewPlatforms = new List<GameObject>();
        while (platformsAdded < PlatformsToSpawn)
        {
            float lateralOffset = Random.Range(-MaxLateralOffset, MaxLateralOffset);
            Vector3 position = new Vector3(lateralOffset, 0, NextPlatformDistanceZ);
            Vector3 localScale = new Vector3(platformSize,0.02f,platformSize);
            
            GameObject newPlatform = Instantiate(PlatformPrefab);
            NewPlatforms.Add(newPlatform);
            newPlatform.transform.position = position;
            newPlatform.transform.rotation = rotation;
            newPlatform.transform.localScale = localScale;
            PlatformStack.Push(newPlatform);
            
            
            // update "pointer" to next platform
            NextPlatformDistanceZ += platformSize + PlatformGap;
            platformsAdded++;
        }
        // for platform spawn animation
        ElapsedTime = 0.0f;
        UpTime = 0.0f;
        DownTime = 0.0f;
        OldSize = size;
    }

    public void SpawnInitialPlatforms(EPlatformSize size)
    {
        // destroy any old platforms and clear stack
        foreach (var o in PlatformStack)
        {
            Destroy(o);
        }
        PlatformStack.Clear();
        NextPlatformDistanceZ = 1.0f;
        
        float platformSize = GetPlatformSizeAsFloat(size);
        
        Quaternion rotation = Quaternion.identity;
        
        while (PlatformStack.Count < PlatformsToSpawn)
        {
            float lateralOffset = Random.Range(-MaxLateralOffset, MaxLateralOffset);
            Vector3 position = new Vector3(lateralOffset, 0, NextPlatformDistanceZ);
            Vector3 localScale = new Vector3(platformSize,0.02f,platformSize);
            
            GameObject newPlatform = Instantiate(PlatformPrefab);
            newPlatform.transform.position = position;
            newPlatform.transform.rotation = rotation;
            newPlatform.transform.localScale = localScale;
            PlatformStack.Push(newPlatform);
            
            
            // update "pointer" to next platform
            NextPlatformDistanceZ += platformSize + PlatformGap;
        }

        OldSize = size;
    }

    public float GetPlatformSizeAsFloat(EPlatformSize size)
    {
        float sizeInFloat = 0.0f;
        switch (size)
        {
            case EPlatformSize.Small:
                sizeInFloat = SmallDiameter;
                break;
            case EPlatformSize.Medium:
                sizeInFloat = MediumDiameter;
                break;
            case EPlatformSize.Large:
                sizeInFloat = LargeDiameter;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(size), size, null);
        }

        return sizeInFloat;
    }
}
