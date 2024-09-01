using System.Collections.Generic;
using UnityEngine;

public class BallTrajectoryCalculator : MonoBehaviour
{
    public List<Vector3> CalculateTrajectory(Vector3 initialPosition, Vector3 initialVelocity, Vector2 kickPoint)
    {
        List<Vector3> trajectoryPoints = new List<Vector3>();
        int steps = 50;
        float timeStep = 0.05f;

        Vector3 position = initialPosition;
        Vector3 velocity = initialVelocity;

        for (int i = 0; i < steps; i++)
        {
            float t = i * timeStep;
            position = CalculatePositionWithMagnusEffect(position, velocity, t, kickPoint);
            trajectoryPoints.Add(position);
        }

        return trajectoryPoints;
    }

    public Vector3 CalculateInitialVelocity(Vector2 kickPoint, Vector2 dragVector)
    {
        float powerMultiplier = 20f;
        float verticalFactor = Mathf.Lerp(0f, 0.6f, Mathf.InverseLerp(0.5f, -0.5f, kickPoint.y));

        Vector3 direction = new Vector3(-dragVector.x, verticalFactor, -dragVector.y).normalized;
        return direction * powerMultiplier * dragVector.magnitude;
    }

    public Vector3 CalculatePositionWithMagnusEffect(Vector3 initialPosition, Vector3 initialVelocity, float time, Vector2 kickPoint)
    {
        float timeStep = 0.05f;
        Vector3 gravity = Physics.gravity;
        float ballRadius = 0.15f;
        float bounceCoefficient = 0.8f;

        Vector3 position = initialPosition;
        Vector3 velocity = initialVelocity;

        float elapsedTime = 0f;
        int bounceCount = 0;
        const int maxBounces = 5;

        while (elapsedTime < time)
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

            elapsedTime += timeStep;
        }

        return position;
    }

    public Vector3 CalculateMagnusForce(Vector3 velocity, Vector2 kickPoint)
    {
        // 左右方向にのみマグナス効果を適用
        float magnusCoefficient = 1.0f;
        Vector3 spinAxis = new Vector3(0, 1, 0); // Y軸に沿ったスピン
        Vector3 magnusForce = magnusCoefficient * Vector3.Cross(spinAxis, velocity);

        // kickPointのX成分が大きいほど、マグナス効果を強くする
        magnusForce *= Mathf.Abs(kickPoint.x);

        // 右側を蹴った場合は左にカーブ、左側を蹴った場合は右にカーブ
        magnusForce *= kickPoint.x > 0 ? -1 : 1;

        return magnusForce;
    }
}
