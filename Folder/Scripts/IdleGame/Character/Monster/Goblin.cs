using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : MonsterClass
{
    public override void AttackAnimation()
    {
        if(currentTarget!=null)
            currentTarget.GetComponent<IDamage>().OnDamage(Mathf.RoundToInt(status.damage * (9 + GameManager.inst.currentStage) * 0.1f));
    }
}
