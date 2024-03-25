using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinerUnit : WorkerUnit
{
    Transform crystalVein;

    public void SetTarget(Transform trans) { crystalVein = trans; }

    protected override void FindWorkPlace()
    {
        myTargets.Add(crystalVein);
    }

    protected override void AfterWork()
    {
        ResourceManager.Instance.AddGold(Mathf.RoundToInt(myDetails.damage));
    }
}
