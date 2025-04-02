using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombat : Combat
{
    ////Timer
    //[Header("---Timer Duration---")]

    //[SerializeField] private Cooldown comboActiveTimer;
    //[SerializeField] private Cooldown knockbackTimer;

    //[SerializeField] private Cooldown hitstopDuration;

    ////Player Component Reference
    //[Header("---Component Reference---")]
    //[SerializeField] private Collider2D neutralAttackCollider1;
    //[SerializeField] private Collider2D neutralAttackCollider2;
    //[SerializeField] private Collider2D neutralAttackCollider3;
    //[SerializeField] private Collider2D overheadAttackCollider;
    //[SerializeField] private Collider2D lowAttackCollider;
    //[SerializeField] private PlayerMovement playerMovement;
    //[SerializeField] private Animator animator;

    //private int comboTally;

    //Bools
    protected bool submergedNeutralAttack;
    protected bool submergedForwardAttack;
    protected bool airOverheadAttack;
    protected bool airLowAttack;    

    //protected bool hitObstacle;

    //public int ComboTally {  get { return comboTally; } }
    //public float WeaponDamage { get { return weaponDamage; } }
    //public bool FlipLocked { get { return flipLocked; } }
    //public bool HitEnemy {  get { return hitEnemy; } }
    //public bool HitObstacle { get {  return hitObstacle; } }
    //public bool SubmergedNeutralAttack { get { return submergedNeutralAttack; } }
    //public bool SubmergedForwardAttack { get { return submergedForwardAttack; } }
    //public bool NeutralAttack { get { return neutralAttack; } }
    //public bool AirOverheadAttack { get { return airOverheadAttack; } }
    //public bool AirLowAttack { get { return airLowAttack; } }
    //public bool Parrying { get { return isParrying; } }
    //public bool ParriedAttack { get { return parriedAttack; } }
    //public Collider2D NeutralAttackCollider1 
    //{ 
    //    get { return neutralAttackCollider1; }
    //    set
    //    {
    //        if (NeutralAttack == true)
    //        {
    //            neutralAttackCollider1 = value;
    //        }
    //    }  
    //}
    //public Collider2D NeutralAttackCollider2 
    //{ 
    //    get { return neutralAttackCollider2; }
    //    set
    //    {
    //        if (NeutralAttack == true)
    //        {
    //            neutralAttackCollider2 = value;
    //        }
    //    }
    //}
    //public Collider2D NeutralAttackCollider3 
    //{  
    //    get { return neutralAttackCollider3; }
    //    set
    //    {
    //        if (NeutralAttack == true)
    //        {
    //            neutralAttackCollider3 = value;
    //        }
    //    }
    //}

    protected override void Start()
    {
        if (stats == null)
        {
            stats = GetComponent<PlayerStats>();
        }

        if (movement == null)
        {
            movement = GetComponent<PlayerMovement>();
        }
    }
    protected override void Update()
    {
        //Basic Attacks
        //DirectionalAttack();
        //Parry();
        //HitStop();

        //AttackTally();
        DirectionLock();
    }

    protected override void UpdateMovementStates()
    {

    }

    protected override void DirectionLock()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            isDirectionLocked = true;
        }
        else
        {
            isDirectionLocked = false;
        }
    }

    ////==================== ATTACK TALLY ====================//
    //private void AttackTally()
    //{
    //    if (comboActiveTimer.CurrentProgress is Cooldown.Progress.Finished)
    //    {
    //        comboTally = 0;
    //        comboActiveTimer.ResetCooldown();
    //    }
    //}

    ////==================== GENERAL PLAYER COLLISION CHECKS ====================//
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Enemy"))
    //    {
    //        hitEnemy = true;
    //        knockbackTimer.StartCooldown();
    //    }

    //    if (collision.CompareTag("Obstacle"))
    //    {
    //        Debug.Log("Hit Obstacle");
    //        hitObstacle = true;
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Obstacle"))
    //    {
    //        Debug.Log("Hit Obstacle");
    //        hitObstacle = false;
    //    }
    //}

    //-------------------------------------------------------- COMBAT FUNCTIONS --------------------------------------------------------//
    //==================== DIRECTIONAL ATTACK ====================//
    //private void DirectionalAttack()
    //{
    //    if (Input.GetKeyDown(KeyCode.J) && attackCooldown.CurrentProgress is Cooldown.Progress.Ready && attackDuration.CurrentProgress is Cooldown.Progress.Ready)
    //    {
    //        //Start attack duration
    //        attackDuration.StartCooldown();

    //        //Check input direction to determine attack direction
    //        //Y input check
    //        if (playerMovement.IsGrounded == false)
    //        {
    //            if (playerMovement.InputDirection.y > 0)
    //            {
    //                airOverheadAttack = true;
    //                overheadAttackCollider.enabled = true;
    //            }
    //            else if (playerMovement.InputDirection.y < 0)
    //            {
    //                airLowAttack = true;
    //                lowAttackCollider.enabled = true;
    //            }
    //        }
    //        else if (playerMovement.IsSubmerged == true)
    //        {
    //            if (playerMovement.IsMovingForward == false)
    //            {
    //                submergedNeutralAttack = true;
    //            }
    //            else
    //            {
    //                submergedForwardAttack = true;
    //            }
    //        }
    //        else
    //        {
    //            neutralAttack = true;
    //            comboTally++;
    //            Debug.Log($"Combo No.{comboTally}");

    //            if (comboTally == 1)
    //            {
    //                neutralAttackCollider1.enabled = true;
    //            }
    //            else if (comboTally == 2)
    //            {
    //                neutralAttackCollider2.enabled = true;
    //            }
    //            else if (comboTally == 3)
    //            {
    //                neutralAttackCollider3.enabled = true;
    //            }

    //            if (comboActiveTimer.CurrentProgress is Cooldown.Progress.InProgress)
    //            {
    //                comboActiveTimer.ResetCooldown();
    //            }
    //        }
    //    }

    //Reset Hurtbox and bool
    //    if (attackDuration.CurrentProgress is Cooldown.Progress.Finished)
    //    {
    //        if (comboTally >= 3)
    //        {
    //            comboTally = 0;
    //            attackCooldown.StartCooldown();
    //        }

    //        if (comboActiveTimer.CurrentProgress is Cooldown.Progress.Ready)
    //        {
    //            comboActiveTimer.StartCooldown();
    //        }

    //        attackDuration.ResetCooldown();

    //        neutralAttack = false;
    //        airOverheadAttack = false;
    //        airLowAttack = false;
    //        submergedNeutralAttack = false;
    //        submergedForwardAttack = false;

    //        neutralAttackCollider1.enabled = false;
    //        neutralAttackCollider2.enabled = false;
    //        neutralAttackCollider3.enabled = false;

    //        overheadAttackCollider.enabled = false;
    //        lowAttackCollider.enabled = false;
    //    }

    //    //Reset cooldown
    //    if (attackCooldown.CurrentProgress is Cooldown.Progress.Finished)
    //    {
    //        attackCooldown.ResetCooldown();
    //    }
    //}

    //==================== PARRY ====================//
    protected override void Parry()
    {
        if (Input.GetKeyDown(KeyCode.F) && parryActiveTime.CurrentProgress is Cooldown.Progress.Ready)
        {
            parryActiveTime.StartCooldown();
        }
        base.Parry();
    }

    ////-------------------------------------------------------- COMBAT EFFECTS --------------------------------------------------------//
    ////==================== PARRY ====================//
    //private void HitStop()
    //{
    //    if (hitEnemy == false && parriedAttack == false)
    //        return;

    //    if (hitstopDuration.CurrentProgress is Cooldown.Progress.Ready)
    //    {
    //        hitstopDuration.StartCooldownRealtime();
    //        Time.timeScale = 0;
    //        Debug.Log("hitstopped");
    //    }
    //    if (hitstopDuration.CurrentProgress is Cooldown.Progress.Finished)
    //    {
    //        Time.timeScale = 1;
    //        hitstopDuration.ResetCooldown();
    //        Debug.Log("hitstop ended");
    //    }
    //}
}
