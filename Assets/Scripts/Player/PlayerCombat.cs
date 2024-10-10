using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("---Gizmo Configuration---")]
    [SerializeField] private bool gizmoToggleOn = true;

    [Header("---Main Configuration---")]
    [SerializeField] private float weaponDamage;
    [SerializeField] private float playerKnockbackPower;
    [SerializeField] private float playerKnockedbackPower;
    [SerializeField] private Vector2 parryBoxSize;
    [SerializeField] private Cooldown basicAttackDuration;
    [SerializeField] private Cooldown basicAttackCooldown;
    [SerializeField] private LayerMask parryableLayer;

    //Timer
    [Header("---Timer Duration---")]
    [SerializeField] private Cooldown knockbackTimer;
    [SerializeField] private Cooldown parryActiveTime;

    //Player Component Reference
    [Header("---Component Reference---")]
    [SerializeField] private Transform parryTransform;
    [SerializeField] private Collider2D basicAttackCollider;
    [SerializeField] private Collider2D airAttackCollider;
    [SerializeField] private Collider2D upwardAirAttackCollider;
    [SerializeField] private Collider2D downwardAirAttackCollider;
    [SerializeField] private PlayerMovement playerMovement;

    //Boolean conditions
    private bool isBasicAttacking;
    private bool downwardAttack;
    private bool hitEnemy;
    private bool hitObstacle;

    public float WeaponDamage { get { return weaponDamage; } }
    public bool HitEnemy {  get { return hitEnemy; } }
    public bool HitObstacle { get {  return hitObstacle; } }

    private void Update()
    {
        //Basic Attacks
        BasicAttack();
        DirectionalAttack();
        Parry();
    }

    //----------------Combat Functions------------------
    //Basic Attack
    private void BasicAttack()
    {
        if (playerMovement.IsGrounded != true)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0) && basicAttackCooldown.CurrentProgress is Cooldown.Progress.Ready)
        {
            //Start basic attack duration and cooldown
            basicAttackCooldown.StartCooldown();
            basicAttackDuration.StartCooldown();

            isBasicAttacking = true; //Bool for animator
            basicAttackCollider.enabled = true;
        }

        //Reset Hurtbox and bool
        if (basicAttackDuration.CurrentProgress is Cooldown.Progress.Finished)
        {
            basicAttackDuration.ResetCooldown();
            isBasicAttacking = false;
            basicAttackCollider.enabled = false;
        }
    }

    //Mid air 4-way directional attack
    private void DirectionalAttack()
    {
        if (playerMovement.IsGrounded == true)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0) && basicAttackCooldown.CurrentProgress is Cooldown.Progress.Ready)
        {
            basicAttackCooldown.StartCooldown();

            //Check input direction to determine attack direction
            //Y input check
            if (playerMovement.InputDirection.y > 0)
            {
                upwardAirAttackCollider.enabled = true;
            }
            else if (playerMovement.InputDirection.y < 0)
            {
                downwardAttack = true;
                downwardAirAttackCollider.enabled = true;
            }
            else
            {
                airAttackCollider.enabled = true;
            }
        }

        //Reset cooldown
        if (basicAttackCooldown.CurrentProgress is Cooldown.Progress.Finished)
        {

            downwardAttack = false;
            basicAttackCooldown.ResetCooldown();
            airAttackCollider.enabled = false;
            upwardAirAttackCollider.enabled = false;
            downwardAirAttackCollider.enabled = false;
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

        if (collision.CompareTag("Obstacle") && downwardAttack == true)
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
