using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitEnemy : MonoBehaviour
{
    public bool HitEnemy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            HitEnemy = true;
        }
    }
}
