using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float currentHealth;
    [SerializeField] protected float baseSpeed;
    [SerializeField] protected float baseDamage;
    [SerializeField] protected float bodyDamage;
    [SerializeField] protected float baseKnockback;

    public float CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            if (status.IsHit)
            {
                currentHealth = Mathf.Clamp(value, 0, maxHealth);
            }
        }
    }
    public float BaseSpeed { get { return baseSpeed; } }
    public float BaseDamage { get { return baseDamage; } }
    public float BodyDamage { get { return bodyDamage; } }
    public float BaseKnockback {  get { return baseKnockback; } }

                      //Ability //Unlocked
    protected Dictionary<string, bool> skills = new Dictionary<string, bool>();
    
    public Dictionary<string, bool> Skills { get { return skills; } }

    protected EnemyStatus status;


    protected virtual void Start()
    {
       
    }

    protected virtual void Update()
    {

    }
}
