using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class AIProperty : CharacterMovement
{
    protected AIStateType aiState;
    protected Coroutine currentCoroutine = null;
    protected int hp;
    protected Vector2Int idleTime = new Vector2Int(3, 9);
    protected Vector2Int moveDistance = new Vector2Int(1, 5);
    protected float timeToMove = 1.5f; // 이동속도(1칸을 움직이는데 걸리는 시간)
    protected UnityAction searchTarget;
    protected UnityAction moveDetector;
    protected float delay = 0.3f;
    protected float hitTime = -1f;
    protected float attackTime = -1f;
    protected bool hit = false;

    protected void SetAnimatorDirection()
    {
        switch (myDirection)
        {
            case CharacterDirection.up: myAnimator.SetFloat("x", 0f); myAnimator.SetFloat("y", 1f); break;
            case CharacterDirection.right: myAnimator.SetFloat("x", 1f); myAnimator.SetFloat("y", 0f); break;
            case CharacterDirection.left: myAnimator.SetFloat("x", -1f); myAnimator.SetFloat("y", 0f); break;
            case CharacterDirection.down: myAnimator.SetFloat("x", 0f); myAnimator.SetFloat("y", -1f); break;
        }
        moveDetector?.Invoke();
    }
}
