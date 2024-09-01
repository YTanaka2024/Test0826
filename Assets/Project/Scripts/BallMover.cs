using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMover : MovableObject
{
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public void StartMoving(Vector3 initialPos, Vector3 initialVel)
    {
    }

    private void Update()
    {
        if (isPaused) { return; }
    }
}

