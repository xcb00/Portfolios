using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvent : AIAnimationEvent
{
    Enemy parent;
    private void Start()
    {
        parent = GetComponentInParent<Enemy>();
    }

    public void AttackEvent()
    {
        if(parent.ReachTarget(1.2f))
            EventHandler.CallAttackPlayerEvent(1);
    }
}
