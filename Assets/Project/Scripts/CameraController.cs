using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 position;
    [SerializeField] private Transform parent;

    private void Start()
    {
        position = transform.localPosition;
    }

    private void Update()
    {
        transform.position = parent.position + position;
    }
}
