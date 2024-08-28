using UnityEngine;
using UnityEngine.UI;

public class KickControl : MonoBehaviour
{
    [SerializeField] private GameObject kickUI;
    [SerializeField] private RectTransform arrow; // 矢印UIをアタッチ
    [SerializeField] private Transform ball; // ボールのTransform
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
    }

    private bool IsMouseInsideBall(Vector2 normalizedMousePos)
    {
        // ボールの半径を考慮して、範囲内かどうかを判定
        float ballRadius = 256f / Screen.width; // ボールの半径を計算
        return normalizedMousePos.sqrMagnitude <= ballRadius * ballRadius;
    }

    void Update()
    {
        // タッチ開始の検出
        if (isDragging == false && Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
            Debug.Log(startTouchPosition);

            Vector2 normalizedMousePos = mouseInputManager.GetNormalizedMousePosition();

            if (IsMouseInsideBall(normalizedMousePos))
            {
                kickUI.SetActive(false); // キックUIを非表示にする
                isDragging = true;
            }
        }

        // ドラッグ中の処理
        if (isDragging)
        {
            Vector2 currentTouchPosition = Input.mousePosition;
            Vector2 dragVector = currentTouchPosition - startTouchPosition;
            UpdateArrow(dragVector); // 矢印の更新
        }

        // ドラッグ終了の検出
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            arrow.gameObject.SetActive(false); // 矢印を非表示
            // ボールを蹴る処理を追加する
        }
    }

    private void UpdateArrow(Vector2 dragVector)
    {
        if (!arrow.gameObject.activeSelf)
        {
            arrow.gameObject.SetActive(true); // 矢印を表示
        }

        // 矢印の回転とスケールを更新
        float angle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;
        arrow.rotation = Quaternion.Euler(0, 0, angle);

        float magnitude = dragVector.magnitude;
        arrow.sizeDelta = new Vector2(magnitude, arrow.sizeDelta.y); // 矢印の長さを更新
    }
}
