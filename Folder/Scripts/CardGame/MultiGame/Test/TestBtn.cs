using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int verticalInput = 1;
    public UnityEvent<int> buttonInputEvent;
    Image input;

    public void OnPointerDown(PointerEventData eventData) => buttonInputEvent?.Invoke(verticalInput);

    public void OnPointerUp(PointerEventData eventData) => buttonInputEvent?.Invoke(0);

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<Image>();
        input.rectTransform.sizeDelta = new Vector2(Screen.width * 0.5f, Screen.height);
    }

}
