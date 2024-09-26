using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("---Weapon configuration---")]
    [SerializeField] private float weaponDamage;
    [SerializeField] private float playerKnockbackPower;
    [SerializeField] private Collider2D weaponHitbox;
    [SerializeField] private Cooldown basicAttackDuration;
    [SerializeField] private Cooldown basicAttackCooldown;

    //Timer
    [Header("---Timer Duration---")]
    [SerializeField] private Cooldown knockbackTimer;

    //Player Component Reference
    [Header("---Component Reference---")]
    [SerializeField] private PlayerMovement playerMovement;

    //Boolean conditions
    private bool isBasicAttacking;
    private bool hitEnemy;

    public bool HitEnemy {  get { return hitEnemy; } }

    private void Update()
    {
        BasicAttack();
        //Debug.Log(hitEnemy);
    }

    //----------------Combat Functions------------------
    //Basic Attack
    private void BasicAttack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && basicAttackCooldown.CurrentProgress is Cooldown.Progress.Ready)
        {
            //Start basic attack duration and cooldown
            basicAttackCooldown.StartCooldown();
            basicAttackDuration.StartCooldown();

            isBasicAttacking = true; //Bool for animator
            weaponHitbox.enabled = true;
        }

        //Reset Hurtbox and bool
        if (basicAttackDuration.CurrentProgress is Cooldown.Progress.Finished)
        {
            basicAttackDuration.ResetCooldown();
            isBasicAttacking = false;
            weaponHitbox.enabled = false;
        }

        //Reset cooldown
        if (basicAttackCooldown.CurrentProgress is Cooldown.Progress.Finished)
        {
            basicAttackCooldown.ResetCooldown();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            hitEnemy = true;
            knockbackTimer.StartCooldown();
        }

        if (collision.CompareTag("Spike"))
        {
            //playerMovement.KnockedBackState()
        }
    }
}
