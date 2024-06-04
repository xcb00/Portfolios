using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thief : PlayerCharacterClass
{

    protected override void AttackMonster()
    {
        target.GetComponent<IDamage>().OnDamage(dmg);
    }

    protected override void SkillMonster()
    {
        List<Transform> monsters;
        StageManager.inst.GetRangeMonsters(transform.position, status.skillDistance, out monsters);
        if (monsters == null)
            return;
        
        foreach(Transform monster in monsters)
        {
            monster.GetComponent<IDamage>().OnDamage(dmg);
        }
    }
}
