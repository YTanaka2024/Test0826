using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class KickControl : MonoBehaviour
{
    private void UpdateArrow(Vector2 kickPoint, Vector2 dragVector)
    {
        Vector3 initialPosition = ball3D.position;
        Vector3 initialVelocity = trajectoryCalculator.CalculateInitialVelocity(kickPoint, dragVector);

        int steps = 50;
        float timeStep = 0.05f;
        trajectoryLineRenderer.positionCount = steps;

        for (int i = 0; i < steps; i++)
        {
            float t = i * timeStep;
            Vector3 position = trajectoryCalculator.CalculatePositionWithMagnusEffect(
                initialPosition, initialVelocity, t, kickPoint);
            trajectoryLineRenderer.SetPosition(i, position);
        }
    }
}
