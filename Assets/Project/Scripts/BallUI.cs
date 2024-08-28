using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallUI : MonoBehaviour
{
    float theta;
    [SerializeField] private RectTransform ballRectTransform;
    [SerializeField] private float animationDuration = 0.5f; // アニメーションの持続時間

    private void OnEnable()
    {
        theta = 0f;
        ballRectTransform.localScale = Vector3.zero;
        enabled = true;
    }

    public void Initialize()
    {
        theta = 0f;
        ballRectTransform.localScale = Vector3.zero;
        enabled = true;
    }

    private void Update()
    {
        theta += Time.deltaTime / animationDuration; // アニメーションの進行度を計算
        float scaleValue = Mathf.SmoothStep(0f, 1f, theta); // 0から1へ滑らかに変化

        ballRectTransform.localScale = new Vector3(scaleValue, scaleValue, 1f);

        // アニメーションが完了したら、Updateを停止する
        if (theta >= 1f)
        {
            enabled = false;
        }
    }
}
