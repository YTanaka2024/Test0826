using UnityEngine;
using TMPro;

public partial class KickControl : MonoBehaviour
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
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LineRenderer ringLineRenderer;
    [SerializeField] private LineRenderer trajectoryLineRenderer;
    [SerializeField] private float baseMaxDragDistance = 100f;
    [SerializeField] private float dragScaleFactor = 0.01f;
    [SerializeField] private TextMeshProUGUI powerText;

    private float maxDragDistance;
    private bool isDragging = false;
    private Vector2 startTouchPosition;
    private BallTrajectoryCalculator trajectoryCalculator;

    private void OnEnable()
    {
        isDragging = false;
    }

    private void Start()
    {
        maxDragDistance = CalculateMaxDragDistance();
        trajectoryCalculator = FindObjectOfType<BallTrajectoryCalculator>();
    }

    private void Update()
    {
        if (!isDragging && Input.GetMouseButtonDown(0))
        {
            TryStartDragging();
        }

        if (isDragging)
        {
            UpdateDragging();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            EndDragging();
        }
    }

    private bool IsMouseInsideBall(Vector2 mousePos, float radius)
    {
        Vector2 ballCenter = ballRectTransform.position;
        return Vector2.Distance(mousePos, ballCenter) <= radius;
    }

    private float GetScaledRadius()
    {
        float scaleFactor = (float)Screen.width / 1920f;
        return baseRadius * scaleFactor;
    }

    private void TryStartDragging()
    {
        Vector2 mousePos = Input.mousePosition;

        if (IsMouseInsideBall(mousePos, GetScaledRadius()))
        {
            startTouchPosition = mousePos;
            ballUI.SetActive(false);
            arrowUI.SetActive(true);
            lineRenderer.enabled = true;
            ringLineRenderer.enabled = true;
            trajectoryLineRenderer.enabled = true;
            isDragging = true;

            Cursor.visible = false;
        }
    }

    private void UpdateDragging()
    {
        Vector2 dragVector = CalculateDragVector();

        // ドラッグ距離の制限
        float dragDistance = Mathf.Min(dragVector.magnitude, maxDragDistance);
        dragVector = dragVector.normalized * dragDistance;

        Vector3 groundStartPoint = GetGroundPointBelowBall();
        Vector3 groundEndPoint = CalculateGroundEndPoint(groundStartPoint, dragVector);
        DrawDashedLine(groundStartPoint, groundEndPoint);

        DrawRing(CalculateRingCenter(groundStartPoint, groundEndPoint, 0.2f), 0.2f);
        UpdatePowerText(dragDistance / maxDragDistance, CalculateRingCenter(groundStartPoint, groundEndPoint, 0.2f));

        Vector2 kickPoint = CalculateKickPoint(dragVector);
        UpdateArrow(kickPoint, dragVector / maxDragDistance);
    }

    private void EndDragging()
    {
        isDragging = false;
        arrowUI.SetActive(false);
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;
        ringLineRenderer.positionCount = 0;
        ringLineRenderer.enabled = false;
        trajectoryLineRenderer.positionCount = 0;
        trajectoryLineRenderer.enabled = false;

        Cursor.visible = true;

        BallMover ballMover = ball3D.GetComponent<BallMover>();
        if (ballMover != null)
        {
            Vector2 dragVector = CalculateDragVector();
            Vector2 kickPoint = CalculateKickPoint(dragVector);
            Vector3 initialVelocity = trajectoryCalculator.CalculateInitialVelocity(kickPoint, dragVector / maxDragDistance);
            ballMover.StartMoving(ball3D.position, initialVelocity, kickPoint);
        }

        GameManager.Instance.ExitKickMode();
    }

    private float CalculateMaxDragDistance()
    {
        float screenScaleFactor = (float)Screen.width / 1920f;
        return baseMaxDragDistance * screenScaleFactor;
    }

    private Vector2 CalculateDragVector()
    {
        Vector2 currentTouchPosition = Input.mousePosition;
        return currentTouchPosition - startTouchPosition;
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

    private Vector3 CalculateGroundEndPoint(Vector3 start, Vector2 dragVector)
    {
        return new Vector3(start.x + dragVector.x * dragScaleFactor, start.y, start.z + dragVector.y * dragScaleFactor);
    }

    private Vector3 CalculateRingCenter(Vector3 start, Vector3 end, float radius)
    {
        return start + (end - start).normalized * (Vector3.Distance(start, end) + radius) + Vector3.up * 0.05f;
    }

    private void UpdatePowerText(float powerRate, Vector3 ringCenter)
    {
        powerText.text = $"{powerRate * 100f:F0}%";
        powerText.transform.position = Camera.main.WorldToScreenPoint(ringCenter + Vector3.up * 0.32f);
    }

    private Vector2 CalculateKickPoint(Vector2 dragVector)
    {
        Vector2 ballCenter = ballRectTransform.position;
        Vector2 kickPoint = startTouchPosition - ballCenter;
        float dist = kickPoint.magnitude;
        float rate = dist / maxDragDistance;
        return kickPoint.normalized * rate;
    }

    private void DrawRing(Vector3 center, float radius, int segments = 100)
    {
        ringLineRenderer.loop = true;
        ringLineRenderer.positionCount = segments + 1;
        float angleStep = 360f / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            ringLineRenderer.SetPosition(i, new Vector3(center.x + x, center.y, center.z + z));
        }
    }

    private void DrawDashedLine(Vector3 start, Vector3 end)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

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