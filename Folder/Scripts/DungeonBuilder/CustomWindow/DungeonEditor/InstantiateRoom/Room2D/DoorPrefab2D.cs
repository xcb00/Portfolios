using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPrefab2D : DoorPrefab
{
    [SerializeField] Collider2D collider = null;

    protected override void OnEnable()
    {
        if (collider == null)
        {
            Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].tag.Equals("DoorCollider"))
                {
                    collider = GetComponentInChildren<Collider2D>();
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
