using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComState : ComProperty
{
    protected override void MoveState()
    {
        base.MoveState();
        if(stateRoutine!=null)
            StopCoroutine(stateRoutine);
        stateRoutine = StartCoroutine(MovingForward());
    }

    protected override void AttackState()
    {
        base.AttackState();
        //if (attackTime >= myDetails.attackSpeed)

        if (stateRoutine != null)
            StopCoroutine(stateRoutine);
        stateRoutine = StartCoroutine(Attacking());
    }
}
