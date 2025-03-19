using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntingBladeEventHandler : MonoBehaviour
{
    [SerializeField] private HauntingBlade HauntingBlade;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void HauntingBladeSlash()
    {
        if (HauntingBlade.SlashCollider.enabled == false)
        {
            HauntingBlade.SlashCollider.enabled = true;
        }
        else
        {
            HauntingBlade.SlashCollider.enabled = false;
        }
    }
}
