using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossUnit : BossState
{
    public override void SpawnUnit(Vector3 position, PoolType _type)
    {
        myState = UnitStateType.none;
        base.SpawnUnit(position, _type);
        ShowIcon(Color.red);
        direction = Vector3.left;
        myDetails = GameDatas.comUnitDetailList.Find(x => x.type == type);
        skillDetails = GameDatas.bossSkillDetailsList.Find(x => x.type == type);
        hp = myDetails.hp;
        particle = GetComponentInChildren<ParticleSystem>();
        bossUnit = true;
        firstMove = true;
        ChangeState(UnitStateType.move);
        useSecondSkill = skillDetails.skill2Cooldown > 0.0f;
        EventHandler.CallBossRespawnEvent(hp);
    }
}
