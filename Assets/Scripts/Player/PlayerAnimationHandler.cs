using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : AnimationHandler
{
    protected override Movement movement { get; set; }
    protected override Combat combat { get; set; }

    protected override void Start()
    {
        if (movement == null)
        {
            movement = GetComponent<PlayerMovement>();
        }
        if (movement == null)
        {
            Debug.Log("Component Movement not found");
        }

        if (combat == null)
        {
            combat = GetComponent<PlayerCombat>();
        }
    }

    protected override void DirectionCheck()
    {
        isFlipLocked = combat.IsDirectionLocked;
        base.DirectionCheck();
        
    }
}
