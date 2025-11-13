
using System;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(LineRenderer))]
public class Parabola : TeleportBase
{
    
    [Range(10,1000)]
    public int MaxLinePoints = 100;
    
    [Range(0.001f, 1.0f)]
    public float LineIncrement = 0.02f;

    [Range(0.0f, 180.0f)] 
    public float AngleForHitPointValidityCheck = 45.0f;
    public bool UseGravity = true;
    public Material TeleportValidLineMaterial;
    public Material TeleportInvalidLineMaterial;
    public LayerMask LayerMask;
    [HideInInspector]
    public float EmissionVelocity = 8.0f;
    
    protected LineRenderer LineRend;
    private float Gravity = Physics.gravity.y;

    
    protected void OnEnable()
    {
        InitParameter();
        // init line Renderer
        LineRend = GetComponent<LineRenderer>();
        if (!LineRend) { Debug.LogError("Could not find LineRenderer"); return; }
        LineRend.numCapVertices = 5;
        LineRend.startWidth = 0.01f;
        LineRend.material = TeleportValidLineMaterial;
        LineRend.enabled = false;
    }

    protected override void ShowTeleportVisualization()
    {
        LineRend.enabled = true;
        if (TargetLocationIsValid)
        {
            // color green
            LineRend.material = TeleportValidLineMaterial;
            // color green
            TargetLocationVisualization.SetActive(true);
        }
        else
        {
            TargetLocationVisualization.SetActive(false);
            // color red
            LineRend.material = TeleportInvalidLineMaterial;
        }
    }
    
    // called every frame while trigger is pressed
    protected override Vector3 GetTargetLocation()
    {
        Vector3 hitLocation = Vector3.one;
        TargetLocationIsValid = false;
        LineRend.positionCount = 0;
        
        // set first point at controller
        Vector3 previousPosition = gameObject.transform.position;
        LineRend.positionCount++;
        LineRend.SetPosition(LineRend.positionCount - 1, previousPosition);
        
        RaycastHit hit;
        bool hitFound = false;
        // generate points for line renderer along arc until hit was found
        for (int i = 0; i < MaxLinePoints; i++)
        {
            Vector3 currentPosition = GetArcPositionAtTime(i * LineIncrement);
            LineRend.positionCount++;
            LineRend.SetPosition(LineRend.positionCount - 1, currentPosition);

            // draw ray and check for hit between each arc point
            Vector3 traceDirection = currentPosition - previousPosition;
            hitFound = Physics.Raycast(previousPosition, traceDirection, out hit, traceDirection.magnitude, LayerMask);
            if (hitFound)
            {
                // test if hit point is valid by checking angle of normal
                float angle = Vector3.Angle(hit.normal, Vector3.up);
                if (angle < Math.Abs(AngleForHitPointValidityCheck))
                {
                    //Debug.DrawRay(previousPosition, traceDirection, Color.yellow,1.0f);
                    hitLocation = hit.point;
                    TargetLocationIsValid = true;
                    // update last point of line renderer to be at hit location
                    LineRend.SetPosition(LineRend.positionCount - 1, hitLocation);
                }
                break;
            }
            previousPosition = currentPosition;
        }
        ShowTeleportVisualization();
        return hitLocation;
    }

    protected override void CleanupTeleportVisualization()
    {
        // remove old points
        LineRend.positionCount = 0;
        LineRend.enabled = false;
        // deactivate position marker before moving user to goal location
        TargetLocationVisualization.SetActive(false);
    }
    
    public Vector3 GetArcPositionAtTime(float time)
    {
        float arcZ = EmissionVelocity * time;
        float arcY = (0.5f * Gravity * time * time);
        Vector3 offset = gameObject.transform.position + gameObject.transform.forward * arcZ + Vector3.up * arcY;
        
        return offset;
    }

    protected void SetUseGravity(bool useGravity)
    {
        this.UseGravity = useGravity;
        Gravity = useGravity ? Physics.gravity.y : 0.0f;
    }
}
