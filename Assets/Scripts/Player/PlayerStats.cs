using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private HealthBar healthbar;
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float baseJumpPower;
    [SerializeField] private float lightDamage;

    //Ability //Unlocked
    private Dictionary<string, bool> skills = new Dictionary<string, bool>()
    {
        {"SludgeBomb", false}
    };

    private PlayerStatus status;

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
    public float BaseSpeed {  get { return baseSpeed; } }
    public float BaseJumpPower { get { return baseJumpPower; } }
    public float LightDamage { get { return lightDamage; } }

    public Dictionary<string, bool> Skills { get { return skills; } }

    protected void Start()
    {
        status = GetComponent<PlayerStatus>();
        healthbar.SetMaxHealth(maxHealth);
    }

    protected void Update()
    {
        UpdateHealthBar();
    }

    

    private void UpdateHealthBar()
    {
        healthbar.SetHealthBar(currentHealth);
    }


}
