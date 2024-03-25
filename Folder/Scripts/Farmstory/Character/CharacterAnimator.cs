using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterAnimator : CharacterProperty
{
    bool isWalk = false;
    CharacterDirection direction = CharacterDirection.down;
    private void Start()
    {
        myAnimator.SetFloat("x", 0f);
        myAnimator.SetFloat("y", -1f);
        myAnimator.SetBool(EnumCaching.ToString(AnimatorParameters.isWalking), false);
    }

    protected void CharacterMove(bool isWalk, UnityAction done = null)
    {
        if (this.isWalk == isWalk)
            return;

        this.isWalk = isWalk;
        if (!isWalk) done?.Invoke();
        myAnimator.SetBool(EnumCaching.ToString(AnimatorParameters.isWalking), this.isWalk);
    }

    protected void CharacterTurn(CharacterDirection direction)
    {
        if (this.direction == direction)
            return;

        this.direction = direction;
        Vector3 dir = Utility.CharacterDirectionToVector3(this.direction);
        myAnimator.SetFloat("x", dir.x);
        myAnimator.SetFloat("y", dir.y);
    }
}
