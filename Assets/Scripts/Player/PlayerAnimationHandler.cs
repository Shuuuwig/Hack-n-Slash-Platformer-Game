using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private bool isFacingLeft;
    [SerializeField] private Vector3 currentScale;

    //Player Component Reference
    [Header("---Component Reference---")]
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Animator playerUpperBodyAnimator;
    [SerializeField] private Animator playerLowerBodyAnimator;

    private void Update()
    {
        HandleFlip();
        HandleAnimation();
    }

    void HandleFlip()
    {
        if (playerMovement.IsMovingRight == true && isFacingLeft)
        {
            FlipCheck();
        }
        if (playerMovement.IsMovingRight == false && !isFacingLeft)
        {
            FlipCheck();
        }
    }

    void FlipCheck()
    {
        currentScale.x *= -1;
        transform.localScale = currentScale;
        isFacingLeft = !isFacingLeft;
    }

    private void HandleAnimation()
    {
        //Upper Body
        playerUpperBodyAnimator.SetBool("isGrounded", playerMovement.IsGrounded);
        playerUpperBodyAnimator.SetBool("isFacingLeft", isFacingLeft);
        //playerUpperBodyAnimator.SetBool("isMovingRight", playerMovement.IsMovingRight);
        playerUpperBodyAnimator.SetBool("isJumping", playerMovement.IsJumping);
        playerUpperBodyAnimator.SetBool("isFalling", playerMovement.IsFalling);
        playerUpperBodyAnimator.SetBool("isClimbingWall", playerMovement.IsClimbingWall);
        playerUpperBodyAnimator.SetBool("isClimbingLedge", playerMovement.IsClimbingLedge);
        playerUpperBodyAnimator.SetBool("isHanging", playerMovement.IsHanging);
        playerUpperBodyAnimator.SetBool("isRunning", playerMovement.IsRunning);
        playerUpperBodyAnimator.SetBool("isSliding", playerMovement.IsSliding);

        //Lower Body    
        playerLowerBodyAnimator.SetBool("isGrounded", playerMovement.IsGrounded);
        playerLowerBodyAnimator.SetBool("isFacingLeft", isFacingLeft);
        //playerLowerBodyAnimator.SetBool("isMovingRight", playerMovement.IsMovingRight);
        playerLowerBodyAnimator.SetBool("isJumping", playerMovement.IsJumping);
        playerLowerBodyAnimator.SetBool("isFalling", playerMovement.IsFalling);
        playerLowerBodyAnimator.SetBool("isClimbingWall", playerMovement.IsClimbingWall);
        playerLowerBodyAnimator.SetBool("isClimbingLedge", playerMovement.IsClimbingLedge);
        playerLowerBodyAnimator.SetBool("isHanging", playerMovement.IsHanging);
        playerLowerBodyAnimator.SetBool("isRunning", playerMovement.IsRunning);
        playerLowerBodyAnimator.SetBool("isSliding", playerMovement.IsSliding);
    }
}
