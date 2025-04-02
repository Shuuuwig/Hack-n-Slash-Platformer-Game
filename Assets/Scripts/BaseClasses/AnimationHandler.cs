using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimationHandler : MonoBehaviour
{
    protected bool isFacingRight;
    protected bool isMovingForward;
    protected bool isFlipLocked;

    public bool IsFacingRight {  get { return isFacingRight; } }
    public bool IsMovingForward { get { return isMovingForward; } }

    protected Movement movement;
    protected Combat combat;

    protected virtual void Start()
    {
        if (movement == null)
        {
            movement = GetComponent<Movement>();
        }

        if (combat == null)
        {
            combat = GetComponent<Combat>();
        }
        
    }

    protected virtual void Update()
    {
        DirectionCheck();
    }

    protected virtual void DirectionCheck()
    {
        isFacingRight = transform.localScale.x > 0;
        isMovingForward = (movement.AttachedRigidBody.velocity.x > 0.1f && isFacingRight || movement.AttachedRigidBody.velocity.x < -0.1f && !isFacingRight);

        if (Mathf.Abs(movement.AttachedRigidBody.velocity.x) > 0.1f && !isMovingForward && !isFlipLocked)
            Flip();
    }

    protected virtual void Flip()
    {
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
    }
}
