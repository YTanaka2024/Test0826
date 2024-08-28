using UnityEngine;
using UnityEngine.UI;

public class KickControl : MonoBehaviour
{
    [SerializeField] private GameObject kickUI;
    [SerializeField] private RectTransform arrow; // ���UI���A�^�b�`
    [SerializeField] private Transform ball; // �{�[����Transform
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
        // �{�[���̔��a���l�����āA�͈͓����ǂ����𔻒�
        float ballRadius = 256f / Screen.width; // �{�[���̔��a���v�Z
        return normalizedMousePos.sqrMagnitude <= ballRadius * ballRadius;
    }

    void Update()
    {
        // �^�b�`�J�n�̌��o
        if (isDragging == false && Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
            Debug.Log(startTouchPosition);

            Vector2 normalizedMousePos = mouseInputManager.GetNormalizedMousePosition();

            if (IsMouseInsideBall(normalizedMousePos))
            {
                kickUI.SetActive(false); // �L�b�NUI���\���ɂ���
                isDragging = true;
            }
        }

        // �h���b�O���̏���
        if (isDragging)
        {
            Vector2 currentTouchPosition = Input.mousePosition;
            Vector2 dragVector = currentTouchPosition - startTouchPosition;
            UpdateArrow(dragVector); // ���̍X�V
        }

        // �h���b�O�I���̌��o
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            arrow.gameObject.SetActive(false); // �����\��
            // �{�[�����R�鏈����ǉ�����
        }
    }

    private void UpdateArrow(Vector2 dragVector)
    {
        if (!arrow.gameObject.activeSelf)
        {
            arrow.gameObject.SetActive(true); // ����\��
        }

        // ���̉�]�ƃX�P�[�����X�V
        float angle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;
        arrow.rotation = Quaternion.Euler(0, 0, angle);

        float magnitude = dragVector.magnitude;
        arrow.sizeDelta = new Vector2(magnitude, arrow.sizeDelta.y); // ���̒������X�V
    }
}
