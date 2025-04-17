using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParryTrigger : MonoBehaviour
{
    [SerializeField] protected PlayerCombat combat;
    [SerializeField] protected PlayerMovement movement;
    [SerializeField] protected PlayerStatus status;

    public Transform collisionPoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Parryable"))
        {
            collisionPoint = collision.gameObject.transform;
            combat.ParriedAttack = true;
            status.ParriedAttack = true;
        }
    }
}
