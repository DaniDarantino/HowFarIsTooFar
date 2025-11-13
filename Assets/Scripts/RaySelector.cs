using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class RaySelector : Parabola
{
    public delegate void OnSelectDelegate(RaycastHit hit);
    public OnSelectDelegate OnSelect;
    public RaycastHit hit;
    public new void OnEnable()
    {
        base.OnEnable();
        SetUseGravity(false);
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
                hitLocation = hit.point;
                // test if hit point is valid by checking angle of normal
                float angle = Vector3.Angle(hit.normal, Vector3.up);
                // update last point of line renderer to be at hit location
                LineRend.SetPosition(LineRend.positionCount - 1, hitLocation);
                TargetLocationIsValid = true;
                break;
            }
            previousPosition = currentPosition;
        }
        ShowTeleportVisualization();
        return hitLocation;
    }

    protected override bool TryTeleport()
    {
        if (TargetLocationIsValid)
        {
            OnSelect?.Invoke(hit);
        }
        return false;
    }
}
