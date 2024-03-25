using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComUnit : ComState
{
    public override void SpawnUnit(Vector3 position, PoolType _type)
    {
        base.SpawnUnit(position, _type);
        ShowIcon(Color.red);
        direction = Vector3.left;
        myDetails = GameDatas.comUnitDetailList.Find(x => x.type == type);
        hp = myDetails.hp;
        ChangeState(UnitStateType.move);
    }
}
