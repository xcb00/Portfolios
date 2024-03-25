using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPrefab3D : DoorPrefab
{
    [SerializeField] Collider collider = null;

    protected override void OnEnable()
    {
        if (collider == null)
        {
            Collider[] colliders = GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].tag.Equals("DoorCollider"))
                {
                    collider = GetComponentInChildren<Collider>();
                    break;
                }
            }
        }

        base.OnEnable();
    }

    protected override void EnterRoom()
    {
        collider.enabled = true;
    }

    protected override void ClearRoom()
    {
        collider.enabled = false;
    }
}
