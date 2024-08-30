using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KickControl : MonoBehaviour
{
    [Header("BallUI")]
    [SerializeField] private GameObject ballUI;                 // �{�[���Ɣw�i��UI
    [SerializeField] private float baseRadius = 400f;           // ��𑜓x�i1920px�j�̍ۂ̃{�[���̔��a
    [SerializeField] private RectTransform ballRectTransform;   // �{�[���̉摜�̈ʒu

    [Header("ArrowUI")]
    [SerializeField] private GameObject arrowUI;                // ���UI

    [Header("Ball3D")]
    [SerializeField] private Transform ball3D;                  // 3D�̃{�[��
    [SerializeField] private LayerMask groundLayer;             // �n�ʃ��C���[
    [SerializeField] private LineRenderer lineRenderer;         // �n�ʂɉ������_����`�悷��LineRenderer
    [SerializeField] private LineRenderer ringLineRenderer;
    [SerializeField] private float maxDragDistance = 100f;
    [SerializeField] private float baseMaxDragDistance = 100f;
    [SerializeField] private float dragScaleFactor = 0.01f;
    [SerializeField] private TextMeshProUGUI powerText;

    // private variables
    private bool isDragging = false;
    private Vector2 startTouchPosition;
    private MouseInputManager mouseInputManager;
    private float theta;

    private void OnEnable()
    {
        isDragging = false;
    }

    private void Start()
    {
        mouseInputManager = GetComponent<MouseInputManager>();
        float screenScaleFactor = (float)Screen.width / 1920f;
        maxDragDistance = baseMaxDragDistance * screenScaleFactor;
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;

        // �^�b�`�J�n�̌��o
        if (!isDragging && Input.GetMouseButtonDown(0))
        {
            if (IsMouseInsideBall(mousePos, GetScaledRadius()))
            {
                startTouchPosition = Input.mousePosition;
                ballUI.SetActive(false);    // �L�b�NUI���\���ɂ���
                arrowUI.SetActive(true);    // ���UI��\������
                lineRenderer.enabled = true;
                ringLineRenderer.enabled = true;
                isDragging = true;          // �Ђ��ς蒆�ł���

                Cursor.visible = false;
                //Cursor.lockState = CursorLockMode.Locked;
            }
        }

        // �Ђ��ς蒆�̏���
        if (isDragging)
        {
            Vector2 currentTouchPosition = Input.mousePosition;
            Vector2 dragVector = currentTouchPosition - startTouchPosition;

            // dragVector�̋����𐧌�
            float dragDistance = Mathf.Min(dragVector.magnitude, maxDragDistance);
            dragVector = dragVector.normalized * dragDistance;

            UpdateArrow(dragVector);

            // �{�[���̐^������n�ʂɈ��������������_����`��
            Vector3 groundStartPoint = GetGroundPointBelowBall();
            groundStartPoint += Vector3.up * 0.05f;
            Vector3 groundEndPoint = ScreenToWorldPointOnGround(groundStartPoint, dragVector * dragScaleFactor);
            groundEndPoint += Vector3.up * 0.05f;
            DrawDashedLine(groundStartPoint, groundEndPoint);

            float length = (groundEndPoint - groundStartPoint).magnitude;
            float radius = 0.2f;
            Vector3 ringCenter = groundStartPoint + (groundEndPoint - groundStartPoint).normalized * (length + radius);
            ringCenter += Vector3.up * 0.05f;
            DrawRing(ringCenter, radius);

            // �͂̊������v�Z
            float powerPercentage = Mathf.Min(dragVector.magnitude / maxDragDistance, 1.0f) * 100f;
            // �͂̊����������O�̉��ɕ\��
            powerText.text = $"{powerPercentage:F0}%";
            // �͂̊����̃e�L�X�g�������O�̈ʒu�ɍ��킹��i��j
            powerText.transform.position = Camera.main.WorldToScreenPoint(ringCenter + Vector3.up * 0.32f);

        }

        // �������u�Ԃ̏���
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            arrowUI.SetActive(false);        // ���UI���\���ɂ���
            lineRenderer.positionCount = 0;  // �_�������Z�b�g
            lineRenderer.enabled = false;
            ringLineRenderer.positionCount = 0;
            ringLineRenderer.enabled = false;

            Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
        }
    }

    private Vector3 GetGroundPointBelowBall()
    {
        Ray ray = new Ray(ball3D.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            return hit.point;
        }
        return ball3D.position; // �n�ʂ�������Ȃ��ꍇ�A�{�[���̒��S�ʒu��Ԃ�
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
            Vector3 position = new Vector3(center.x + x, center.y, center.z + z);
            ringLineRenderer.SetPosition(i, position);
        }
    }

    private void OnDrawGizmos()
    {
        Ray ray = new Ray(ball3D.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(hit.point, 0.05f);
        }
    }

    private Vector3 ScreenToWorldPointOnGround(Vector3 groundStartPoint, Vector2 dragVector)
    {
        return new Vector3(groundStartPoint.x + dragVector.x, 
            groundStartPoint.y, 
            groundStartPoint.z + dragVector.y);
    }

    private void DrawDashedLine(Vector3 start, Vector3 end)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    private void UpdateArrow(Vector2 dragVector)
    {
        // ���̉�]�ƃX�P�[�����X�V���鏈����ǉ�
        // �Ⴆ�΁A���̌����Ⓑ����dragVector�Ɋ�Â��Đݒ�
    }

    private float GetScaledRadius()
    {
        // ��𑜓x(1920)�ɑ΂��錻�݂̉�ʕ��̔䗦�Ŕ��a���X�P�[�����O
        float scaleFactor = (float)Screen.width / 1920f;
        return baseRadius * scaleFactor;
    }

    private bool IsMouseInsideBall(Vector2 mousePos, float radius)
    {
        // ���[�J���|�C���g�̈ʒu�Ɋ�Â��{�[�����S���Z�o (�������ɂ���)
        Vector2 ballCenter = ballRectTransform.position;

        // �}�E�X�ʒu�ƃ{�[�����S�̋������v�Z
        float distance = Vector2.Distance(mousePos, ballCenter);

        // ���a�����ǂ����𔻒�
        return distance <= radius;
    }
}
