using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIState : AIProperty
{
    protected void ChangeState(AIStateType _state) //
    {
        //Debug.Log($"{transform.name}'s state change : {aiState} > {_state}");
        if (aiState == _state && _state != AIStateType.create) return;

        aiState = _state;
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        switch (_state)
        {
            case AIStateType.create: CreateState(); break;
            case AIStateType.idle: IdleState(); break;
            case AIStateType.move: MoveState(); break;
            case AIStateType.follow: FollowState(); break;
            case AIStateType.attack: AttackState(); break;
            case AIStateType.die: DieState(); break;
        }
    }

    public virtual void Respawn(Vector3 position) //
    {
        transform.position = position;
        myDirection = (CharacterDirection)Random.Range(0, 4);
        hit = false;
        ChangeState(AIStateType.create);
    }

    //public virtual void Hit(CharacterDirection dir, int damage = 1) //
    public void Hit(CharacterDirection dir, int damage = 1) //
    {
        hit = true;
        CharacterMove(false);
        StopAllCoroutines();
        myDirection = dir;
        SetAnimatorDirection();
        hp -= damage;
        if (hp <= 0) ChangeState(AIStateType.die);
        else StartCoroutine(Hitting());
    }
    protected virtual void DieState()
    {
        aiState = AIStateType.die;
        myAnimator.SetTrigger(EnumCaching.ToString(AnimatorParameters.Dead));
    }

    void CreateState()
    {
        gameObject.SetActive(true);
        StartCoroutine(Creating());
    }

    IEnumerator Creating()
    {
        yield return null;
        ChangeState(AIStateType.move);
    }

    //protected virtual void IdleState()
    protected void IdleState()
    {
        currentCoroutine = StartCoroutine(Idling(Random.Range(idleTime.x, idleTime.y) * 0.3f, () => ChangeState(AIStateType.move)));
    }

    //protected virtual void MoveState()
    protected void MoveState()
    {
        currentCoroutine = StartCoroutine(Moving(Random.Range(moveDistance.x, moveDistance.y), () => ChangeState(AIStateType.idle)));
    }

    protected virtual void FollowState() { }
    protected virtual void AttackState() { }

    #region State Coroutine

    protected virtual IEnumerator Hitting()
    {
        yield return null;
    }

    protected IEnumerator Idling(float time, UnityAction done = null)
    {
        CharacterMove(false);
        while (time > 0.0f)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        done?.Invoke();
    }

    protected IEnumerator Moving(int distacne, UnityAction done = null)
    {
        myDirection = (CharacterDirection)Random.Range(0, 4);
        SetAnimatorDirection();
        CharacterMove(true);
        while (distacne-- > 0)
        {
            // 이동하려는 위치에 Collider가 있다면 이동 중지
            if (MapTileManager.Instance.IsCollider(myDirection, transform.position)) break;
            yield return StartCoroutine(MovingCharacter(transform.position, myDirection, timeToMove, searchTarget/*() => SearchTarget()*/));
        }
        done?.Invoke();
    }

    #endregion
}
