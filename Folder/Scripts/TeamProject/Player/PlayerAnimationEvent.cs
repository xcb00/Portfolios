using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    Player parent = null;
    private void OnEnable()
    {
        parent = GetComponentInParent<Player>();
    }

    private void OnDisable()
    {
        parent = null;
    }
    public void AfterDieEvent()
    {
        //parent.UnitDie();
    }

    public void HitEvent()
    {
        parent.AttackTarget();
    }

    public void FirstSkillEvent()
    {
        parent.FirstSkill();
    }

    public void SecondSkillEvent()
    {
        parent.SecondSkill();
    }

    public void EndSkillEvent()
    {
        //EventHandler.CallCanAttackEvent(true);
    }
}
