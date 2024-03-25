using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProperty : CharacterProperty
{
    UnitDetails _unitDetails;
    protected UnitDetails myDetails
    {
        get { return _unitDetails; }
        set
        {
            //if (_unitDetails == null)
                _unitDetails = value;
        }
    }



    protected List<Transform> myTargets = null;
    protected List<Transform> removeTargets = null;
    protected UnitStateType myState = UnitStateType.none;
    protected Vector3 direction = Vector3.zero;
    protected float attackTime = 0.0f;
    protected float knockbackTime = -1.0f;
    protected PoolType poolType;
    //[SerializeField] protected LayerMask enemy;
    protected IEnumerator AttackTimer()
    {
        while (true)
        {
            attackTime += Time.deltaTime;
            yield return null;
        }
    }

    public float Damage => myDetails.damage;

    protected override void InitProperty()
    {
        base.InitProperty();
        _unitDetails = null;
        hp = 0f;
        myTargets = null;
        myState = UnitStateType.none;
        direction = Vector3.zero;
        attackTime = 0.0f;
    }
}
