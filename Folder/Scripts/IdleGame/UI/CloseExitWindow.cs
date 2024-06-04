using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CloseExitWindow : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent<bool> OnClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(false);
    }
}
