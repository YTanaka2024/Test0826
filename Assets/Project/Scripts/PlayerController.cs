using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MovableObject
{
    public Animator Animator { get; private set; }
    [SerializeField] private Transform kickingFootToe;
    [SerializeField] private LayerMask ballLayer;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void PauseMovement()
    {
        base.PauseMovement();

        Animator.speed = 0f;
    }

    public override void ResumeMovement()
    {
        base.ResumeMovement();

        Animator.speed = 1f;
    }

    private void Start()
    {
        Animator = GetComponentInChildren<Animator>();
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
            Animator.SetTrigger("kick");
        }

        if (Physics.CheckSphere(kickingFootToe.position, 0.1f, ballLayer))
        {
            GameManager.Instance.EnterKickMode();
        }
    }
}
