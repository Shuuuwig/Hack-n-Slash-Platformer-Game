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

    void Update()
    {
        
    }

    private void NeutralAttack()
    {
        if (playerCombat.NeutralAttack == true)
        {
            playerCombat.NeutralAttackCollider1.enabled = false;
            playerCombat.NeutralAttackCollider2.enabled = false;
            playerCombat.NeutralAttackCollider3.enabled = false;

            if (playerCombat.ComboTally == 1)
            {
                playerCombat.NeutralAttackCollider1.enabled = true;
            }
            else if (playerCombat.ComboTally == 2)
            {
                playerCombat.NeutralAttackCollider2.enabled = true;
            }
            else if (playerCombat.ComboTally == 3)
            {
                playerCombat.NeutralAttackCollider3.enabled = true;
            }
        }
    }

    private void SubmergedAttack()
    {

    }

    private void AirAttack()
    {

    }
}
