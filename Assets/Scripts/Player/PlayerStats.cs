using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    [SerializeField] private float currentHealth;
    [SerializeField] private float currentDamage;
    [SerializeField] private Cooldown InvulnerabilityDuration;

    private bool canTakeDamage = true;

    public float PlayerCurrentHealth
    {
        get { return currentHealth; }
        set 
        {
            if (canTakeDamage == true)
            {
                Debug.Log("Health change");
                currentHealth = value;
            }
        }
    }
    public float PlayerCurrentDamage { get { return currentDamage; } }
    void Start()
    {
        
    }

    void Update()
    {
        OnDeath();
        InvulnerabilityFrames();    
    }

    private void OnDeath()
    {
        if (currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void InvulnerabilityFrames()
    {
        if (canTakeDamage == true)
            return;

        if (InvulnerabilityDuration.CurrentProgress is Cooldown.Progress.Ready)
        {
            InvulnerabilityDuration.StartCooldown();
            Debug.Log("Currently Invulnerable");
        }

        if (InvulnerabilityDuration.CurrentProgress is Cooldown.Progress.Finished)
        {
            InvulnerabilityDuration.ResetCooldown();
            Debug.Log("No longer Invulnerable");
            canTakeDamage = true;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Obstacle"))
        {
            if (canTakeDamage == true)
            {
                canTakeDamage = false;
            }
        }
    }
}
