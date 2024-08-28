using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MovableObject
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused) { return; }
    }
}
