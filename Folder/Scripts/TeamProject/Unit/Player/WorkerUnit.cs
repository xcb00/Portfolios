using System.Collections;
using UnityEngine;

public class WorkerUnit : PlayerUnit
{
    Vector3 returnPosition = Vector3.zero;
    bool beforeWork = true;

    public override void SpawnUnit(Vector3 position, PoolType _type)
    {
        base.SpawnUnit(position, _type);
        returnPosition = position;
    }

    protected override Transform CheckTarget(bool isHit = false)
    {
        return null;
    }

    protected override void MoveState()
    {
        if (beforeWork)
        {
            FindWorkPlace();
            if (myTargets.Count<1)
                ChangeState(UnitStateType.idle);
            else
            {
                float dist = GetDistance(myTargets[0].position.x, out bool isRight);
                Vector3 dir = isRight ? Vector3.right : Vector3.left;
                StartCoroutine(MovingPosition(dist, dir, ()=>ChangeState(UnitStateType.attack)));
            }
        }
        else
        {
            float dist = GetDistance(returnPosition.x, out bool isRight);
            Vector3 dir = isRight ? Vector3.right : Vector3.left;
            StartCoroutine(MovingPosition(dist, dir, () => ChangeState(UnitStateType.idle)));
        }
        beforeWork = !beforeWork;
    }

    

    protected override void AttackState()
    {
        StartCoroutine(Working());
    }

    

    

    IEnumerator Working()
    {
        myAnimator.SetBool(EnumCaching.ToString(AnimationParameters.walking), false);
        myAnimator.SetTrigger(EnumCaching.ToString(AnimationParameters.action1));
        yield return null;

        float actionTime = myAnimator.GetCurrentAnimatorStateInfo(0).length + myDetails.attackSpeed;

        yield return StartCoroutine(Utility.Waiting(actionTime));

        for (int i = 0; i < 2; i++)
        {
            myAnimator.SetTrigger(EnumCaching.ToString(AnimationParameters.action1));
            yield return StartCoroutine(Utility.Waiting(actionTime));
        }

        if (myTargets.Count > 0)
        { 
            AfterWork();
            myTargets.Clear();
        }


        ChangeState(UnitStateType.move);
    }

    protected virtual void AfterWork()
    {

    }

    
}