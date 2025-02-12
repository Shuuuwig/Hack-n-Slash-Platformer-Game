using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private string currentAnimation;

    //Player Component Reference
    [Header("---Component Reference---")]
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Animator playerAnimator;

    private void Start()
    {

    }

    private void Update()
    {
        HandleAnimation();
        Debug.Log($"Current animation: {currentAnimation}");
    }

    private void HandleAnimation()
    {
        if (playerMovement.IsJumping)
        {
            if (playerMovement.IsMovingForward)
            {
                ChangeAnimation("playerJumpForward");
            }
            else if (playerMovement.IsMovingBackward)
            {
                ChangeAnimation("playerJumpBackward");
            }
            else
            {
                ChangeAnimation("playerJumpNeutral");
            }
        }

        else if (playerMovement.IsSuperJumping)
        {
            if (playerMovement.IsMovingForward)
            {
                ChangeAnimation("playerSuperJumpForward");
            }
            else if (playerMovement.IsMovingBackward)
            {
                ChangeAnimation("playerSuperJumpBackward");
            }
            else
            {
                ChangeAnimation("playerSuperJumpNeutral");
            }
        }

        else if (playerMovement.IsSubmerged)
        {
            if (playerMovement.IsMovingForward)
            {
                ChangeAnimation("playerSubmergeMoveForward");
            }
            else if (playerMovement.IsMovingBackward)
            {
                ChangeAnimation("playerSubmergeMoveBackward");
            }
            else
            {
                ChangeAnimation("playerSubmergeNeutral");
            }
        }

        else if (playerMovement.IsDashingForward)
        {
            ChangeAnimation("playerDashForward");
        }
        else if (playerMovement.IsDashingBackward)
        {
            ChangeAnimation("playerDashBackward");
        }

        else if (playerMovement.IsFalling)
        {
            if (playerMovement.IsMovingForward)
            {
                ChangeAnimation("playerFallForward");
            }
            else if (playerMovement.IsMovingBackward)
            {
                ChangeAnimation("playerFallBackward");
            }
            else
            {
                ChangeAnimation("playerFallNeutral");
            }
        }

        else if (playerMovement.IsMovingForward)
        {
            ChangeAnimation("playerMoveForward");
        }
        else if (playerMovement.IsMovingBackward)
        {
            ChangeAnimation("playerMoveBackward");
        }

        else if (playerCombat.NeutralAttack == true && playerCombat.ComboTally == 1)
        {
            ChangeAnimation("playerNeutralAttackCombo1");
        }
        else if (playerCombat.NeutralAttack == true && playerCombat.ComboTally == 2)
        {
            ChangeAnimation("playerNeutralAttackCombo2");
        }
        else if (playerCombat.NeutralAttack == true && playerCombat.ComboTally == 3)
        {
            ChangeAnimation("playerNeutralAttackCombo3");
            Debug.Log("Final Combo");
        }

        else
        {
            ChangeAnimation("playerIdle");
        }
        



    }

    private void ChangeAnimation(string animation)
    {
        if (currentAnimation != animation)
        {
            currentAnimation = animation;
            playerAnimator.Play(animation);
        }
    }
}
