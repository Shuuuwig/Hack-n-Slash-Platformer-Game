using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float currentHealth;
    [SerializeField] protected float baseDamage;

                      //Ability //Unlocked
    protected Dictionary<string, bool> moveset = new Dictionary<string, bool>();
    
    public Dictionary<string, bool> Moveset { get { return moveset; } }

    protected virtual void Start()
    {
     
    }
}
