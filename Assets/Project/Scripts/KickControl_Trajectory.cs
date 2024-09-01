using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class KickControl : MonoBehaviour
{
    private void UpdateArrow(Vector2 kickPoint, Vector2 dragVector)
    {
        Vector3 initialPosition = ball3D.position;
        Vector3 initialVelocity = CalculateInitialVelocity(kickPoint, dragVector);

        int steps = 50;
        float timeStep = 0.05f;
        trajectoryLineRenderer.positionCount = steps;

        for (int i = 0; i < steps; i++)
        {
            float t = i * timeStep;
            Vector3 position = CalculatePositionWithMagnusEffect(initialPosition, initialVelocity, t, kickPoint);
            trajectoryLineRenderer.SetPosition(i, position);
        }
    }

    private Vector3 CalculateInitialVelocity(Vector2 kickPoint, Vector2 dragVector)
    {
        float powerMultiplier = 20f;

        // �S���A�Ⴂ�O���A���u�𖳒i�K�ŏ�������
        float verticalFactor = Mathf.Lerp(0f, 0.6f, Mathf.InverseLerp(0.5f, -0.5f, kickPoint.y));

        Vector3 direction = new Vector3(-dragVector.x, verticalFactor, -dragVector.y).normalized;
        return direction * powerMultiplier * dragVector.magnitude;
    }

    private Vector3 CalculatePositionWithMagnusEffect(Vector3 initialPosition, Vector3 initialVelocity, float time, Vector2 kickPoint)
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
            // Magnus���ʂ̓K�p�i���E�����̂݁j
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

            // �{�[�����n�ʂ������Ȃ��悤�ɂ���
            if (position.y < ballRadius)
            {
                position.y = ballRadius;
                velocity.y = 0;
            }

            elapsedTime += timeStep;
        }

        return position;
    }

    private Vector3 CalculateMagnusForce(Vector3 velocity, Vector2 kickPoint)
    {
        // ���E�����ɂ̂݃}�O�i�X���ʂ�K�p
        float magnusCoefficient = 1.0f;
        Vector3 spinAxis = new Vector3(0, 1, 0); // Y���ɉ������X�s��
        Vector3 magnusForce = magnusCoefficient * Vector3.Cross(spinAxis, velocity);

        // kickPoint��X�������傫���قǁA�}�O�i�X���ʂ���������
        magnusForce *= Mathf.Abs(kickPoint.x);

        // �E�����R�����ꍇ�͍��ɃJ�[�u�A�������R�����ꍇ�͉E�ɃJ�[�u
        magnusForce *= kickPoint.x > 0 ? -1 : 1;

        return magnusForce;
    }
}
