using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "Game Data/EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
    public List<EnemyStat> listEnemy;
}
[Serializable]
public class EnemyStat
{
    public EnemyType type;
    public float speed;
    public float damage;
    public float attackRange;
    public float attackSpeed;
    public float maxHealth;
    public GameObject model;
    public int price;

    public EnemyStat(EnemyStat data) 
    {
        speed = data.speed;
        damage = data.damage;
        attackRange = data.attackRange;
        attackSpeed = data.attackSpeed;
        maxHealth = data.maxHealth;
        model = data.model;
        price = data.price;
    }
}