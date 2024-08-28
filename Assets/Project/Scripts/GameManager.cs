using UnityEngine;

public class GameManager : MonoBehaviour
{
    // シングルトンインスタンス
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        // シングルトンのインスタンスを設定
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // シーンをロードしてもオブジェクトを破壊しない
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EnterKickMode()
    {
        // 全てのキャラクターとボールの動きを停止
        foreach (PlayerController player in FindObjectsOfType<PlayerController>())
        {
            player.PauseMovement();
        }

        foreach (BallController ball in FindObjectsOfType<BallController>())
        {
            ball.PauseMovement();
        }

        // 必要なUIやサッカー場のアニメーションは動かし続ける
    }

    public void ExitKickMode()
    {
        // 全てのキャラクターとボールの動きを再開
        foreach (PlayerController player in FindObjectsOfType<PlayerController>())
        {
            player.ResumeMovement();
        }

        foreach (BallController ball in FindObjectsOfType<BallController>())
        {
            ball.ResumeMovement();
        }
    }
}
