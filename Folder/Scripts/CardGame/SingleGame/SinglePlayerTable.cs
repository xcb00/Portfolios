using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SinglePlayerTable : TableSript, IDropHandler
{
    [SerializeField] protected UnityEvent<TableType, int> SetResultEvent;
    public override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);
        SetResultEvent?.Invoke(type, result);
    }
}
