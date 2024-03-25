using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyState : AIState
{
    protected Collider2D detector = null;
    protected bool playerEnter = false;
    protected Transform targetTransform;

    public override void Respawn(Vector3 position)
    {
        detector = transform.GetChild(1).GetComponent<Collider2D>();
        // 리스폰 시 실행할 명령어(변수 초기화 등)
        searchTarget += SearchTarget;
        moveDetector += MoveDetector;
        playerEnter = false;
        hit = false;
        targetTransform = null;
        base.Respawn(position);
    }

    protected override void DieState()
    {
        searchTarget -= SearchTarget;
        moveDetector -= MoveDetector;
        base.DieState();
        EventHandler.CallEnemyDieEvent();
    }
    /*protected override void IdleState()
    {
        currentCoroutine = StartCoroutine(Idling(Random.Range(idleTime.x, idleTime.y) * 0.3f, () => ChangeState(AIStateType.move)));
    }

    protected override void MoveState()
    {
        currentCoroutine = StartCoroutine(Moving(Random.Range(moveDistance.x, moveDistance.y), () => ChangeState(AIStateType.idle)));
    }*/
    protected override void FollowState() { currentCoroutine = StartCoroutine(Following()); }
    protected override void AttackState() { currentCoroutine = StartCoroutine(Attacking()); }

    #region State Coroutine

    protected override IEnumerator Hitting()
    {
        myAnimator.SetTrigger(EnumCaching.ToString(AnimatorParameters.Hit));
        yield return null;

        if (hitTime < 0f) hitTime = myAnimator.GetFloat(EnumCaching.ToString(AnimatorParameters.length)) + 0.1f;
        float t = 0.0f;
        while (t < hitTime)
        {
            t += Time.deltaTime;
            yield return null;
        }
        searchTarget?.Invoke();
    }

    protected IEnumerator Following()
    {
        while (targetTransform != null)
        {
            CharacterMove(true);
            myDirection = AIPathFind.Instance.GetDirection(myDirection, transform.position, targetTransform.position);
            SetAnimatorDirection();
            yield return StartCoroutine(MovingCharacter(transform.position, myDirection, timeToMove, searchTarget));
        }
    }

    protected IEnumerator Attacking()
    {
        // 게임이 일시정지가 아닐 경우 공격
        if (!GameDatas.pause)
        {
            CharacterMove(false);

            // Target에 도달하면 delay동안 대기
            yield return Idling(delay, null);

            if (!hit)
            {
                // 공격방향 설정
                SetAnimatorDirection();

                // 공격 애니메이션 재생
                myAnimator.SetTrigger(EnumCaching.ToString(AnimatorParameters.Attack));

                yield return null;

                if (attackTime < 0f) attackTime = myAnimator.GetFloat(EnumCaching.ToString(AnimatorParameters.length)) + delay * 3;
                float t = 0.0f;
                while (t < attackTime)
                {
                    t += Time.deltaTime;
                    yield return null;
                }
            }
            hit = false;
            searchTarget?.Invoke();
        }
    }

    #endregion

    protected void SearchTarget()
    {
        if (!playerEnter)
        {
            if(targetTransform!=null)
                targetTransform = null;
            ChangeState(AIStateType.idle);
            return;
        }

        aiState = AIStateType.none;
        if (ReachTarget()) ChangeState(AIStateType.attack);
        else ChangeState(AIStateType.follow);
    }

    public bool ReachTarget(float dist = 1f)
    {
        int x = Mathf.RoundToInt(targetTransform.position.x - transform.position.x);
        int y = Mathf.RoundToInt(targetTransform.position.y - transform.position.y);

        if (Mathf.Abs(x) + Mathf.Abs(y) <= 1 * dist) { myDirection = Utility.Vector2IntToCharacterDirection(new Vector2Int(x, y)); return true; }
        else return false;
    }

    protected void MoveDetector()
    {
        detector.offset = Utility.CharacterDirectionToVector2(myDirection);
    }

    public void EnterPlayer(bool enter, Transform target = null)
    {
        playerEnter = enter;
        if (playerEnter)
        {
            targetTransform = target;
            SearchTarget();
        }
    }
}
