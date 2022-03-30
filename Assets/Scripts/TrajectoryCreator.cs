using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryCreator : MonoBehaviour
{
    // Components
    private LineRenderer MLineRenderer;
    private Transform MTransform;

    // Variables
    /// <summary>
    /// If true, direction will same with transform.formward
    /// </summary>
    public bool UseTransform = false;

    /// <summary>
    /// Horizontal and vertical angle of trajectory. if UseTransform is true. these will be overrided.
    /// https://en.wikipedia.org/wiki/Aircraft_principal_axes
    /// </summary>
    public float YawAngle = 0f;
    public float PitchAngle = 45f;

    /// <summary>
    /// Resolution of the trajectory
    /// </summary>
    public int PositionStep = 16;

    /// <summary>
    /// To limit trajectory distance as meter
    /// </summary>
    public float MaxProjectileLength = 350f;

    /// <summary>
    /// Initial velocity of ray as m/s
    /// </summary>
    public float VelocityMagnitude = 50f;

    /// <summary>
    /// Gravity force as m/s
    /// </summary>
    public float GravityYForce = -9.81f;
    private bool IsInitialized = false;

    /// <summary>
    /// To automatically initialize the component.
    /// </summary>
    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Get components and do startp settings.
    /// </summary>
    private void Initialize()
    {
        IsInitialized = true;
        MTransform = transform;
        MLineRenderer = GetComponent<LineRenderer>();
        MLineRenderer.useWorldSpace = true;
        MLineRenderer.positionCount = 0;
    }

    /// <summary>
    /// Calculate projectile continuously
    /// </summary>
    private void Update()
    {
        if (IsInitialized)
        {
            Vector3 start = MTransform.position;
            Vector3 direction = Vector3.forward;

            if (UseTransform)
            {
                YawAngle = MTransform.localEulerAngles.y;
                PitchAngle = -MTransform.localEulerAngles.x;
            }

            direction = Quaternion.Euler(-PitchAngle, YawAngle, 0) * direction;


            // Limit some values
            if (PositionStep < 3)
                PositionStep = 3;
            if (MaxProjectileLength < 0.001f)
                MaxProjectileLength = 0.001f;
            if (VelocityMagnitude < 0f)
                VelocityMagnitude = 0f;

            // Draw the trajectory
            float timeStep = MaxProjectileLength / (PositionStep * VelocityMagnitude);
            MLineRenderer.positionCount = PositionStep + 1;
            for (int i = 0; i < MLineRenderer.positionCount; i++)
            {
                float time = i * timeStep;

                Vector3 position = start + GetPosition(time, direction * VelocityMagnitude);
                MLineRenderer.SetPosition(i, position);
            }
        }
    }

    /// <summary>
    /// Calculates position with projectile motion formulas
    /// https://en.wikipedia.org/wiki/Projectile_motion
    /// </summary>
    /// <param name="time"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    private Vector3 GetPosition(float time, Vector3 speed)
    {
        Vector3 result = Vector3.zero;

        result.x = speed.x * time;
        result.y = (speed.y * time) + (0.5f * GravityYForce * time * time);
        result.z = speed.z * time;

        return result;
    }
}
