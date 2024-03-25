using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSlot : Slot
{
    protected override void ClickEvent()
    {
        EventHandler.CallDragItemActiveEvent(panel, type, index);
    }
}
