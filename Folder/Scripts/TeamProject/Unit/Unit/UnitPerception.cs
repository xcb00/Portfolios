using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPerception : MonoBehaviour
{
    UnitMovement unit;
    [SerializeField]LayerMask enemy;
    private void Start()
    {
        unit = GetComponentInParent<UnitMovement>();
        /*unit = GetComponentInParent<ComUnit>();
        if(unit==null)
            unit = GetComponentInParent<PlayerUnit>();*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((enemy & (1 << other.gameObject.layer)) != 0)
            unit.AddTarget(other.transform);

        if (((1<<(int)LayerName.EnquequeUnit) & (1 << other.gameObject.layer)) != 0)
            unit.GetDamage(10000);
    }

    private void OnTriggerExit(Collider other)
    {
        if ((enemy & (1 << other.gameObject.layer)) != 0)
            unit.RemoveTarget(other.transform);
    }
}
