using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMover : MovableObject
{
    private Vector3 initialPosition;
    private Vector3 initialVelocity;
    private Vector2 kickPoint;
    private bool isMoving = false;
    private float timeElapsed = 0f;
    private BallTrajectoryCalculator trajectoryCalculator;

    private void Start()
    {
        trajectoryCalculator = FindObjectOfType<BallTrajectoryCalculator>();
    }

    public void StartMoving(Vector3 startPos, Vector3 velocity, Vector2 kickPt)
    {
        initialPosition = startPos;
        initialVelocity = velocity;
        kickPoint = kickPt;
        isMoving = true;
        timeElapsed = 0f;  // タイマーリセット
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveBall();
        }
    }

    private void MoveBall()
    {
        float timeStep = Time.deltaTime;
        timeElapsed += timeStep;

        Vector3 previousPosition = transform.position; // 現在の位置を保存
        Vector3 targetPosition = trajectoryCalculator.CalculatePositionWithMagnusEffect(
            initialPosition, initialVelocity, timeElapsed, kickPoint);

        // 補間を使用してスムーズに移動
        transform.position = Vector3.Lerp(previousPosition, targetPosition, 0.5f);

        if (transform.position.y <= 0.15f && initialVelocity.magnitude < 0.05f)
        {
            isMoving = false;
        }
    }
}
