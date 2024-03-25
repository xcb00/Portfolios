using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger2D : DoorTrigger
{
    [SerializeField] Collider2D collider = null;

    protected override void OnEnable()
    {
        if (collider == null)
            collider = GetComponent<Collider2D>();

        base.OnEnable();
    }
    protected override void EnterRoom() => collider.enabled = false;
    protected override void ClearRoom() => collider.enabled = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & DungeonBuilder.Inst.playerCollider) != 0)
        {
            DungeonBuilder.Inst.ChangeRoom(transform.GetComponentInParent<DoorPrefab>().orientation);
        }
    }
}
