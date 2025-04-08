using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;

public class HauntingBlade : EnemyClass
{
    [SerializeField] private Collider2D slashCollider;
    [SerializeField] private Timer slashAttackDuration;
    [SerializeField] private Timer slashAttackCooldown;

    private bool neutralSlash;

    public bool NeutralSlash { get { return neutralSlash; } }
    public Collider2D SlashCollider
    {
        get { return slashCollider; }
        set { slashCollider = value; }
    }

    protected override void EnemyMoveset()
    {
        if (canAttack == true && slashAttackDuration.CurrentProgress is Timer.Progress.Ready && slashAttackCooldown.CurrentProgress is Timer.Progress.Ready)
        {
            neutralSlash = true;
            slashAttackDuration.StartCooldown();
            Debug.Log("Slashing");
        }

        if (slashAttackDuration.CurrentProgress is Timer.Progress.Finished)
        {
            neutralSlash = false;
            slashAttackCooldown.StartCooldown();
            slashAttackDuration.ResetCooldown();
        }

        if (slashAttackCooldown.CurrentProgress is Timer.Progress.Finished)
        {
            slashAttackCooldown.ResetCooldown();
        }
    }

    protected override void EnemyMovement()
    {
        base.EnemyMovement();

        
    }
}
