using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float currentHealth;
    [SerializeField] protected float baseDamage;

    public float CurrentHealth {  get { return currentHealth; } }
    public float BaseDamage { get { return baseDamage; } }

                      //Ability //Unlocked
    protected Dictionary<string, bool> skills = new Dictionary<string, bool>();
    
    public Dictionary<string, bool> Skills { get { return skills; } }

    protected virtual void Start()
    {
     
    }

    protected virtual void Update()
    {

    }
}
