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
            // ȭ�鿡 ��ġ�� �� �հ����� ������
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // �� �հ����� ���� �����ӿ����� ��ġ�� ����
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // ���� �����Ӱ� ���� �����ӿ����� �� �հ��� ���� �Ÿ��� ����
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // �� �Ÿ��� ���̸� ����
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
