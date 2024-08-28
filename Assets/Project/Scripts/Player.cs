using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator Animator { get; private set; }
    [SerializeField] private Transform kickingFootToe;
    [SerializeField] private LayerMask ballLayer;

    private void Start()
    {
        Animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Animator.SetTrigger("kick");
        }

        if (Physics.CheckSphere(kickingFootToe.position, 0.1f, ballLayer))
        {
            Animator.speed = 0.0f;
        }
    }
}
