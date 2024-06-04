using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SingleComTable : ComTable
{
    [SerializeField] UnityEvent<TableType, int> SetResultEvent;
    public override void PutCard(int cardNum, int index)
    {
        base.PutCard(cardNum, index);
        SetResultEvent?.Invoke(type, result);
    }

}
