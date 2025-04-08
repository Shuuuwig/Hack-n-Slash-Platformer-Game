using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats
{
    
    [SerializeField] private HealthBar healthbar;

    public float PlayerCurrentHealth
    {
        get { return currentHealth; }
        set 
        {

        }
    }
    //protected override void Start()
    //{
    //    healthbar.SetMaxHealth(maxHealth);

    //    skills.Add("SludgeProjectile", true);
    //}

    //void Update()
    //{
    //    UpdateHealthBar();
    //}

    //private void UpdateHealthBar()
    //{
    //    healthbar.SetHealthBar(currentHealth);
    //}

    
}
