using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimationEvent : MonoBehaviour
{
    UnitMovement parent = null;
    private void OnEnable()
    {
        parent = GetComponentInParent<UnitMovement>();
    }

    private void OnDisable()
    {
        parent = null;
    }
    public void AfterDieEvent()
    {
        parent.UnitDie();
    }

    public void HitEvent()
    {
        parent.AttackTarget();
    }

    public void EffectEvent()
    {
        parent.AttackEffect();
    }
}
