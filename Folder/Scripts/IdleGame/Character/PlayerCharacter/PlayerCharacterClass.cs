using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterClass : PlayerCharacterProperty, IDamage
{
    float sqrAtkDist;
    float sqrSkillDist;
    Vector2 dir = Vector2.zero;
    Vector3 flip = Vector3.one;
    protected Transform target;

    float attackTime = 0.0f;
    float skillTime = 0.0f;
    bool canAttack = true;
    bool canSkill = false;

    bool isDie = false;

    int hp;
    protected int dmg;

    public int GetCurrentHP => currentHP;
    public Vector2 GetPlayerDirection => dir;

    protected override void OnDisable()
    {
        EventHandler.EnquequeMonsterEvent -= FindNewTarget;
    }

    private void OnEnable()
    {
        EventHandler.EnquequeMonsterEvent += FindNewTarget;
    }

    void FindNewTarget(Transform trans)
    {
        currentState = CharacterState.none;
        if (target == null)
            EndRoutine(CharacterState.idle);
        else if (target == trans)
        {
            if (++currentExp >= status.exp)
            {
                currentExp = 0;
                level++;
                SetLevel();
            }

            target = null;
            EndRoutine(CharacterState.idle);
        }
        else
            EndRoutine(CharacterState.follow);
    }

    public override void Initialize(PlayerCharacter character)
    {
        isDie = false;

        GameManager.inst.ActiveCharacter(character);

        base.Initialize(character);

        sqrAtkDist = status.attackDistance * status.attackDistance;
        sqrSkillDist = status.skillDistance * status.skillDistance;
        
        timer = StartCoroutine(Timer());


        ChangeState(CharacterState.idle);
    }

    public void SetLevel(bool init = false)
    {
        if (init)
        {
            level = 1;
            currentExp = 0;
        }
        int value = (((int)character - 1) * 2);

        int adjust = 8 + level + DataManager.inst.GetUpgradeLevel(value++);
        hp = Mathf.RoundToInt(status.hp * adjust * 0.1f);// + (10 + upgrade) * 0.1f;
        currentHP = hp; 
        adjust = 8 + level + DataManager.inst.GetUpgradeLevel(value);
        dmg = Mathf.RoundToInt(status.damage * adjust * 0.1f);
        status.exp = Mathf.RoundToInt(status.exp + status.exp * (level - 1) * 0.2f);

        characterUI.InitInfo(currentHP, level);
        characterUI.ActiveObject(true);
        characterUI.MoveCharacter(transform.position);
    }

    void Respawn()
    {
        if (GameManager.inst.EndGame)
            return;

        isDie = false;

        GameManager.inst.ActiveCharacter(character);

        currentHP = hp;
        characterUI.InitInfo(currentHP);
        gameObject.SetActive(true);
        characterUI.ActiveObject(true); 
        characterUI.MoveCharacter(transform.position);

        currentState = CharacterState.none;

        timer = StartCoroutine(Timer());

        ChangeState(CharacterState.follow);
    }

    protected void ChangeState(CharacterState state)
    {
        if (isDie) return;

        animator.SetBool(EnumCaching.ToString(AnimPara.walking), false);
        switch (state)
        {
            case CharacterState.follow: 
                FollowState();
                break;
            case CharacterState.attack:
                AttackState();
                break;
            case CharacterState.hurt: break;
            case CharacterState.die: break;

            case CharacterState.idle:
                animator.SetTrigger(EnumCaching.ToString(AnimPara.idle));
                currentRoutine = StartCoroutine(Idling());
                break;
            case CharacterState.move:
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

    protected void FollowState()
    {
        currentState = CharacterState.follow;
        currentRoutine = StartCoroutine(Following());

    }

    protected void AttackState()
    {
        currentState = CharacterState.attack;

        if (CheckTarget())
        {
            if (target != null)
                ChangeState(CharacterState.follow);
            else
                ChangeState(CharacterState.idle);
        }
        else
        {
            if (canAttack)
            {
                canAttack = false;
                attackTime = 0.0f;

                if (canSkill)
                {
                    canSkill = false;
                    skillTime = 0.0f;
                    animator.SetTrigger(EnumCaching.ToString(AnimPara.casting));
                }
                else
                    animator.SetTrigger(EnumCaching.ToString(AnimPara.attack));
            }
        }
    }

    protected void HurtState()
    {
        currentState = CharacterState.hurt;
    }

    protected void DieState()
    {
        currentState = CharacterState.die;
    }

    bool CheckTarget()
    {
        if (target == null)
            EndRoutine(CharacterState.idle);

        dir = target.position - transform.position;

        flip.x = dir.x >= 0.0f ? -1.0f : 1.0f;
        transform.localScale = flip;

        if (dir.sqrMagnitude <= sqrAtkDist)
            return false;
        else
            return true;
    }

    IEnumerator Idling()
    {
        while (target == null)
        {
            target = StageManager.inst.GetNearestMonster(transform.position);
            yield return null;
        }
        EndRoutine(CharacterState.follow);
    }

    protected IEnumerator Following()
    {
        if (target == null)
            EndRoutine(CharacterState.idle);

        dir = target.position - transform.position;

        animator.SetBool(EnumCaching.ToString(AnimPara.walking), true);

        while (CheckTarget())
        {
            moveTransform.Translate(dir.normalized * Time.deltaTime * status.speed);
            characterUI.MoveCharacter(transform.position);
            yield return null;
        }
        EndRoutine(CharacterState.attack);
    }

    IEnumerator Timer()
    {
        while (true)
        {
            if (isDie) break;

            if (!canAttack)
            {
                attackTime += Time.deltaTime;
                if (status.attackCooldown <= attackTime)
                    canAttack = true;
            }
            else if(currentState == CharacterState.attack)
            {
                AttackState();
            }

            if (!canSkill)
            {
                skillTime += Time.deltaTime;
                if (status.skillCooldown <= skillTime)
                    canSkill = true;
            }

            yield return null;
        }
    }

    public void AttackAnimation()
    {
        if (animator.GetBool(EnumCaching.ToString(AnimPara.skilling)))
            SkillMonster();
        else
            AttackMonster();
    }

    protected virtual void AttackMonster() { }
    protected virtual void SkillMonster() { }

    // Die 애니메이션 후 실행되는 메소드
    public void DisablePlayerCharacter()
    {
        DisablePlayerCharacter(true);        
    }

    public void DisablePlayerCharacter(bool respawn)
    {
        target = null;
        currentState = CharacterState.idle;
        characterUI.ActiveObject(false);
        gameObject.SetActive(false);
        StopAllCoroutines();

        if (respawn)
        {
            GameManager.inst.InActiveCharacter(character);
            Invoke("Respawn", status.spawnTime);
        }
        else
            transform.SetParent(GameManager.inst.transform);
    }

    public void OnDamage(int damage, float stun = -1f)
    {
        if (isDie) return;

        animator.SetBool(EnumCaching.ToString(AnimPara.walking), false);

        currentHP -= damage;

        if (currentHP > hp)
            currentHP = hp;

        if (currentHP > 0)
        {
            animator.SetTrigger(EnumCaching.ToString(AnimPara.hurt));
            characterUI.OnDamage(currentHP);
        }
        else
        {
            isDie = true;
            animator.SetTrigger(EnumCaching.ToString(AnimPara.die));
        }
    }
}
