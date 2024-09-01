using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
}