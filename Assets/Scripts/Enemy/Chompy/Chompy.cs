using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chompy : EnemyClass
{
    [SerializeField] private Collider2D chompCollider;
    [SerializeField] private Cooldown chompAttackDuration;
    [SerializeField] private Cooldown chompAttackCooldown;

    private bool neutralSlash;

    public bool NeutralSlash { get { return neutralSlash; } }
    public Collider2D ChompCollider
    {
        get { return chompCollider; }
        set { chompCollider = value; }
    }

    protected override void EnemyMoveset()
    {
        if (canAttack == true && chompAttackDuration.CurrentProgress is Cooldown.Progress.Ready && chompAttackCooldown.CurrentProgress is Cooldown.Progress.Ready)
        {
            neutralSlash = true;
            chompAttackDuration.StartCooldown();
            Debug.Log("Slashing");
        }

        if (chompAttackDuration.CurrentProgress is Cooldown.Progress.Finished)
        {
            neutralSlash = false;
            chompAttackCooldown.StartCooldown();
            chompAttackDuration.ResetCooldown();
        }

        if (chompAttackCooldown.CurrentProgress is Cooldown.Progress.Finished)
        {
            chompAttackCooldown.ResetCooldown();
        }
    }

    protected override void EnemyMovement()
    {
        base.EnemyMovement();

        enemyRigidbody.velocity = new Vector2((target.position.x - transform.position.x) * speed, 0);
    }
}
