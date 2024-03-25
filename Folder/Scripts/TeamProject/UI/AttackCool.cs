using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCool : Cooldown
{
    protected override IEnumerator CoolingDown(float time)
    {
        yield return StartCoroutine(base.CoolingDown(time));

        Debug.Log("Attack cool done");
    }
}
