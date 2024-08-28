using UnityEngine;

public class GameManager : MonoBehaviour
{
    // �V���O���g���C���X�^���X
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        // �V���O���g���̃C���X�^���X��ݒ�
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // �V�[�������[�h���Ă��I�u�W�F�N�g��j�󂵂Ȃ�
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EnterKickMode()
    {
        // �S�ẴL�����N�^�[�ƃ{�[���̓������~
        foreach (PlayerController player in FindObjectsOfType<PlayerController>())
        {
            player.PauseMovement();
        }

        foreach (BallController ball in FindObjectsOfType<BallController>())
        {
            ball.PauseMovement();
        }

        // �K�v��UI��T�b�J�[��̃A�j���[�V�����͓�����������
    }

    public void ExitKickMode()
    {
        // �S�ẴL�����N�^�[�ƃ{�[���̓������ĊJ
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
