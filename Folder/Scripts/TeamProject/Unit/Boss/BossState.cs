using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossState : ComProperty
{
    protected float skill1Time = 0.0f;
    protected float skill2Time = 0.0f;
    protected BossSkillDetails skillDetails;
    protected ParticleSystem particle;
    protected bool firstMove = true;
    protected bool useSecondSkill = true;

    protected override void MoveState()
    {
        if (!firstMove) { ChangeState(UnitStateType.idle); return; }
        firstMove = false;
        base.MoveState();
        if (stateRoutine != null)
            StopCoroutine(stateRoutine);
        StartCoroutine(SkillTimer());
        stateRoutine = StartCoroutine(MovingPosition(7f, Vector3.left, ()=>ChangeState(UnitStateType.idle)));
    }

    protected override void IdleState()
    {
        myAnimator.SetBool(EnumCaching.ToString(AnimationParameters.walking), false);
        if (stateRoutine != null)
            StopCoroutine(stateRoutine);
        stateRoutine = StartCoroutine(BossIdling());
        /*if (myTargets.Count > 0)
            ChangeState(UnitStateType.attack);
        else*/
    }

    public override void AddTarget(Transform target)
    {
        base.AddTarget(target);
        ChangeState(UnitStateType.attack);
    }

    protected override void AttackState()
    {
        base.AttackState();
        if (stateRoutine != null)
            StopCoroutine(stateRoutine);
        stateRoutine = StartCoroutine(BossAttacking());
    }

    public override void AttackEffect()
    {
        particle.Play();
        for(int i = 0; i < myTargets.Count; i++)
        {
            if (myTargets[i] == null) continue;
            if (Mathf.Abs(myTargets[i].position.x - transform.position.x) > myDetails.range * 1.5f) continue;
            myTargets[i].GetComponentInParent<IDamage>().GetDamage(skillDetails.skill2Damage, 20);
        }
    }
    public override void UnitDie()
    {
        myCollider.enabled = false;
        StopAllCoroutines();
        StartCoroutine(NextLevel());
    }

    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(1f);
        SceneControlManager.Instance.NextLevel(SceneChange.StageClear);
    }

    IEnumerator BossAttacking()
    {
        Transform target;// = CheckTarget();
        while (true)
        {
            target = CheckTarget();

            if(target==null)
            {
                ChangeState(UnitStateType.idle);
                break;
            }

            while (attackTime < myDetails.attackSpeed || myAnimator.GetBool(EnumCaching.ToString(AnimationParameters.playing)))
                yield return null;

            myAnimator.SetTrigger(EnumCaching.ToString(AnimationParameters.action1));
            attackTime = 0.0f;

            AttackAnimation();
            yield return null;

            while (myAnimator.GetBool(EnumCaching.ToString(AnimationParameters.playing)))
                yield return null;


        }
    }

    IEnumerator BossIdling()
    {
        while (true)
        {
            if (CheckTarget() != null)
                break;

            yield return null;
        }
        ChangeState(UnitStateType.attack);
    }

    void AttackAnimation()
    {
        if (useSecondSkill && skill2Time >= skillDetails.skill2Cooldown)
        {
            myAnimator.SetTrigger(EnumCaching.ToString(AnimationParameters.action3));
            skill2Time = 0.0f;
        }
        else if (skill1Time >= skillDetails.skill1Cooldown)
        {
            myAnimator.SetTrigger(EnumCaching.ToString(AnimationParameters.action2));
            skill1Time = 0.0f;
        }
        else
        {
            myAnimator.SetTrigger(EnumCaching.ToString(AnimationParameters.action1));
        }
        attackTime = 0.0f;
    }

    IEnumerator SkillTimer()
    {
        while (true)
        {
            skill1Time += Time.deltaTime;
            skill2Time += Time.deltaTime;
            yield return null;
        }
    }
}
