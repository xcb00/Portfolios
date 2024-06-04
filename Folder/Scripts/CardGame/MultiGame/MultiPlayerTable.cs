using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MultiPlayerTable : TableSript
{

    [SerializeField] protected UnityEvent<Vector2Int, int> SetResultEvent;
    Vector2Int tableIdx = Vector2Int.zero;
    public override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);
        tableIdx.x = (int)type;
        tableIdx.y = cardIdx - 1;
        SetResultEvent?.Invoke(tableIdx, cardNum);
    }
}
