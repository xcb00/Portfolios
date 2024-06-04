using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttackBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (animator.GetBool(AnimPara.skilling.ToString())) Debug.Log($"{animator.name} use skill");
        //else Debug.Log($"{animator.name} attack");

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) => animator.SetBool(EnumCaching.ToString(AnimPara.skilling), false);
}
