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
    [SerializeField] private Cooldown comboActiveTimer;
    [SerializeField] private Cooldown knockbackTimer;
    [SerializeField] private Cooldown parryActiveTime;
    [SerializeField] private Cooldown parrySuccessTime;
    [SerializeField] private Cooldown comboDuration;
    [SerializeField] private Cooldown hitstopDuration;

    //Player Component Reference
    [Header("---Component Reference---")]
    [SerializeField] private Transform parryTransform;
    [SerializeField] private Collider2D neutralAttackCollider;
    [SerializeField] private Collider2D overheadAttackCollider;
    [SerializeField] private Collider2D lowAttackCollider;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Animator animator;

    private int comboTally;

    //Boolean conditions
    private bool flipLocked;
    private bool isNeutralAttacking;
    private bool isOverheadAttacking;
    private bool isLowAttacking;
    private bool isParrying;
    private bool parriedAttack;
    private bool hitEnemy;
    private bool hitObstacle;

    public float WeaponDamage { get { return weaponDamage; } }
    public bool FlipLocked { get { return flipLocked; } }
    public bool HitEnemy {  get { return hitEnemy; } }
    public bool HitObstacle { get {  return hitObstacle; } }
    public bool NeutralAttack { get { return isNeutralAttacking; } }
    public bool OverheadAttack { get { return isOverheadAttacking; } }
    public bool LowAttack { get { return isLowAttacking; } }
    public bool Parrying { get { return isParrying; } }
    public bool ParriedAttack { get { return parriedAttack; } }
    

    private void Update()
    {
        //Basic Attacks
        DirectionalAttack();
        Parry();

        DirectionLock();
        AttackTally();
    }

    private void DirectionLock()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            flipLocked = true;
            Debug.Log("Flip Locked");
        }
        else
        {
            flipLocked = false;
        }
    }

    private void AttackTally()
    {
        if (hitEnemy == true)
        {
            comboTally++;
            comboDuration.StartCooldown();
        }

        if (comboDuration.CurrentProgress is Cooldown.Progress.Finished)
        {
            comboTally = 0;
            comboDuration.ResetCooldown();
        }
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
            else if (playerMovement.InputDirection.y < 0 && playerMovement.IsGrounded == false)
            {
                isLowAttacking = true;
                lowAttackCollider.enabled = true;
            }
            else
            {
                isNeutralAttacking = true;
                comboTally++;
                neutralAttackCollider.enabled = true;
                neutralAttackCollider.GetComponent<SpriteRenderer>().enabled = true;
            }
        }

        if (isNeutralAttacking == true)
        {
            if (comboActiveTimer.CurrentProgress is Cooldown.Progress.Ready || comboActiveTimer.CurrentProgress is Cooldown.Progress.InProgress)
            {
                comboActiveTimer.StartCooldown();
                Debug.Log(comboTally);
            }
        }
        if (comboActiveTimer.CurrentProgress is Cooldown.Progress.Finished)
        {
            comboActiveTimer.ResetCooldown();
            comboTally = 0;
            Debug.Log(comboTally);
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
            neutralAttackCollider.GetComponent<SpriteRenderer>().enabled = false;

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
        if (Input.GetKeyDown (KeyCode.F) && parryActiveTime.CurrentProgress is Cooldown.Progress.Ready)
        {
            parryActiveTime.StartCooldown();
            isParrying = true;
        }

        if (parryActiveTime.CurrentProgress is Cooldown.Progress.InProgress)
        {
            if (Physics2D.OverlapBox(parryTransform.position, parryBoxSize, 0, parryableLayer))
            {
                parriedAttack = true;
                Debug.Log("Parried");
            }
        }

        if (parriedAttack == true)
        {
            attackCooldown.ResetCooldown();
            parrySuccessTime.StartCooldown();
        }

        if (parrySuccessTime.CurrentProgress is Cooldown.Progress.Finished)
        {
            parriedAttack = false;
            parrySuccessTime.ResetCooldown();
        }

        //Reset time
        if (parryActiveTime.CurrentProgress is Cooldown.Progress.Finished)
        {
            parryActiveTime.ResetCooldown();
            isParrying = false;
        }
    }
    //-----------------------------------------------------------------
    //Combat Effects
    private void HitStop()
    {
        if (hitEnemy == false)
            return;

        if (hitstopDuration.CurrentProgress is Cooldown.Progress.Ready)
        {
            hitstopDuration.StartCooldownRealtime();
            Time.timeScale = 0;
            Debug.Log("hitstopped");
        }
        if (hitstopDuration.CurrentProgress is Cooldown.Progress.Finished)
        {
            Time.timeScale = 1;
            hitstopDuration.ResetCooldown();
            Debug.Log("hitstop ended");
            hitEnemy = false;
        }
    }

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
