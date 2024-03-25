using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MinimapUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public UnityEvent<Vector2> dragEvent = null;
    public UnityEvent<float> pinchEvent = null;

    bool isDrag = false;
    bool isPinch = false;
    Touch first;
    Touch second;

    public void OnDrag(PointerEventData eventData)
    {
        if (isPinch)
        {
            // 화면에 터치된 두 손가락을 가져옴
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // 각 손가락의 이전 프레임에서의 위치를 구함
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // 현재 프레임과 이전 프레임에서의 두 손가락 사이 거리를 구함
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // 두 거리의 차이를 구함
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            Debug.LogWarning(deltaMagnitudeDiff);

            pinchEvent?.Invoke(deltaMagnitudeDiff);
        }
        else
        {
            dragEvent?.Invoke(eventData.delta);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //if (!isDrag)
        //{
            if (Input.touchCount == 1)
            {
                isPinch = false;
                first = Input.GetTouch(0);
                Debug.Log("Drag");
            }
            else if (Input.touchCount > 1)
            {
                isPinch = true;
                first = Input.GetTouch(0);
                second = Input.GetTouch(1);
                Debug.Log("Pinch");
            }

            isDrag = true;
        //}
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDrag = false;
        isPinch = false;
    }

    public void ActiveMinimap() => gameObject.SetActive(true);
    public void InactiveMinimap() => gameObject.SetActive(false);
}
