using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicianUnit : AttackUnit
{
    public override void AttackEffect()
    {
        PoolManager.Instance.SpawnObject(PoolType.PlayerFireball, new Vector3(transform.position.x + 0.5f, 0.8f, 1.5f));
    }
}
