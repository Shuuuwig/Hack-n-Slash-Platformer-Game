using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventHandler : MonoBehaviour
{
    //Player Component Reference
    [Header("---Component Reference---")]
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerMovement playerMovement;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void NeutralSlash()
    {
        if (playerCombat.NeutralAttack == true)
        {
            if (playerCombat.ComboTally == 1)
            {
                
            }
            else if (playerCombat.ComboTally == 2)
            {

            }
            else if (playerCombat.ComboTally == 3)
            {

            }
        }
    }
}
