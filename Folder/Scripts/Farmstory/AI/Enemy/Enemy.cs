using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EnemyState
{
    [SerializeField] float enemyDelay = 0.3f;
    [SerializeField] int enemyHP = 5;
    public override void Respawn(Vector3 position)
    {
        delay = enemyDelay;
        hp = enemyHP;
        hitTime = -1f;
        attackTime = -1;
        base.Respawn(position);
    }
}
