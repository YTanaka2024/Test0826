using UnityEngine;

public class MouseInputManager : MonoBehaviour
{
    public Vector2 GetNormalizedMousePosition()
    {
        // マウスのスクリーン座標を取得 (ピクセル単位、左下が(0,0))
        Vector2 mousePosition = Input.mousePosition;

        // 画面中央を(0,0)とするようにオフセット
        float normalizedX = (mousePosition.x - Screen.width / 2) / (Screen.width / 2);
        float normalizedY = (mousePosition.y - Screen.height / 2) / (Screen.height / 2);

        // 正規化された座標を返す (-1,1)の範囲
        return new Vector2(normalizedX, normalizedY);
    }
}
