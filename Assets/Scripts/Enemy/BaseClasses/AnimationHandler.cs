using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAnimationHandler : MonoBehaviour
{
    private string currentAnimation;

    protected bool facingRight;
    protected bool facingLeft;
    protected bool movingForward;
    protected bool movingBackward;
    protected bool flipLocked;

    public bool FacingRight {  get { return facingRight; } }
    public bool FacingLeft { get { return facingLeft; } }
    public bool MovingForward { get { return movingForward; } }
    public bool MovingBackward {  get { return movingBackward; } }

    protected EnemyMovement movement;
    protected EnemyCombat combat;
    [SerializeField] protected Animator animator;

    protected virtual void Start()
    {
        if (animator == null)
        {
            animator = transform.Find("Graphic").GetComponentInChildren<Animator>();
        }

    }

    protected virtual void Update()
    {
        DirectionCheck();
        MovementAnimation();
    }

    protected virtual void DirectionCheck()
    {
        facingRight = transform.localScale.x > 0;
        facingLeft = transform.localScale.x < 0;
        movingForward = ((movement.AttachedRigidBody.velocity.x > 0.1f && facingRight) || (movement.AttachedRigidBody.velocity.x < -0.1f && facingLeft));
        movingBackward = ((movement.AttachedRigidBody.velocity.x < -0.1f && facingRight) || (movement.AttachedRigidBody.velocity.x > 0.1f && facingLeft));

        if (Mathf.Abs(movement.AttachedRigidBody.velocity.x) > 0.1f && !movingForward && !flipLocked)
            Flip();
    }

    protected virtual void Flip()
    {
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
    }

    protected virtual void ChangeAnimation(string animation)
    {
        if (currentAnimation != animation)
        {
            //Debug.Log($"Changing animation from {currentAnimation} to {animation}");
            currentAnimation = animation;
            animator.Play(animation);
        }
    }

    protected abstract void MovementAnimation();
}
