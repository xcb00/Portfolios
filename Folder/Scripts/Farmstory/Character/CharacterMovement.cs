using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterMovement : CharacterAnimator
{
    protected bool isCharacterMoving = false;

    #region Summary
    /// <summary>
    /// MovingCharacter 코루틴을 호출하는 함수
    /// </summary>
    /// <param name="originPosition">현재 Character의 position</param>
    /// <param name="direction">Character가 움직일 방향</param>
    /// <param name="timeToMove">Character가 1칸 움직이는데 걸리는 시간</param>
    /// <param name="done">Character가 1칸을 움직인 후 실행할 함수</param>
    #endregion
    protected void MoveCharacter(Vector3 originPosition, CharacterDirection direction, float timeToMove, UnityAction done = null)
    {
        StartCoroutine(MovingCharacter(originPosition, direction, timeToMove, done));
    }

    #region Summary
    /// <summary>
    /// 캐릭터를 _drirection 방향으로 _timeToMove 시간 동안 1칸 움직이는 코루틴
    /// 매개변수는 MoveCharacter의 매개변수와 동일
    /// </summary>
    #endregion
    protected IEnumerator MovingCharacter(Vector3 originPosition, CharacterDirection direction, float timeToMove, UnityAction done)
    {
        // 게임이 일시정지일 경우 캐릭터를 움직이지 않음
        if (!GameDatas.pause)
        {
            isCharacterMoving = true;
            // 원래 위치에서 움직일 방향의 벡터를 더해 이동할 위치를 구함
            Vector3 targetPosition = GetTargetPosition(originPosition, direction); // + Utility.CharacterDirectionToVector3(direction);

            // 코루틴이 실행되는 시간으로, Lerp를 사용하기 위해 사용
            float time = 0.0f;
            slow = true;
            // timeToMove 시간동안 targetPosition으로 이동
            while (time < timeToMove)
            {
                float delta = Time.deltaTime * (GameDatas.pause ? 0.1f : 1.0f);
                time = time + delta > timeToMove ? timeToMove : time + delta;
                transform.position = Vector3.Lerp(originPosition, targetPosition, time / timeToMove);

                yield return null;
            }

            // 반복문이 종료되면 캐릭터의 위치를 타겟 위치로 이동
            transform.position = targetPosition;

            isCharacterMoving = false;

            // _done이 null이 아니면 _done에 연결된 함수 실행
            done?.Invoke();
        }
        else
        {
            while (GameDatas.pause) { yield return null; }
        }
    }



    public Vector3 GetTargetPosition(Vector3 position, CharacterDirection dir) => new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z)) + Utility.CharacterDirectionToVector3(dir);


    public void CharacterStopMove()
    {
        StopAllCoroutines();
        isCharacterMoving = false;
        CharacterMove(false);
    }



#if UNITY_EDITOR
    /*private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + Utility.CharacterDirectionToVector3(myDirection), 2f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Utility.CharacterDirectionToVector3(myDirection), 0.75f);

    }*/
#endif
}

