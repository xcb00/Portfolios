using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger3D : DoorTrigger
{
    [SerializeField] Collider collider = null;

    protected override void OnEnable()
    {
        if (collider == null)
            collider = GetComponent<Collider>();

        base.OnEnable();
    }
    protected override void EnterRoom() => collider.enabled = false;
    protected override void ClearRoom() => collider.enabled = true;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.layer);
        Debug.Log(LayerMask.LayerToName(DungeonBuilder.Inst.playerCollider));
        if (((1 << other.gameObject.layer) & DungeonBuilder.Inst.playerCollider) != 0)
        {
            DungeonBuilder.Inst.ChangeRoom(transform.GetComponentInParent<DoorPrefab>().orientation);
        }
    }
}
