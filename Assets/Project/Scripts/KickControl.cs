//using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;

public class KickControl : MonoBehaviour
{
    [Header("BallUI")]
    [SerializeField] private GameObject ballUI;                 // �{�[���Ɣw�i��UI
    [SerializeField] private float baseRadius = 400f;           // ��𑜓x�i1920px�j�̍ۂ̃{�[���̔��a
    [SerializeField] private RectTransform ballRectTransform;   // �{�[���̉摜�̈ʒu

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

        // �^�b�`�J�n�̌��o
        if (isDragging == false &&
            Input.GetMouseButtonDown(0))
        {
            if (IsMouseInsideBall(mousePos, GetScaledRadius()))
            {
                startTouchPosition = Input.mousePosition;
                Debug.Log(mousePos);
                ballUI.SetActive(false);    // �L�b�NUI���\���ɂ���
                arrowUI.SetActive(true);    // ���UI��\������
                isDragging = true;          // �Ђ��ς蒆�ł���
            }
        }

        // �Ђ��ς蒆�̏���
        if (isDragging)
        {
            Vector2 currentTouchPosition = Input.mousePosition;
            Vector2 dragVector = currentTouchPosition - startTouchPosition;
            UpdateArrow(dragVector);
        }

        // �������u�Ԃ̏���
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
        //    arrowRectTransform.gameObject.SetActive(true); // ����\��
        //}

        //// ���̉�]�ƃX�P�[�����X�V
        //float angle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;
        //arrowRectTransform.rotation = Quaternion.Euler(0, 0, angle);

        //float magnitude = dragVector.magnitude;
        //arrowRectTransform.sizeDelta = new Vector2(magnitude, arrowRectTransform.sizeDelta.y); // ���̒������X�V
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
