using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("playing", true);
        animator.SetFloat("length", stateInfo.length);
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("playing", false);
        animator.SetFloat("length", -1f);
    }
}
