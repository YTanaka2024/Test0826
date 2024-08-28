using UnityEngine;

public abstract class MovableObject : MonoBehaviour
{
    protected Rigidbody rb;
    protected bool isPaused = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public virtual void PauseMovement()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }
        isPaused = true;
    }

    public virtual void ResumeMovement()
    {
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        isPaused = false;
    }
}
