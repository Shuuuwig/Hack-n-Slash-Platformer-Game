using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass
{
    private float health;
    private float damage;
    private float attackSpeed;
    private float movementSpeed;

    public float Health { get { return health; } set { health = value; } }
    public float Damage { get { return damage; } set { damage = value; } }
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    public float MovementSpeed { get { return movementSpeed; } set { attackSpeed = value; } }

    //General constructor for enemies
    public EnemyClass(float Health, float Damage, float AttackSpeed, float MovementSpeed)
    {
        health = Health;
        damage = Damage;
        attackSpeed = AttackSpeed;
        movementSpeed = MovementSpeed;
    }
}
