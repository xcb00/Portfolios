using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ArrowPadButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent keyDown = null;
    public UnityEvent keyUp = null;

    public void OnPointerDown(PointerEventData eventData)
    {
        keyDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        keyUp?.Invoke();
    }
}
