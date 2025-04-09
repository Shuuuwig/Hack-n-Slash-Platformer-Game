using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntingBladeAnimationHandler : AnimationHandler
{
    //[SerializeField] private HauntingBlade hauntingBlade;
    protected override void Start()
    {
        base.Start();
        movement = GetComponent<HauntingBladeMovement>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base .Update();
    }

    protected override void MovementAnimation()
    {
        if (((HauntingBladeMovement)movement).AwakingTime.CurrentProgress == Timer.Progress.InProgress)
        {
            ChangeAnimation("hauntingBladeAwake");
        }

        if (((HauntingBladeMovement)movement).AwakingTime.CurrentProgress != Timer.Progress.Finished)
            return;

        if (((HauntingBladeMovement)movement).MovingForward)
        {
            ChangeAnimation("hauntingBladeMoveForward");
        }
    }
}
