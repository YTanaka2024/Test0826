//using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;

public class KickControl : MonoBehaviour
{
    [Header("BallUI")]
    [SerializeField] private GameObject ballUI;                 // ボールと背景のUI
    [SerializeField] private float baseRadius = 400f;           // 基準解像度（1920px）の際のボールの半径
    [SerializeField] private RectTransform ballRectTransform;   // ボールの画像の位置

    [Header("ArrowUI")]
    [SerializeField] private GameObject arrowUI;

    // private variables
    private bool isDragging = false;
    private Vector2 startTouchPosition;
    private MouseInputManager mouseInputManager;
    private float theta;

    private void OnEnable()
    {
        isDragging = true;
    }

    private void Start()
    {
        mouseInputManager = GetComponent<MouseInputManager>();
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;

        // タッチ開始の検出
        if (isDragging == false &&
            Input.GetMouseButtonDown(0))
        {
            if (IsMouseInsideBall(mousePos, GetScaledRadius()))
            {
                startTouchPosition = Input.mousePosition;
                Debug.Log(mousePos);
                ballUI.SetActive(false);    // キックUIを非表示にする
                arrowUI.SetActive(true);    // 矢印UIを表示する
                isDragging = true;          // ひっぱり中である
            }
        }

        // ひっぱり中の処理
        if (isDragging)
        {
            Vector2 currentTouchPosition = Input.mousePosition;
            Vector2 dragVector = currentTouchPosition - startTouchPosition;
            UpdateArrow(dragVector);
        }

        // 離した瞬間の処理
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            arrowUI.SetActive(false);
        }
    }

    private void UpdateArrow(Vector2 dragVector)
    {
        //if (!arrowRectTransform.gameObject.activeSelf)
        //{
        //    arrowRectTransform.gameObject.SetActive(true); // 矢印を表示
        //}

        //// 矢印の回転とスケールを更新
        //float angle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;
        //arrowRectTransform.rotation = Quaternion.Euler(0, 0, angle);

        //float magnitude = dragVector.magnitude;
        //arrowRectTransform.sizeDelta = new Vector2(magnitude, arrowRectTransform.sizeDelta.y); // 矢印の長さを更新
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
