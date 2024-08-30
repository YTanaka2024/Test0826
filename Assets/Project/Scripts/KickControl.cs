using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KickControl : MonoBehaviour
{
    [Header("BallUI")]
    [SerializeField] private GameObject ballUI;                 // ボールと背景のUI
    [SerializeField] private float baseRadius = 400f;           // 基準解像度（1920px）の際のボールの半径
    [SerializeField] private RectTransform ballRectTransform;   // ボールの画像の位置

    [Header("ArrowUI")]
    [SerializeField] private GameObject arrowUI;                // 矢印UI

    [Header("Ball3D")]
    [SerializeField] private Transform ball3D;                  // 3Dのボール
    [SerializeField] private LayerMask groundLayer;             // 地面レイヤー
    [SerializeField] private LineRenderer lineRenderer;         // 地面に沿った点線を描画するLineRenderer
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

        // タッチ開始の検出
        if (!isDragging && Input.GetMouseButtonDown(0))
        {
            if (IsMouseInsideBall(mousePos, GetScaledRadius()))
            {
                startTouchPosition = Input.mousePosition;
                ballUI.SetActive(false);    // キックUIを非表示にする
                arrowUI.SetActive(true);    // 矢印UIを表示する
                lineRenderer.enabled = true;
                ringLineRenderer.enabled = true;
                isDragging = true;          // ひっぱり中である

                Cursor.visible = false;
                //Cursor.lockState = CursorLockMode.Locked;
            }
        }

        // ひっぱり中の処理
        if (isDragging)
        {
            Vector2 currentTouchPosition = Input.mousePosition;
            Vector2 dragVector = currentTouchPosition - startTouchPosition;

            // dragVectorの距離を制限
            float dragDistance = Mathf.Min(dragVector.magnitude, maxDragDistance);
            dragVector = dragVector.normalized * dragDistance;

            UpdateArrow(dragVector);

            // ボールの真下から地面に引っ張り具合を示す点線を描画
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

            // 力の割合を計算
            float powerPercentage = Mathf.Min(dragVector.magnitude / maxDragDistance, 1.0f) * 100f;
            // 力の割合をリングの横に表示
            powerText.text = $"{powerPercentage:F0}%";
            // 力の割合のテキストをリングの位置に合わせる（例）
            powerText.transform.position = Camera.main.WorldToScreenPoint(ringCenter + Vector3.up * 0.32f);

        }

        // 離した瞬間の処理
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            arrowUI.SetActive(false);        // 矢印UIを非表示にする
            lineRenderer.positionCount = 0;  // 点線をリセット
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
        return ball3D.position; // 地面が見つからない場合、ボールの中心位置を返す
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
        // 矢印の回転とスケールを更新する処理を追加
        // 例えば、矢印の向きや長さをdragVectorに基づいて設定
    }

    private float GetScaledRadius()
    {
        // 基準解像度(1920)に対する現在の画面幅の比率で半径をスケーリング
        float scaleFactor = (float)Screen.width / 1920f;
        return baseRadius * scaleFactor;
    }

    private bool IsMouseInsideBall(Vector2 mousePos, float radius)
    {
        // ローカルポイントの位置に基づきボール中心を算出 (これを基準にする)
        Vector2 ballCenter = ballRectTransform.position;

        // マウス位置とボール中心の距離を計算
        float distance = Vector2.Distance(mousePos, ballCenter);

        // 半径内かどうかを判定
        return distance <= radius;
    }
}
