using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : PlayerCharacterClass
{

    protected override void AttackMonster()
    {
        target.GetComponent<IDamage>().OnDamage(dmg);
    }

    protected override void SkillMonster()
    {
        target.GetComponent<IDamage>().OnDamage(Mathf.RoundToInt(dmg * 2.5f));
    }
}
