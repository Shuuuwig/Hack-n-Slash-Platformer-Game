using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : AnimationHandler
{
    protected override void Start()
    {
        base.Start();

        if (movement == null)
        {
            movement = GetComponent<PlayerMovement>();
        }

        if (combat == null)
        {
            combat = GetComponent<PlayerCombat>();
        }
    }

    protected override void DirectionCheck()
    {
        flipLocked = combat.IsDirectionLocked;
        base.DirectionCheck();
    }

    protected override void MovementAnimation()
    {
        // Combat Animations

        if (((PlayerCombat)combat).IsParry)
        {
            ChangeAnimation("playerParry");
        }
        else if (((PlayerCombat)combat).IsAirLightHigh)
        {
            ChangeAnimation("playerAirLightHigh");
        }
        else if (((PlayerCombat)combat).IsAirLightLow)
        {
            ChangeAnimation("playerAirLightLow");
        }
        else if (((PlayerCombat)combat).IsAirLight)
        {
            ChangeAnimation("playerAirLight");
        }
        else if (((PlayerCombat)combat).IsSubmergeLight)
        {
            ChangeAnimation("playerSubmergeLight");
        }
        else if (((PlayerCombat)combat).IsForwardLight)
        {
            ChangeAnimation("playerForwardLight");
        }
        else if (((PlayerCombat)combat).IsNeutralLight)
        {
            ChangeAnimation("playerLight");
        }
        else if (((PlayerCombat)combat).IsDashLight)
        {
            ChangeAnimation("playerDashLight");
        }
        else if (((PlayerCombat)combat).IsSludgeBomb)
        {
            ChangeAnimation("playerSludgeBomb");
        }

        // Movement Animations 

        else if (((PlayerMovement)movement).JumpingForward)
        {
            ChangeAnimation("playerJumpForward");
        }
        else if (((PlayerMovement)movement).JumpingBackward)
        {
            ChangeAnimation("playerJumpBackward");
        }
        else if (((PlayerMovement)movement).Jumping)
        {
            ChangeAnimation("playerJump");
        }
        else if (((PlayerMovement)movement).AirDashingForward)
        {
            ChangeAnimation("playerAirDashForward");
        }
        else if (((PlayerMovement)movement).AirDashingBackward)
        {
            ChangeAnimation("playerAirDashBackward");
        }
        else if (((PlayerMovement)movement).DashingForward)
        {
            ChangeAnimation("playerDashForward");
        }
        else if (((PlayerMovement)movement).DashingBackward)
        {
            ChangeAnimation("playerDashBackward");
        }
        else if (((PlayerMovement)movement).SubmergingForward)
        {
            ChangeAnimation("playerSubmergeForward");
        }
        else if (((PlayerMovement)movement).SubmergingBackward)
        {
            ChangeAnimation("playerSubmergeBackward");
        }
        else if (((PlayerMovement)movement).Submerging)
        {
            ChangeAnimation("playerSubmerge");
        }
        else if (((PlayerMovement)movement).Grappling)
        {
            ChangeAnimation("playerGrapple");
        }
        else if (((PlayerMovement)movement).FallingForward)
        {
            ChangeAnimation("playerFallForward");
        }
        else if (((PlayerMovement)movement).FallingBackward)
        {
            ChangeAnimation("playerFallBackward");
        }
        else if (((PlayerMovement)movement).Falling)
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
