using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KickControl : MonoBehaviour
{
    [Header("BallUI")]
    [SerializeField] private GameObject ballUI;
    [SerializeField] private float baseRadius = 400f;
    [SerializeField] private RectTransform ballRectTransform;

    [Header("ArrowUI")]
    [SerializeField] private GameObject arrowUI;

    [Header("Ball3D")]
    [SerializeField] private Transform ball3D;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LineRenderer trajectoryLineRenderer;
    [SerializeField] private LineRenderer ringLineRenderer;
    [SerializeField] private float maxDragDistance = 100f;
    [SerializeField] private float baseMaxDragDistance = 100f;
    [SerializeField] private float dragScaleFactor = 0.01f;
    [SerializeField] private TextMeshProUGUI powerText;

    private bool isDragging = false;
    private Vector2 startTouchPosition;
    private MouseInputManager mouseInputManager;

    private void OnEnable()
    {
        isDragging = false;
    }

    private void Start()
    {
        mouseInputManager = GetComponent<MouseInputManager>();
        maxDragDistance = CalculateMaxDragDistance();
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;

        if (!isDragging && Input.GetMouseButtonDown(0))
        {
            if (IsMouseInsideBall(mousePos))
            {
                StartDrag(mousePos);
            }
        }

        if (isDragging)
        {
            HandleDragging(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            EndDrag();
        }
    }

    private void StartDrag(Vector2 mousePos)
    {
        startTouchPosition = mousePos;
        ballUI.SetActive(false);
        arrowUI.SetActive(true);
        trajectoryLineRenderer.enabled = true;
        ringLineRenderer.enabled = true;
        isDragging = true;
        Cursor.visible = false;
    }

    private void HandleDragging(Vector2 currentTouchPosition)
    {
        Vector2 dragVector = GetDragVector(currentTouchPosition);
        DrawTrajectoryAndRing(dragVector);
        UpdatePowerText(dragVector);
        UpdateArrow(Vector2.zero, dragVector / maxDragDistance);
    }

    private void EndDrag()
    {
        isDragging = false;
        arrowUI.SetActive(false);
        trajectoryLineRenderer.positionCount = 0;
        trajectoryLineRenderer.enabled = false;
        ringLineRenderer.positionCount = 0;
        ringLineRenderer.enabled = false;
        Cursor.visible = true;
    }

    private Vector2 GetDragVector(Vector2 currentTouchPosition)
    {
        Vector2 dragVector = currentTouchPosition - startTouchPosition;
        float dragDistance = Mathf.Min(dragVector.magnitude, maxDragDistance);
        return dragVector.normalized * dragDistance;
    }

    private void DrawTrajectoryAndRing(Vector2 dragVector)
    {
        Vector3 groundStartPoint = GetGroundPointBelowBall();
        groundStartPoint += Vector3.up * 0.05f;
        Vector3 groundEndPoint = ScreenToWorldPointOnGround(groundStartPoint, dragVector * dragScaleFactor);
        groundEndPoint += Vector3.up * 0.05f;

        DrawDashedLine(groundStartPoint, groundEndPoint);
        DrawRing(groundStartPoint, groundEndPoint);
    }

    private void UpdatePowerText(Vector2 dragVector)
    {
        float powerRate = Mathf.Min(dragVector.magnitude / maxDragDistance, 1.0f);
        powerText.text = $"{powerRate * 100f:F0}%";
        Vector3 ringCenter = GetRingCenter(dragVector);
        powerText.transform.position = Camera.main.WorldToScreenPoint(ringCenter + Vector3.up * 0.32f);
    }

    private void DrawRing(Vector3 groundStartPoint, Vector3 groundEndPoint)
    {
        float length = (groundEndPoint - groundStartPoint).magnitude;
        float radius = 0.2f;
        Vector3 ringCenter = groundStartPoint + (groundEndPoint - groundStartPoint).normalized * (length + radius);
        ringCenter += Vector3.up * 0.05f;

        ringLineRenderer.loop = true;
        ringLineRenderer.positionCount = 101;
        float angleStep = 360f / 100;

        for (int i = 0; i <= 100; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            Vector3 position = new Vector3(ringCenter.x + x, ringCenter.y, ringCenter.z + z);
            ringLineRenderer.SetPosition(i, position);
        }
    }

    private Vector3 GetGroundPointBelowBall()
    {
        Ray ray = new Ray(ball3D.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            return hit.point;
        }
        return ball3D.position;
    }

    private Vector3 ScreenToWorldPointOnGround(Vector3 groundStartPoint, Vector2 dragVector)
    {
        return new Vector3(groundStartPoint.x + dragVector.x,
            groundStartPoint.y,
            groundStartPoint.z + dragVector.y);
    }

    private void DrawDashedLine(Vector3 start, Vector3 end)
    {
        trajectoryLineRenderer.positionCount = 2;
        trajectoryLineRenderer.SetPosition(0, start);
        trajectoryLineRenderer.SetPosition(1, end);
    }

    private Vector3 CalculateInitialVelocity(Vector2 dragVector)
    {
        float powerMultiplier = 20f;
        Vector3 direction = new Vector3(-dragVector.x, 0.3f, -dragVector.y).normalized;
        return direction * powerMultiplier * dragVector.magnitude;
    }

    private void UpdateArrow(Vector2 kickPoint, Vector2 dragVector)
    {
        Vector3 initialPosition = ball3D.position;
        Vector3 initialVelocity = CalculateInitialVelocity(dragVector);

        int steps = 50;
        float timeStep = 0.05f;
        trajectoryLineRenderer.positionCount = steps;

        float bounceCoefficient = 0.8f;
        float ballRadius = 0.15f;
        Vector3 velocity = initialVelocity;

        for (int i = 0; i < steps; i++)
        {
            float t = i * timeStep;
            Vector3 position = CalculatePosition(initialPosition, velocity, t);

            if (position.y <= ballRadius && velocity.y < 0)
            {
                velocity.y = -velocity.y * bounceCoefficient;
                velocity.x *= bounceCoefficient;
                velocity.z *= bounceCoefficient;
                position.y = ballRadius;
                position += velocity * timeStep * 0.5f;
            }

            trajectoryLineRenderer.SetPosition(i, position);
        }
    }

    private Vector3 CalculatePosition(Vector3 initialPosition, Vector3 velocity, float time)
    {
        Vector3 gravity = Physics.gravity;
        float timeStep = 0.05f;

        return initialPosition + velocity * time + 0.5f * gravity * time * time;
    }

    private bool IsMouseInsideBall(Vector2 mousePos)
    {
        Vector2 ballCenter = ballRectTransform.position;
        float distance = Vector2.Distance(mousePos, ballCenter);
        return distance <= GetScaledRadius();
    }

    private float GetScaledRadius()
    {
        float scaleFactor = (float)Screen.width / 1920f;
        return baseRadius * scaleFactor;
    }

    private float CalculateMaxDragDistance()
    {
        float screenScaleFactor = (float)Screen.width / 1920f;
        return baseMaxDragDistance * screenScaleFactor;
    }

    private Vector3 GetRingCenter(Vector2 dragVector)
    {
        Vector3 groundStartPoint = GetGroundPointBelowBall();
        groundStartPoint += Vector3.up * 0.05f;
        Vector3 groundEndPoint = ScreenToWorldPointOnGround(groundStartPoint, dragVector * dragScaleFactor);
        groundEndPoint += Vector3.up * 0.05f;

        float length = (groundEndPoint - groundStartPoint).magnitude;
        float radius = 0.2f;
        return groundStartPoint + (groundEndPoint - groundStartPoint).normalized * (length + radius);
    }
}
