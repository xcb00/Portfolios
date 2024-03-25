using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitMovement : UnitState
{
    //Transform target = null;
    public virtual void SpawnUnit(Vector3 position, PoolType _type)
    {
        InitProperty();
        InitData();
        transform.position = position;
        myAnimator.SetTrigger(EnumCaching.ToString(AnimationParameters.reset));
        myAnimator.SetBool(EnumCaching.ToString(AnimationParameters.walking), true);
        poolType = _type;
        knockbackTime = -1f;
        isDie = false;
    }

    protected void InitData()
    {
        myTargets = new List<Transform>();
        removeTargets = new List<Transform>();

        myMaterial.color = new Color(1f, 1f, 1f, 1f);
        halfKnockBack = false;
        myCollider.enabled = true;

        originColor = myMaterial.color;
        gameObject.SetActive(true);

        StartCoroutine(AttackTimer());
    }

    /*protected virtual Transform CheckTarget()
    {
        Transform _target = null;
        float dist = myDetails.range;

        if (myTargets.Count < 1) return null;

        foreach (Transform trans in myTargets)
        {
            if (trans == null) continue;
            else if (!trans.GetComponentInChildren<Collider>().enabled) { removeTargets.Add(trans); continue; }
            else if (Mathf.Abs(trans.position.x - transform.position.x) > myDetails.range) continue;
            else if (Mathf.Abs(trans.position.x - transform.position.x) < dist)
            {
                dist = trans.position.x - transform.position.x;
                _target = trans;
            }
        }

        if (removeTargets.Count > 0)
        {
            foreach (Transform trans in removeTargets)
                myTargets.Remove(trans);
            removeTargets.Clear();
        }
        return _target;
    }*/

    public void AttackTarget()
    {
        Transform target = CheckTarget();
        if (target != null)
        {
            target.GetComponentInParent<IDamage>().GetDamage(myDetails.damage);
        }
    }

    public virtual void AttackEffect()
    {

    }

    protected IEnumerator Attacking()
    {
        Transform target = CheckTarget();
        //int i = 10;
        //Debug.Log($"{target.name}>{target.parent.name}>{target.parent.parent.name}");
        while (target != null)
        {
            // 공격 딜레이가 남았거나 애니메이션이 재생 중이라면 공격 대기
            while (attackTime < myDetails.attackSpeed || myAnimator.GetBool(EnumCaching.ToString(AnimationParameters.playing)))
                yield return null;


            //if (GameDatas.stopUnit) continue;

            // 공격
            myAnimator.SetTrigger(EnumCaching.ToString(AnimationParameters.action1));
            //target.GetComponentInParent<UnitMovement>().GetDamage(myDetails.damage);
            attackTime = 0.0f;

            // SetTrigger를 한다고 애니메이션이 바로 실행되는 것이 아니기 때문에 한 프레임 동안 대기
            yield return null;

            // 공격 애니메이션이 재생될 동안 대기
            while (myAnimator.GetBool(EnumCaching.ToString(AnimationParameters.playing)))
                yield return null;

            // 새로운 타겟 설정
            target = CheckTarget();

            /*if (--i < 0)
                break;*/
        }
        ChangeState(UnitStateType.move);
    }

    protected IEnumerator MovingForward()
    {
        while (CheckTarget()==null)
        {
            //if (GameDatas.stopUnit) continue;

            if (myAnimator.GetBool(EnumCaching.ToString(AnimationParameters.playing)))
            {
                yield return null;
                continue;
            }
                //Debug.DrawRay(transform.GetChild(2).position, direction * myDetails.range, Color.red);
            transform.Translate(direction * Time.deltaTime * myDetails.speed);
            yield return null;
        }
        ChangeState(UnitStateType.attack);
    }

    protected IEnumerator MovingPosition(float dist, Vector3 dir, UnityAction done, bool searchTarget = false)
    {
        while (dist > 0.0f)
        {
            //if (GameDatas.pause) continue;

            if (searchTarget)
            {
                if (CheckTarget() != null) ChangeState(UnitStateType.attack);
            }

            if (myAnimator.GetBool(EnumCaching.ToString(AnimationParameters.playing)))
            {
                yield return null;
                continue;
            }

            float delta = Time.deltaTime * myDetails.speed;
            delta = dist - delta < 0.0f ? dist : delta;
            dist -= delta;
            transform.Translate(dir * delta);
            yield return null;
        }
        done?.Invoke();
    }

    

}
