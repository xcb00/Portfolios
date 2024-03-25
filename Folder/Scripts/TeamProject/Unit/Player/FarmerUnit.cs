using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmerUnit : WorkerUnit
{
    protected override void FindWorkPlace()
    {
        if (myTargets.Count < 1)
            FindSoil();
    }

    protected override void AfterWork()
    {
        if(myTargets.Count>0)
            GameDatas.soilQueue.Enqueue(myTargets[0]);
    }

    protected void FindSoil()
    {
        if (GameDatas.soilQueue.Count > 0)
            myTargets.Add(GameDatas.soilQueue.Dequeue());
    }

    public override void UnitDie()
    {
        ResourceManager.Instance.MinusMaxPopulation();
        AfterWork();
        base.UnitDie();
    }

    /*protected Transform FindTree()
    {
        return null;
    }*/
}
