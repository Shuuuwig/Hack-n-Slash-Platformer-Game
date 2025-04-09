using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntingBladeEventHandler : MonoBehaviour
{
    [SerializeField] private HauntingBladeCombat combat;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void HauntingBladeSlash()
    {
        if (combat.SlashCollider.enabled == false)
        {
            combat.SlashCollider.enabled = true;
        }
        else
        {
            combat.SlashCollider.enabled = false;
        }
    }
}
