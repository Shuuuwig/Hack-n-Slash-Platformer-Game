using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("---Gizmo Configuration---")]
    [SerializeField] private bool gizmoToggleOn = true;

    [Header("===Combat Configuration===")]
    [SerializeField] private float weaponDamage;
    [SerializeField] private float playerKnockbackPower;
    [SerializeField] private float playerKnockedbackPower;
    [SerializeField] private Vector2 parryBoxSize;
    [SerializeField] private LayerMask parryableLayer;

    //Timer
    [Header("---Timer Duration---")]
    [SerializeField] private Cooldown attackDuration;
    [SerializeField] private Cooldown attackCooldown;
    [SerializeField] private Cooldown knockbackTimer;
    [SerializeField] private Cooldown parryActiveTime;

    //Player Component Reference
    [Header("---Component Reference---")]
    [SerializeField] private Transform parryTransform;
    [SerializeField] private Collider2D neutralAttackCollider;
    [SerializeField] private Collider2D overheadAttackCollider;
    [SerializeField] private Collider2D lowAttackCollider;
    [SerializeField] private PlayerMovement playerMovement;

    //Boolean conditions
    private bool isNeutralAttacking;
    private bool isOverheadAttacking;
    private bool isLowAttacking;
    private bool hitEnemy;
    private bool hitObstacle;

    public float WeaponDamage { get { return weaponDamage; } }
    public bool HitEnemy {  get { return hitEnemy; } }
    public bool HitObstacle { get {  return hitObstacle; } }
    public bool NeutralAttack { get { return isNeutralAttacking; } }
    public bool OverheadAttack { get { return isOverheadAttacking; } }
    public bool LowAttack { get { return isLowAttacking; } }
    

    private void Update()
    {
        //Basic Attacks
        DirectionalAttack();
        Parry();
    }

    //----------------Combat Functions------------------
    private void DirectionalAttack()
    {
        if (Input.GetKeyDown(KeyCode.J) && attackCooldown.CurrentProgress is Cooldown.Progress.Ready && attackDuration.CurrentProgress is Cooldown.Progress.Ready)
        {
            //Start attack duration
            attackDuration.StartCooldown();

            //Check input direction to determine attack direction
            //Y input check
            if (playerMovement.InputDirection.y > 0)
            {
                isOverheadAttacking = true;
                overheadAttackCollider.enabled = true;
            }
            else if (playerMovement.InputDirection.y < 0)
            {
                isLowAttacking = true;
                lowAttackCollider.enabled = true;
            }
            else
            {
                isNeutralAttacking = true;
                neutralAttackCollider.enabled = true;
            }
        }

        //Reset Hurtbox and bool
        if (attackDuration.CurrentProgress is Cooldown.Progress.Finished)
        {
            attackDuration.ResetCooldown();
            attackCooldown.StartCooldown();

            isNeutralAttacking = false;
            isOverheadAttacking = false;
            isLowAttacking = false;

            neutralAttackCollider.enabled = false;
            overheadAttackCollider.enabled = false;
            lowAttackCollider.enabled = false;
        }

        //Reset cooldown
        if (attackCooldown.CurrentProgress is Cooldown.Progress.Finished)
        {
            attackCooldown.ResetCooldown();
        }
    }

    private void Parry()
    {
        if (Input.GetKeyDown (KeyCode.F))
        {
            parryActiveTime.StartCooldown();
        }

        if (parryActiveTime.CurrentProgress is Cooldown.Progress.InProgress)
        {
            if (Physics2D.OverlapBox(parryTransform.position, parryBoxSize, 0, parryableLayer))
            {
                Debug.Log("Parried");
            }
        }

        //Reset time
        if (parryActiveTime.CurrentProgress is Cooldown.Progress.Finished)
        {
            parryActiveTime.ResetCooldown();
        }
    }
    //-----------------------------------------------------------------
    private void OnDrawGizmos()
    {
        if (gizmoToggleOn != true)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(parryTransform.position, parryBoxSize);
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            hitEnemy = true;
            knockbackTimer.StartCooldown();
        }

        if (collision.CompareTag("Obstacle"))
        {
            Debug.Log("Hit Obstacle");
            hitObstacle = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            Debug.Log("Hit Obstacle");
            hitObstacle = false;
        }
    }
}
