
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ParabolaAlternative : MonoBehaviour
{
    /*// called every frame while trigger is pressed
    protected override Vector3 GetTargetLocation()
    {
        Vector3 hitLocation = Vector3.one;
        Quaternion hitRotation = Quaternion.identity;
        targetLocationIsValid = false;
        lineRenderer.positionCount = 0;
        
        Vector3 velocity = gameObject.transform.forward * emissionVelocity;
        float lastT = ComputeTForGivenYLocal(velocity, -gameObject.transform.position.y); // can be replaced with more elaborate intersection test
        float iterT = 0.0f;
        
        lineRenderer.positionCount = 0;
        
        // set first point at controller
        Vector3 previousPosition = gameObject.transform.position;
        /*lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1,previousPosition);#1#
        RaycastHit hit;
        bool hitFound = false;
        // generate points for line renderer along arc until hit was found
        for (int i = 0; i < maxLinePoints; i++)
        {
            if (iterT < lastT)
            {
                Vector3 currentPosition = ComputeProjectilePoint(gameObject.transform.position, velocity, iterT);
                iterT += tLineIncrement; 
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1,currentPosition);
            
                // draw ray and check for hit
                Vector3 traceDirection = currentPosition - previousPosition;
                hitFound = Physics.Raycast(previousPosition, traceDirection, out hit, traceDirection.magnitude,layerMask);
                if (hitFound)
                {
                    //Debug.DrawRay(previousPosition, traceDirection, Color.yellow,1.0f);
                    hitLocation = hit.point;
                    targetLocationIsValid = true;
                    // update last point of line renderer to be at hit location
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, hitLocation);
                    break;
                }
                previousPosition = currentPosition;
            }
        }
        return hitLocation;
    }
    
    Vector3 ComputeProjectilePoint(Vector3 initialPosition, Vector3 initialVelocity, float t)
    {
        float theta = GetPitchAngle();
        Vector3 forward = initialVelocity.normalized;
        float v0 = initialVelocity.magnitude;

        // Projectile motion equation formulas: https://www.toppr.com/guides/physics/motion-in-a-plane/projectile-motion/
        float xLocal = (v0 * Mathf.Cos(Mathf.Deg2Rad * theta)) * t;
        float yLocal = (v0 * Mathf.Sin(Mathf.Deg2Rad * theta)) * t - 0.5f * gravity * t * t;

        Vector3 horizontalForward = new Vector3(forward.x, 0.0f, forward.z);
        horizontalForward.Normalize();

        Vector3 projectilePoint = initialPosition + horizontalForward * xLocal;
        projectilePoint.y += yLocal;

        return projectilePoint;
    }

    float ComputeTForGivenYLocal(Vector3 initialVelocity, float yLocal)
    {
        float theta = GetPitchAngle();
        float v0 = initialVelocity.magnitude;
        float a = -0.5f * gravity;
        float b = v0 * Mathf.Sin(Mathf.Deg2Rad * theta);
        float c = -yLocal;  

        float t1 = (-b + Mathf.Sqrt(b*b - 4 * a * c)) / (2 * a); // solve quadratic equation
        float t2 = (-b - Mathf.Sqrt(b*b - 4 * a * c)) / (2 * a); // solve quadratic equation
        return Mathf.Max(t1, t2); // return positive value of t as forward intersection point
    }

    float GetPitchAngle()
    {
        float angle = gameObject.transform.localEulerAngles.x;
        if (angle < 90.0f) {
            return -angle;
        }
        else if (angle > 270.0f) {
            return 360.0f - angle;
        }
        return angle;
    }*/
}
