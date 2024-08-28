using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MovableObject
{
    private Animator animator;
    [SerializeField] private Transform kickingFootToe;
    [SerializeField] private LayerMask ballLayer;
    [SerializeField] private GameObject kickControl;

    private bool isKicked = false;

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponentInChildren<Animator>();
    }

    public override void PauseMovement()
    {
        base.PauseMovement();

        if (animator != null)
        {
            animator.speed = 0f;
        }
    }

    public override void ResumeMovement()
    {
        base.ResumeMovement();

        if (animator != null)
        {
            animator.speed = 1f;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GameManager.Instance.ExitKickMode();
        }

        if (isPaused) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("kick");
        }

        if (isKicked == false && Physics.CheckSphere(kickingFootToe.position, 0.1f, ballLayer))
        {
            OnPlayerContactBall();
            isKicked = true;
        }
    }

    public void OnPlayerContactBall()
    {
        GameManager.Instance.EnterKickMode();
        ShowKickUI();
    }

    private void ShowKickUI()
    {
        if (kickControl != null)
        {
            kickControl.SetActive(true);
        }
    }
}
