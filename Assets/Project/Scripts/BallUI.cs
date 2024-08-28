using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallUI : MonoBehaviour
{
    float theta;
    [SerializeField] private RectTransform ballRectTransform;
    [SerializeField] private float animationDuration = 0.5f; // �A�j���[�V�����̎�������

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
        theta += Time.deltaTime / animationDuration; // �A�j���[�V�����̐i�s�x���v�Z
        float scaleValue = Mathf.SmoothStep(0f, 1f, theta); // 0����1�֊��炩�ɕω�

        ballRectTransform.localScale = new Vector3(scaleValue, scaleValue, 1f);

        // �A�j���[�V����������������AUpdate���~����
        if (theta >= 1f)
        {
            enabled = false;
        }
    }
}
