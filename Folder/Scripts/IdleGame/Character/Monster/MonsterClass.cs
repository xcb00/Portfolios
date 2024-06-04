using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MonsterClass : MonsterProperty, IDamage
{
    List<Transform> targets = null;
    public Transform currentTarget;

    float sqrAtkDist;
    float attackTime = 0.0f;
    bool canAttack = true;
    Vector3 flip = Vector3.one;
    Vector2 dir = Vector2.zero;

    bool isStun = false;
    bool isDie = false;


    public override void Initialize(MonsterCharacter character, bool isBoss = false)
    {
        if (character == MonsterCharacter.None)
            return;

        isDie = false;

        base.Initialize(character, isBoss);
        if (targets == null)
            targets = new List<Transform>();
        else
            targets.Clear();

        GetComponentInChildren<MonsterDetector>().Initialize(status.detectDistance, (trans) => EnterCharacter(trans), (trans) => ExitCharacter(trans));
        sqrAtkDist = status.attackDistance * status.attackDistance;
        timer = StartCoroutine(Timer());
        ChangeState(CharacterState.idle);
        gameObject.SetActive(true);
    }

    protected void ChangeState(CharacterState state)
    {
        animator.SetBool(EnumCaching.ToString(AnimPara.walking), false);
        if (isDie || currentState == state) return;
        if (!gameObject.activeSelf) return;

        switch (state)
        {
            case CharacterState.follow:
                FollowState();
                break;
            case CharacterState.attack:
                AttackState();
                break;
            case CharacterState.idle:
                IdleState();
                break;
            case CharacterState.move:
                MoveState();
                break;
            case CharacterState.hurt: 
            case CharacterState.die: 
            case CharacterState.none:
            default: break;
        }
    }

    void EndRoutine(CharacterState state = CharacterState.none)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        if (state != CharacterState.none)
        {
            currentState = CharacterState.none;
            ChangeState(state);
        }
    }

    protected void IdleState()
    {
        currentState = CharacterState.idle;
        currentRoutine = StartCoroutine(Idling());
    }

    IEnumerator Idling()
    {
        
        float t = Random.Range(8, 16) * 0.1f;
        while (t > 0.0f)
        {
            if (isDie) break;
            while (isStun)
                yield return null;

            t -= Time.deltaTime;
            yield return null;
        }
        EndRoutine(CharacterState.move);
    }

    IEnumerator Moving()
    {
        float t = Random.Range(5, 15) * 0.1f;
        float degree = Random.Range(0, 18) * 20f;
        dir.x = Mathf.Cos(degree * Mathf.Deg2Rad);
        dir.y = Mathf.Sin(degree * Mathf.Deg2Rad);

        flip.x = dir.x <= 0.0f ? -1.0f : 1.0f;
        transform.localScale = flip;
        animator.SetBool(EnumCaching.ToString(AnimPara.walking), true);

        while (t > 0.0f)
        {
            if (isDie) break;
            while (isStun)
                yield return null;

            t -= Time.deltaTime;
            transform.Translate(dir * Time.deltaTime * status.speed);
            yield return null;
        }
        EndRoutine(CharacterState.idle);
    }

    protected void MoveState()
    {
        currentState = CharacterState.move;
        currentRoutine = StartCoroutine(Moving());
    }

    protected void FollowState()
    {
        currentState = CharacterState.follow;
        animator.SetBool(EnumCaching.ToString(AnimPara.walking), true);
        currentRoutine = StartCoroutine(Following());
    }

    protected void AttackState()
    {
        currentState = CharacterState.attack;

        if (CheckTarget())
        {
            EndRoutine(CharacterState.follow);
        }
        else
        {
            if (currentTarget == null)
                EndRoutine(CharacterState.idle);

            else if (canAttack)
            {
                canAttack = false;
                attackTime = 0.0f;
                animator.SetTrigger(EnumCaching.ToString(AnimPara.attack));
            }
        }
    }

    bool CheckTarget()
    {
        if (currentTarget == null)
            return false; // 타겟이 없는 경우 null로 변경

        dir = currentTarget.position - transform.position;
        flip.x = dir.x <= 0.0f ? -1.0f : 1.0f;
        transform.localScale = flip;

        if (dir.sqrMagnitude <= sqrAtkDist)
            return false; // 타겟이 범위 내에 있는 경우 공격
        else
            return true; // 타겟이 범위 밖에 있는 경우 이동
    }

    IEnumerator Following()
    {
        dir = currentTarget.position - transform.position;

        while (CheckTarget())
        {
            if (isDie) break;
            while (isStun)
                yield return null;

            transform.Translate(dir.normalized * Time.deltaTime * status.speed);
            yield return null;
        }
        AttackState();
        /*if (currentTarget == null)
            EndRoutine(CharacterState.idle);
        else
            EndRoutine(CharacterState.attack);*/
    }

    void EnterCharacter(Transform trans)
    {
        if (!targets.Contains(trans))
            targets.Add(trans);

        if (currentTarget == null)
            currentTarget = trans;

        AttackState();
        //ChangeState(CharacterState.attack);
    }

    void ExitCharacter(Transform trans)
    {
        if (targets.Contains(trans))
            targets.Remove(trans);

        if (trans == currentTarget)
        {
            if (targets.Count > 0)
                currentTarget = targets[0];
            else
                currentTarget = null;
        }
        AttackState();
    }

    IEnumerator Timer()
    {
        while (true)
        {
            if (isDie) break;
            while (isStun)
                yield return null;

            if (!canAttack)
            {
                attackTime += Time.deltaTime;
                if (status.attackCooldown <= attackTime)
                    canAttack = true;
            }
            else if (currentState == CharacterState.attack)
                AttackState();

            yield return null;

        }
    }

    IEnumerator Stuning(float time)
    {
        animator.SetTrigger(EnumCaching.ToString(AnimPara.idle));
        while (time > 0.0f)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        isStun = false;
    }

    public virtual void AttackAnimation() { }

    public virtual void DieAnimation()
    {
        StageManager.inst.EnqueueMonster(character, gameObject);
        if (isBoss)
        {
            transform.GetChild(0).localScale *= 0.5f;
            GameManager.inst.StageClear();
        }
    }

    public void OnDamage(int damage, float stun = -1f)
    {
        if (isDie) return;

        animator.SetBool(EnumCaching.ToString(AnimPara.walking), false);
        currentHP -= damage;

        if (currentHP > 0)
        {
            if (!isStun && stun > 0.0f)
            {
                isStun = true;
                StartCoroutine(Stuning(stun));
            }
        }
        else
        {
            isDie = true;
            animator.SetTrigger(EnumCaching.ToString(AnimPara.die));
        }
    }

    public MonsterCharacter GetMonsterCharacter() => character;
}
