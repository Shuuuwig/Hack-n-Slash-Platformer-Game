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
    [SerializeField] private Animator playerAnimator;


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
        playerAnimator.SetBool("isGrounded", playerMovement.IsGrounded);
        playerAnimator.SetBool("isFacingLeft", isFacingLeft);
        //playerAnimator.SetBool("isMovingRight", playerMovement.IsMovingRight);
        playerAnimator.SetBool("isJumping", playerMovement.IsJumping);
        playerAnimator.SetBool("isFalling", playerMovement.IsFalling);
        playerAnimator.SetBool("isClimbingWall", playerMovement.IsClimbingWall);
        playerAnimator.SetBool("isClimbingLedge", playerMovement.IsClimbingLedge);
        playerAnimator.SetBool("isHanging", playerMovement.IsHanging);
        playerAnimator.SetBool("isRunning", playerMovement.IsRunning);
        playerAnimator.SetBool("isSubmerged", playerMovement.IsSubmerged);
    }
}
