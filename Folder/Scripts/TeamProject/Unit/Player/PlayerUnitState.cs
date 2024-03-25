using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitState : PlayerUnitProperty
{
    protected override void MoveState()
    {
        base.MoveState();
        if (stateRoutine != null)
            StopCoroutine(stateRoutine);
        //stateRoutine = StartCoroutine(MovingForward());
    }

    protected override void AttackState()
    {
        transform.GetChild(0).transform.rotation = Quaternion.identity;
        base.AttackState();

        if (stateRoutine != null)
            StopCoroutine(stateRoutine);
        stateRoutine = StartCoroutine(Attacking());
    }

    /*protected override void HitState()
    {
        base.HitState();
        StartCoroutine(Hitting());
    }*/

    
}
