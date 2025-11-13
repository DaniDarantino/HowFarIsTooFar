
using System;
using UnityEngine;
using UXF;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(LineRenderer), typeof(Tracker))]
public class StudyParabola : Parabola
{
    [HideInInspector]
    public TeleportTracker TeleportTracker;

    // logged data after trial
    [HideInInspector]
    public int FailedJumps = 0;
    [HideInInspector]
    public int SuccessfulJumps = 0;
    private GameObject TargetPlatform;
    private float JumpSpecificationTime = 0.0f;
    
    public delegate void OnTryTeleportDelegate(bool validTeleport, GameObject hit, float TeleportDistance, Vector3 TargetLocation = new Vector3());
    public OnTryTeleportDelegate OnTryTeleport; 
    
    new void OnEnable()
    {
        base.OnEnable();
        // init Teleport tracker
        TeleportTracker = GetComponent<TeleportTracker>();
        if (!TeleportTracker) { Debug.LogError("Could not find TeleportTracker"); return; }
        
        // reset timer when script is enabled
        JumpSpecificationTime = 0.0f;
    }

    new void Update()
    {
        base.Update();
        JumpSpecificationTime += Time.deltaTime;
    }
    
    // overwritten to add check if we are on a teleport platform
    protected override Vector3 GetTargetLocation()
    {
        Vector3 hitLocation = Vector3.zero;
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
            hitLocation = hit.point;
            if (hitFound)
            {
                // test if hit point is valid by checking angle of normal
                float angle = Vector3.Angle(hit.normal, Vector3.up);
                if (angle < Math.Abs(AngleForHitPointValidityCheck))
                {
                    // Only allow teleportation to platforms tagged with "TeleportPoint"
                    if (hit.collider.tag == "AllowTeleport")
                    {
                        TargetLocationIsValid = true;
                        TargetPlatform = hit.collider.gameObject;
                    }
                    else
                    {
                        TargetLocationIsValid = false;
                    }
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

    // override to add logging 
    protected override bool TryTeleport()
    {
        float JumpDistance = 0.0f;
        Vector2 TargetLocation2D = new Vector2(TargetLocation.x, TargetLocation.z);
        Vector2 UserPosition2D = new Vector2(Camera.transform.position.x, Camera.transform.position.z);
        JumpDistance = (TargetLocation2D - UserPosition2D).magnitude;
        if (TargetLocationIsValid)
        {
            // record jump parameters
            TeleportTracker.JumpSuccessful = true;
            TeleportTracker.JumpDistance = JumpDistance;
            TeleportTracker.JumpSpecificationTime = JumpSpecificationTime;
            TeleportTracker.HorizontalControllerAngle = GetHorizontalControllerAngle();
            TeleportTracker.ControllerHeightAboveGround = gameObject.transform.localPosition.y;
            Vector2 HittedPlatformPosition2D =
                new Vector2(TargetPlatform.transform.position.x, TargetPlatform.transform.position.z);
            TeleportTracker.DistanceToTarget = (HittedPlatformPosition2D - TargetLocation2D).magnitude;
            TeleportTracker.RelativeControllerCoordinates = gameObject.transform.localPosition.ToString();
            
            // counter for final results of trial
            SuccessfulJumps++;
            TeleportPlayerToTargetLocation();
        }
        else
        {
            // record jump parameters
            TeleportTracker.JumpSuccessful = false;
            // in case the target location is invalid(indicated by target location being (0,0))
            // e.g. straight ray into the air, log -1000 as jump distance
            if (TargetLocation2D == Vector2.zero)
            {
                TeleportTracker.JumpDistance = -1000;
            }
            else
            {
                TeleportTracker.JumpDistance = JumpDistance;
            }
            TeleportTracker.HorizontalControllerAngle = GetHorizontalControllerAngle();
            TeleportTracker.ControllerHeightAboveGround = gameObject.transform.localPosition.y;
            TeleportTracker.JumpSpecificationTime = JumpSpecificationTime;
            TeleportTracker.RelativeControllerCoordinates = gameObject.transform.localPosition.ToString("F3");

            // counter for final results of trial
            FailedJumps++;
        }
        // Call delegate so other events can be triggered when user is teleported.
        // Other classes might also record relevant data.
        OnTryTeleport?.Invoke(TargetLocationIsValid,TargetPlatform, JumpDistance,TargetLocation);
        
        JumpSpecificationTime = 0;
        return TargetLocationIsValid;
    }

    public float GetHorizontalControllerAngle()
    {
        float angle = Vector3.SignedAngle(Vector3.up, gameObject.transform.forward, gameObject.transform.right);
        angle = angle > -90 ? angle-90 : angle += 270;
        return -angle;
    }
}
