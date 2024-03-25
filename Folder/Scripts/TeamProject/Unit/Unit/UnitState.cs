using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface IDamage
{
    public void GetDamage(float dmg, int possibility = 0);
}

public class UnitState : UnitProperty, IDamage
{
    protected Coroutine stateRoutine = null;
    protected bool halfKnockBack = false;
    protected bool bossUnit = false;
    protected bool isDie = false;
    Coroutine hitRoutine = null;

    protected void ChangeState(UnitStateType state)
    {
        if (myState == state) return;
        else myState = state;

        switch (state)
        {
            case UnitStateType.move:
                MoveState();
                break;
            case UnitStateType.attack:
                AttackState();
                break;
            /*case UnitStateType.hit:
                HitState();
                break;*/
            case UnitStateType.knockback:
                KnockBackState();
                break;
            case UnitStateType.idle:
                IdleState();
                break;
            default:
                break;
        }
    }

    protected virtual void MoveState()
    {
        myAnimator.SetBool(EnumCaching.ToString(AnimationParameters.walking), true);
    }

    protected virtual void AttackState()
    {
        myAnimator.SetBool(EnumCaching.ToString(AnimationParameters.walking), false);
    }

    /*protected virtual void HitState()
    {
        Debug.Log($"{transform.name} hit");
        if (hitRoutine != null) StopCoroutine(hitRoutine);
        hitRoutine = StartCoroutine(CharacterHitting());
    }*/
    protected void KnockBackState()
    {
        if (stateRoutine != null)
            StopCoroutine(stateRoutine);
        stateRoutine = StartCoroutine(KnockBacking());

    }

    protected IEnumerator KnockBacking()
    {
        myAnimator.SetTrigger(EnumCaching.ToString(AnimationParameters.knockback));
        attackTime = 0.0f;
        yield return null;

        if (knockbackTime < 0f)
        {
            while (!myAnimator.GetBool(EnumCaching.ToString(AnimationParameters.playing)))
                yield return null;

            knockbackTime = myAnimator.GetCurrentAnimatorStateInfo(0).length;// knockbackTime = myAnimator.GetFloat("length");
        }
        float backDist = 1f;
        float timing = 0.0f;
        Vector3 from = transform.position;
        Vector3 to = transform.position - direction * backDist;
        StartCoroutine(CharacterHitting(knockbackTime * 0.5f));
        while (timing < knockbackTime)
        {
            timing = timing + Time.deltaTime > knockbackTime ? knockbackTime : timing + Time.deltaTime;
            transform.position = Vector3.Lerp(from, to, timing / knockbackTime);
            yield return null;
        }

        yield return null;

        ChangeState(UnitStateType.move);
        /*if (CheckTarget() != null)
            ChangeState(UnitStateType.attack);
        else
            ChangeState(UnitStateType.move);*/
    }

    protected virtual void IdleState()
    {

    }

    public virtual void UnitDie()
    {
        PoolManager.Instance.EnqueueObject(PoolType.MinimapIcon, icon);
        myCollider.enabled = false;
        StopAllCoroutines();
        StartCoroutine(Dying());
    }

    protected IEnumerator Dying()
    {
        yield return StartCoroutine(Utility.Waiting(0.8f));

        float dist = 0.5f;
        while (dist > 0f)
        {
            dist -= Time.deltaTime;
            transform.Translate(-transform.up * Time.deltaTime);
            yield return null;
        }

        isDie = false;
        if ((int)poolType < (int)PoolType.PlayerFarmer)
            ResourceManager.Instance.AddGold(myDetails.price);

        PoolManager.Instance.EnqueueObject(poolType, transform.gameObject);
    }

    public void GetDamage(float damage, int possibility = 0)
    {
        if (isDie)
            return;
        hp -= damage;
        if (bossUnit) EventHandler.CallBossGetDamageEvent(hp);

        if (hp<0f)
        {
            isDie = true;
            myAnimator.SetTrigger(EnumCaching.ToString(AnimationParameters.death));
            UnitDie();
        }
        else if(hp<=myDetails.hp*0.5f && !halfKnockBack)
        {
            halfKnockBack = true;
            ChangeState(UnitStateType.knockback);
        }
        else
        {
            if (Random.Range(1, 21) < possibility)
                ChangeState(UnitStateType.knockback);
            else
            {
                //ChangeState(UnitStateType.hit);
                if (hitRoutine != null) StopCoroutine(hitRoutine);
                hitRoutine = StartCoroutine(CharacterHitting());

                //if (CheckTarget(true) != null)
                ChangeState(UnitStateType.move);
            }
        }
    }

    public virtual void AddTarget(Transform target)
    {
        if (!myTargets.Contains(target))
            myTargets.Add(target);
    }

    public void RemoveTarget(Transform target)
    {
        if (myTargets.Contains(target))
            myTargets.Remove(target);
    }

    protected virtual Transform CheckTarget(bool isHit = false)
    {
        Transform _target = null;
        float dist = myDetails.range * (isHit ? 1.5f : 1f);

        if (myTargets.Count < 1) return null;

        foreach (Transform trans in myTargets)
        {
            if (trans == null) continue;
            else if (!trans.GetComponentInChildren<Collider>().enabled) { removeTargets.Add(trans); continue; }
            else if (Mathf.Abs(trans.position.x - transform.position.x) > dist) continue;
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
    }
}
