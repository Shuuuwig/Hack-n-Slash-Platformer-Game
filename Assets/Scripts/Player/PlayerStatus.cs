using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("--- Knockback Effect ---")]
    [SerializeField] protected float knockedbackForce;
    [SerializeField] protected Timer knockedbackTimer;

    [SerializeField] protected Timer invulnerabilityDuration;

    private bool isProcessingHit;
    protected bool isHit;
    protected bool isKnockedback;
    protected bool isSlowed;
    protected bool isStunned;
    protected bool isParalyzed;
    protected bool isBurned;
    protected bool isDead;

    protected bool parriedAttack;
    protected bool knockbackEnd;

    protected Vector2 collisionPoint;

    private PlayerMovement movement;
    private PlayerCombat combat;
    private PlayerInputTracker inputTracker;
    private PlayerStats stats;
    private EnemyCombat hostileCombat;

    public float KnockedbackForce { get { return knockedbackForce; } }
    public Timer KnockedbackTimer { get { return knockedbackTimer; } }

    public bool IsHit {  get { return isHit; } }
    public bool IsKnockedback
    {
        get { return isKnockedback; }
        set { isKnockedback = value; }
    }
    public bool IsSlowed
    {
        get { return isSlowed; }
        set { isSlowed = value; }
    }
    public bool IsStunned
    {
        get { return isStunned; }
        set { isStunned = value; }
    }
    public bool IsParalyzed
    {
        get { return isParalyzed; }
        set { isParalyzed = value; }
    }
    public bool IsBurned
    {
        get { return isBurned; }
        set { isBurned = value; }
    }

    public bool IsDead { get { return isDead; } }
    public bool ParriedAttack 
    {
        get { return parriedAttack; }
        set { parriedAttack = value; }
    }

    public bool KnockbackEnd { get { return knockbackEnd; } }

    public Vector2 CollisionPoint { get { return collisionPoint; } }

    private void Start()
    {
        stats = GetComponent<PlayerStats>();
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
        inputTracker = GetComponent<PlayerInputTracker>();
    }

    private void Update()
    {
        InvulnerabilityFrames();
        StateManager();
        
    }

    private void TakeDamage(float damageTaken)
    {
        stats.CurrentHealth -= damageTaken;
        invulnerabilityDuration.StartCooldown();

        if (stats.CurrentHealth <= 0)
        {
            isDead = true;
            movement.enabled = false;
            combat.enabled = false;
            inputTracker.enabled = false;
        }

        isHit = false;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        isHit = true;
        hostileCombat = collision.gameObject.GetComponentInParent<EnemyCombat>();

        
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collisionPoint = collision.gameObject.transform.position;
            TakeDamage(hostileCombat.BodyDamage);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isProcessingHit) 
            return;

        isHit = true;
        hostileCombat = collision.gameObject.GetComponentInParent<EnemyCombat>();

        StartCoroutine(WaitForParry(collision, hostileCombat));

    }

    private void InvulnerabilityFrames()
    {
        if (invulnerabilityDuration.CurrentProgress == Timer.Progress.InProgress)
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
            invulnerabilityDuration.ResetCooldown();
        }

    }
    
    private void StateManager()
    {
        if (isKnockedback && knockedbackTimer.CurrentProgress == Timer.Progress.Ready)
        {
            knockedbackTimer.StartCooldown();
            knockbackEnd = true;
        }
        else if (knockedbackTimer.CurrentProgress == Timer.Progress.Finished)
        {
            IsKnockedback = false;
            knockbackEnd = true;
            knockedbackTimer.ResetCooldown();
        }
    }

    protected IEnumerator WaitForParry(Collider2D collision, EnemyCombat enemyCombat)
    {
        isProcessingHit = true;

        yield return new WaitForSeconds(0.05f);
        Debug.Log(parriedAttack);
        if (parriedAttack)
        {
            parriedAttack = false;
            isProcessingHit = false;
            yield break;
        }

        if (collision.gameObject.CompareTag("Knockback"))
        {
            isKnockedback = true;
            knockedbackForce = enemyCombat.FinalizedKnockback;
            collisionPoint = collision.gameObject.transform.position;
            TakeDamage(enemyCombat.FinalizedDamage);
        }

        if (collision.gameObject.CompareTag("Slow"))
        {
            isSlowed = true;
        }

        if (collision.gameObject.CompareTag("Stun"))
        {
            isStunned = true;
        }

        if (collision.gameObject.CompareTag("Paralyze"))
        {
            isParalyzed = true;
        }

        if (collision.gameObject.CompareTag("Burn"))
        {
            isBurned = true;
        }

        isProcessingHit = false;
    }
}
