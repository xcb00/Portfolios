using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterMovement : CharacterAnimator
{
    protected bool isCharacterMoving = false;

    #region Summary
    /// <summary>
    /// MovingCharacter �ڷ�ƾ�� ȣ���ϴ� �Լ�
    /// </summary>
    /// <param name="originPosition">���� Character�� position</param>
    /// <param name="direction">Character�� ������ ����</param>
    /// <param name="timeToMove">Character�� 1ĭ �����̴µ� �ɸ��� �ð�</param>
    /// <param name="done">Character�� 1ĭ�� ������ �� ������ �Լ�</param>
    #endregion
    protected void MoveCharacter(Vector3 originPosition, CharacterDirection direction, float timeToMove, UnityAction done = null)
    {
        StartCoroutine(MovingCharacter(originPosition, direction, timeToMove, done));
    }

    #region Summary
    /// <summary>
    /// ĳ���͸� _drirection �������� _timeToMove �ð� ���� 1ĭ �����̴� �ڷ�ƾ
    /// �Ű������� MoveCharacter�� �Ű������� ����
    /// </summary>
    #endregion
    protected IEnumerator MovingCharacter(Vector3 originPosition, CharacterDirection direction, float timeToMove, UnityAction done)
    {
        // ������ �Ͻ������� ��� ĳ���͸� �������� ����
        if (!GameDatas.pause)
        {
            isCharacterMoving = true;
            // ���� ��ġ���� ������ ������ ���͸� ���� �̵��� ��ġ�� ����
            Vector3 targetPosition = GetTargetPosition(originPosition, direction); // + Utility.CharacterDirectionToVector3(direction);

            // �ڷ�ƾ�� ����Ǵ� �ð�����, Lerp�� ����ϱ� ���� ���
            float time = 0.0f;
            slow = true;
            // timeToMove �ð����� targetPosition���� �̵�
            while (time < timeToMove)
            {
                float delta = Time.deltaTime * (GameDatas.pause ? 0.1f : 1.0f);
                time = time + delta > timeToMove ? timeToMove : time + delta;
                transform.position = Vector3.Lerp(originPosition, targetPosition, time / timeToMove);

                yield return null;
            }

            // �ݺ����� ����Ǹ� ĳ������ ��ġ�� Ÿ�� ��ġ�� �̵�
            transform.position = targetPosition;

            isCharacterMoving = false;

            // _done�� null�� �ƴϸ� _done�� ����� �Լ� ����
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

