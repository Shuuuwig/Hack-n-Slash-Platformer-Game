using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private string currentAnimation;

    protected bool facingRight;
    protected bool facingLeft;
    protected bool movingForward;
    protected bool movingBackward;
    protected bool flipLocked;

    public bool FacingRight { get { return facingRight; } }
    public bool FacingLeft { get { return facingLeft; } }
    public bool MovingForward { get { return movingForward; } }
    public bool MovingBackward { get { return movingBackward; } }

    protected PlayerMovement movement;
    protected PlayerCombat combat;
    protected PlayerStatus status;
    [SerializeField] protected Animator animator;

    protected void Start()
    {
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
        status = GetComponent<PlayerStatus>();
        animator = transform.Find("Graphic").GetComponentInChildren<Animator>();
    }

    protected void Update()
    {
        DirectionCheck();
        MovementAnimation();
    }

    protected void DirectionCheck()
    {
        flipLocked = combat.IsDirectionLocked;

        facingRight = transform.localScale.x > 0;
        facingLeft = transform.localScale.x < 0;
        movingForward = ((movement.AttachedRigidBody.velocity.x > 0.1f && facingRight) || (movement.AttachedRigidBody.velocity.x < -0.1f && facingLeft));
        movingBackward = ((movement.AttachedRigidBody.velocity.x < -0.1f && facingRight) || (movement.AttachedRigidBody.velocity.x > 0.1f && facingLeft));

        if (Mathf.Abs(movement.AttachedRigidBody.velocity.x) > 0.1f && !movingForward && !flipLocked)
            Flip();
    }

    protected void Flip()
    {
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
    }

    protected void ChangeAnimation(string animation)
    {
        if (currentAnimation != animation)
        {
            //Debug.Log($"Changing animation from {currentAnimation} to {animation}");
            currentAnimation = animation;
            animator.Play(animation);
        }
    }

    protected void MovementAnimation()
    {
        if (status.IsDead)
        {
            ChangeAnimation("playerDead");
            return;
        }

        //Combat Animations
        if (combat.IsParry)
        {
            ChangeAnimation("playerParry");
        }
        else if (combat.IsAirLightHigh)
        {
            ChangeAnimation("playerAirLightHigh");
        }
        else if (combat.IsAirLightLow)
        {
            ChangeAnimation("playerAirLightLow");
        }
        else if (combat.IsAirLight)
        {
            ChangeAnimation("playerAirLight");
        }
        else if (combat.IsSubmergeLight)
        {
            ChangeAnimation("playerSubmergeLight");
        }
        else if (combat.IsForwardLight)
        {
            ChangeAnimation("playerForwardLight");
        }
        else if (combat.IsNeutralLight)
        {
            ChangeAnimation("playerLight");
        }
        else if (combat.IsDashLight)
        {
            ChangeAnimation("playerDashLight");
        }
        else if (combat.IsSludgeBomb)
        {
            ChangeAnimation("playerSludgeBomb");
        }

        //Movement
        else if (movement.JumpingForward)
        {
            ChangeAnimation("playerJumpForward");
        }
        else if (movement.JumpingBackward)
        {
            ChangeAnimation("playerJumpBackward");
        }
        else if (movement.Jumping)
        {
            ChangeAnimation("playerJump");
        }
        else if (movement.AirDashingForward)
        {
            ChangeAnimation("playerAirDashForward");
        }
        else if (movement.AirDashingBackward)
        {
            ChangeAnimation("playerAirDashBackward");
        }
        else if (movement.SubmergingDashingForward)
        {
            ChangeAnimation("playerDashForward");
        }
        else if (movement.SubmergingDashingBackward)
        {
            ChangeAnimation("playerDashBackward");
        }
        else if (movement.SubmergingForward)
        {
            ChangeAnimation("playerSubmergeForward");
        }
        else if (movement.SubmergingBackward)
        {
            ChangeAnimation("playerSubmergeBackward");
        }
        else if (movement.Submerging)
        {
            ChangeAnimation("playerSubmerge");
        }
        else if (movement.FallingForward)
        {
            ChangeAnimation("playerFallForward");
        }
        else if (movement.FallingBackward)
        {
            ChangeAnimation("playerFallBackward");
        }
        else if (movement.Falling)
        {
            ChangeAnimation("playerFall");
        }
        else if (movingForward)
        {
            ChangeAnimation("playerMoveForward");
        }
        else if (movingBackward)
        {
            ChangeAnimation("playerMoveBackward");
        }
        else
        {
            ChangeAnimation("playerIdle");
        }
    }
}
