using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UXF;

public class TeleportTracker : Tracker
{
    // DATA will be set by StudyParabola script and user study task
    
    // Euclidean distance in 2D from user position to selected target location
    [HideInInspector] public float JumpDistance = 0.0f;
    // Time until user presses the trigger to perform a jump
    [HideInInspector] public float JumpSpecificationTime = 0.0f;
    // If the target location intersects with a teleport platform, jump is considered successful
    [HideInInspector] public bool JumpSuccessful = false;
    // Angle from controller forward to horizontal plane, -90 = straight down, +90 = straight up
    [HideInInspector] public float HorizontalControllerAngle = 0.0f;
    // Distance from XROrigin.y to Controller.y
    [HideInInspector] public float ControllerHeightAboveGround = 0.0f;
    // Platform size for river task (small,medium,large)
    [HideInInspector] public EPlatformSize PlatformSize = EPlatformSize.Small;
    // In case of successful jump: distacne to platform center
    // In case of unsuccessful jump: distance to nearest platform geometry (distance to nearest platform center - platform radius)
    [HideInInspector] public float DistanceToTarget = 0.0f;
    // Same as HorizontalControllerAngle but during selection(duck) task
    [HideInInspector] public float SelectionHorizontalControllerAngle = 0.0f;
    // Same as ControllerHeightAboveGround but during selection(duck) task
    [HideInInspector] public float SelectionControllerHeightAboveGround = 0.0f;
    // Controller position relative to xr origin
    [HideInInspector] public String RelativeControllerCoordinates = "";
    
    public override string MeasurementDescriptor => "TeleportTracker";
    public override IEnumerable<string> CustomHeader => new string[] {"JumpSuccessful","PlatformSize","JumpDistance", "JumpSpecificationTime", 
        "HorizontalControllerAngle", "ControllerHeightAboveGround", "DistanceToTarget","SelectionHorizontalControllerAngle","SelectionControllerHeightAboveGround","RelativeControllerCoordinates"};
    
    protected override UXFDataRow GetCurrentValues()
    {
        var teleportData = new UXFDataRow()
        {
            ("JumpSuccessful", JumpSuccessful),
            ("PlatformSize", PlatformSize.ToString()),
            ("JumpDistance", JumpDistance),
            ("JumpSpecificationTime", JumpSpecificationTime),
            ("HorizontalControllerAngle", HorizontalControllerAngle),
            ("ControllerHeightAboveGround", ControllerHeightAboveGround),
            ("DistanceToTarget", DistanceToTarget),
            ("SelectionHorizontalControllerAngle", SelectionHorizontalControllerAngle),
            ("SelectionControllerHeightAboveGround", SelectionControllerHeightAboveGround),
            ("RelativeControllerCoordinates", RelativeControllerCoordinates),
        };
        
        return teleportData;
    }

    public void SaveAverageResultData()
    {

    }

    private object GetAverage(List<float> list)
    {
        if (list.Count < 1) return 0.0f;

        return list.Average();
    }

    private float GetStdDev(List<float> list)
    {
        if (list.Count < 1) return 0.0f;
        
        float mean = list.Average();
        IEnumerable<float> squares = from float value in list
            select (value - mean) * (value - mean);

        float sum_of_squares = squares.Sum();

        return (float)Math.Sqrt(sum_of_squares / (list.Count()));
    }
    
}
