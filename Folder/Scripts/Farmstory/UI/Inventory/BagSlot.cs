using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagSlot : Slot
{
    protected override void ClickEvent()
    {
        if (itemCode > 0 && panel==Inventories.bag && InventoryManager.Instance.GetCanCropped(itemCode))
            inventory.SelectSeed(itemCode, index);
    }

    protected override void BegineDragEvent()
    {
        if (itemCode > 0)
        {
            EventHandler.CallDragItemActiveEvent(panel, type, index);
        }
        else
            Debug.Log("∫Û ΩΩ∑‘¿‘¥œ¥Ÿ");
    }

    protected override void EndDragEvent()
    {
        EventHandler.CallDragItemInactiveEvent();
    }

    protected override void DropEvent()
    {
        EventHandler.CallDropItemeEvent(panel, type, index);
    }
}
