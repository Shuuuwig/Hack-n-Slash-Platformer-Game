using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    protected bool isHit;
    protected bool isStunned;
    protected bool isDead;

    protected EnemyMovement movement;
    protected EnemyCombat combat;
    protected EnemyStats stats;
    protected PlayerCombat hostileCombat;

    public bool IsHit {  get { return isHit; } }
    public bool IsStunned
    {
        get { return isStunned; }
        set { isStunned = value; }
    }

    public bool IsDead { get { return isDead; } }

    protected virtual void Start()
    {
        stats = GetComponent<EnemyStats>();
        movement = GetComponent<EnemyMovement>();
        combat = GetComponent<EnemyCombat>();
    }

    protected virtual void Update()
    {
        StateManager();
    }

    protected virtual void TakeDamage(float damageTaken)
    {
        stats.CurrentHealth -= damageTaken;

        if (stats.CurrentHealth <= 0)
        {
            isDead = true;
            movement.enabled = false;
            combat.enabled = false;
            Destroy(this.gameObject);
        }

        isHit = false;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        isHit = true;
        hostileCombat = collision.gameObject.GetComponentInParent<PlayerCombat>();

        if (collision.gameObject.CompareTag("Stun"))
        {
            isStunned = true;
            TakeDamage(hostileCombat.FinalizedDamage);
        }

    }

    protected virtual void StateManager()
    {
        if (isStunned)
        {
            //More later
        }
    }
}
