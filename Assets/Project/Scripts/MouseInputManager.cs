using UnityEngine;

public class MouseInputManager : MonoBehaviour
{
    public Vector2 GetNormalizedMousePosition()
    {
        // �}�E�X�̃X�N���[�����W���擾 (�s�N�Z���P�ʁA������(0,0))
        Vector2 mousePosition = Input.mousePosition;

        // ��ʒ�����(0,0)�Ƃ���悤�ɃI�t�Z�b�g
        float normalizedX = (mousePosition.x - Screen.width / 2) / (Screen.width / 2);
        float normalizedY = (mousePosition.y - Screen.height / 2) / (Screen.height / 2);

        // ���K�����ꂽ���W��Ԃ� (-1,1)�͈̔�
        return new Vector2(normalizedX, normalizedY);
    }
}
