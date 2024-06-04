using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonsterAnimationEvent : MonoBehaviour
{
    public UnityEvent AttackEvent;
    public UnityEvent DieEvent;
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void CallAttackEvent() => AttackEvent?.Invoke();

    public void CallDieEvent() => DieEvent?.Invoke();
}
