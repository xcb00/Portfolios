using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : PlayerCharacterClass
{
    protected override void AttackMonster()
    {
        target.GetComponent<IDamage>().OnDamage(dmg);
    }

    protected override void SkillMonster()
    {
        target.GetComponent<IDamage>().OnDamage(dmg, 1f);
    }
}
