 using System.Collections.Generic;
using UnityEngine;

public static class BallTrajectoryCalculator
{
    private static Vector3 gravity = Physics.gravity;
    private static float ballRadius = 0.15f;
    private static float bounceCoefficient = 0.8f;
    private static int maxBounces = 5;

    public static Vector3 CalculateInitialVelocity(Vector2 kickPoint, Vector2 dragVector, float powerMultiplier = 20f)
    {
        float verticalFactor = Mathf.Lerp(0f, 0.6f, Mathf.InverseLerp(0.5f, -0.5f, kickPoint.y));
        Vector3 direction = new Vector3(-dragVector.x, verticalFactor, -dragVector.y).normalized;
        return direction * powerMultiplier * dragVector.magnitude;
    }

    public static List<Vector3> CalculateTrajectory(Vector3 initialPosition, Vector3 initialVelocity, Vector2 kickPoint, int steps, float timeStep)
    {
        List<Vector3> trajectoryPoints = new List<Vector3>();
        Vector3 position = initialPosition;
        Vector3 velocity = initialVelocity;

        float elapsedTime = 0f;
        int bounceCount = 0;

        for (int i = 0; i < steps; i++)
        {
            Vector3 magnusForce = CalculateMagnusForce(velocity, kickPoint);
            Vector3 acceleration = gravity + magnusForce;

            position += velocity * timeStep + 0.5f * acceleration * timeStep * timeStep;
            velocity += acceleration * timeStep;

            if (position.y <= ballRadius && velocity.y < 0 && bounceCount < maxBounces)
            {
                velocity.y = -velocity.y * bounceCoefficient;
                velocity.x *= bounceCoefficient;
                velocity.z *= bounceCoefficient;
                bounceCount++;
                position.y = ballRadius;
            }

            if (position.y < ballRadius)
            {
                position.y = ballRadius;
                velocity.y = 0;
            }

            trajectoryPoints.Add(position);
            elapsedTime += timeStep;
        }

        return trajectoryPoints;
    }

    private static Vector3 CalculateMagnusForce(Vector3 velocity, Vector2 kickPoint)
    {
        float magnusCoefficient = 1.0f;
        Vector3 spinAxis = new Vector3(0, 1, 0);
        Vector3 magnusForce = magnusCoefficient * Vector3.Cross(spinAxis, velocity);

        magnusForce *= Mathf.Abs(kickPoint.x);
        magnusForce *= kickPoint.x > 0 ? -1 : 1;

        return magnusForce;
    }
}
