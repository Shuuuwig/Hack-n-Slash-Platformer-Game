using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("---Weapon configuration---")]
    [SerializeField] private float weaponDamage;
    [SerializeField] private Collider2D weaponHitbox;
    [SerializeField] private Cooldown basicAttackCooldown;

    //Boolean conditions
    private bool isBasicAttacking;

    private void Update()
    {
        BasicAttack();
    }

    //----------------Combat Functions------------------
    //Basic Attack
    private void BasicAttack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && basicAttackCooldown.CurrentProgress is Cooldown.Progress.Ready)
        {
            basicAttackCooldown.StartCooldown();
            isBasicAttacking = true; //Bool for animator
            weaponHitbox.enabled = true;
        }

        if (basicAttackCooldown.CurrentProgress is Cooldown.Progress.Finished)
        {
            basicAttackCooldown.ResetCooldown();
            isBasicAttacking = false;
            weaponHitbox.enabled = false;
        }
    }
}
