using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntingBladeAnimationHandler : EnemyAnimationHandler
{
    protected override void Start()
    {
        base.Start();
        movement = GetComponent<HauntingBladeMovement>();
        combat = GetComponent<HauntingBladeCombat>();
    }

    protected override void Update()
    {
        flipLocked = combat.IsDirectionLocked;
        base.Update();
    }

    protected override void MovementAnimation()
    {
        if (((HauntingBladeMovement)movement).AwakingTime.CurrentProgress == Timer.Progress.InProgress)
        {
            ChangeAnimation("hauntingBladeAwake");
        }

        if (((HauntingBladeMovement)movement).AwakingTime.CurrentProgress != Timer.Progress.Finished)
            return;

        if (((HauntingBladeCombat)combat).NeutralSlash)
        {
            ChangeAnimation("hauntingBladeNeutralSlash");
        }

        else if (((HauntingBladeMovement)movement).WalkingForward)
        {
            ChangeAnimation("hauntingBladeWalkForward");
        }
        else if (((HauntingBladeMovement)movement).WalkingBackward)
        {
            ChangeAnimation("hauntingBladeWalkBackward");
        }

    }
}
