using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : PlayerUnitState
{
    //protected bool hasIdleDuration = true;
    protected bool arrive = false;
    public override void SpawnUnit(Vector3 position, PoolType _type)
    {
        base.SpawnUnit( position,  _type);
        ShowIcon(Color.green);
        direction = Vector3.right;
        myDetails = GameDatas.playerUnitDetailList.Find(x => x.type == type);
        hp = myDetails.hp;
        moveForward = false;
        arrive = false;
        ResourceManager.Instance.AddCurrentPopulation(true);

        ChangeState(UnitStateType.move);
    }

    protected virtual void FindWorkPlace()
    {

    }

    public override void UnitDie()
    {
        base.UnitDie();
        ResourceManager.Instance.AddCurrentPopulation(false);
    }

    protected override void IdleState()
    {
        myAnimator.SetBool(EnumCaching.ToString(AnimationParameters.walking), false);
        transform.GetChild(0).transform.rotation = Quaternion.identity;
        if (!attackUnit)
            StartCoroutine(Idling());
        else
            stateRoutine = StartCoroutine(Detecting());
    }
    
    IEnumerator Idling()
    {
        yield return StartCoroutine(Utility.Waiting(Random.Range(1.0f, 3.0f), () => ChangeState(UnitStateType.move)));
    }

    IEnumerator Detecting()
    {
        while (true)
        {
            if (CheckTarget() != null)
                break;
            else
                yield return null;
        }
        ChangeState(UnitStateType.attack);
    }

    protected float GetDistance(float targetX, out bool isRight)
    {
        myAnimator.SetBool(EnumCaching.ToString(AnimationParameters.walking), true);
        float dist = Mathf.Abs(transform.position.x - targetX);
        if (transform.position.x > targetX)
        {
            isRight = false;
            transform.GetChild(0).transform.Rotate(Vector3.up * 180f);
        }
        else
        {
            isRight = true;
            transform.GetChild(0).transform.rotation = Quaternion.identity;
        }
        //transform.GetChild(0).transform.Rotate(Vector3.up * 1800f);
        return dist;
    }
}
