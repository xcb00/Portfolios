using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] RectTransform pad;
    [SerializeField] RectTransform stick;
    RectTransform rect;

    [HideInInspector]
    public CharacterDirection inputDir;// { get; private set; }
    //public bool isDrag { get; private set; }
    public bool isMove { get; private set; }
    float stickRange = 0.0f;

    void OnEnable()
    {
        EventHandler.ChangeSettingEvent += SetSize;
    }
    void OnDisable()
    {
        EventHandler.ChangeSettingEvent -= SetSize;
    }

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    void SetSize()
    {
        // 조이스틱 크기 설정
        pad.sizeDelta = new Vector3(Settings.Instance.padSize, Settings.Instance.padSize, 0f);
        stick.sizeDelta = pad.sizeDelta * Settings.Instance.stickScale;

        stickRange = pad.rect.width * Settings.Instance.stickRange;

        rect.offsetMin = new Vector2(Settings.Instance.padSize, Settings.Instance.padSize) * 0.725f;
        rect.offsetMax = -rect.offsetMin;

        InActive();
    }

    #region Events
    public void OnDrag(PointerEventData eventData)
    {
        float x = eventData.position.x - pad.position.x;
        float y = eventData.position.y - pad.position.y;

        #region 드레그에 따른 스틱 위치 이동
        // 좌우로 드레그한 길이가 상하로 드레그한 길이보다 길 경우 스틱을 좌우로 이동
        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            stick.localPosition = new Vector3(Mathf.Clamp(x, -stickRange, stickRange), 0f, 0f);

            // stickDirection 변경
            if (x > 0)  inputDir = CharacterDirection.right;
            else        inputDir = CharacterDirection.left;
        }
        // 상하로 드레그한 길이가 좌우로 드레그한 길이보다 길 경우 스틱을 상하로 이동
        else
        {
            stick.localPosition = new Vector3(0f, Mathf.Clamp(y, -stickRange, stickRange), 0f);

            // stickDirection 변경
            if (y <= 0) inputDir = CharacterDirection.down;
            else inputDir = CharacterDirection.up;
        }
        #endregion

        // 스틱을 특정 범위 이상 움직였을 경우 플레이어를 움직이고 특정 범위 이하로 움직이면 방향만 변경
        // localPosition은 (x, 0, 0) 또는 (0, y, 0)이므로 x + y는 x 또는 y의 이동거리
        isMove = Mathf.Abs(stick.localPosition.x + stick.localPosition.y) > stickRange * Settings.Instance.minDistanceToMove;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pad.position = eventData.position;
        stick.position = eventData.position;
        pad.gameObject.SetActive(true);
        stick.gameObject.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InActive();
    }
    #endregion

    // 조이스틱 초기화
    public void InActive()
    {
        pad.position = Vector2.zero;
        stick.position = Vector3.zero;
        pad.gameObject.SetActive(false);
        stick.gameObject.SetActive(false);
        isMove = false;
    }/*

    public void OnStick() { gameObject.SetActive(true); }
    public void OffStick() { gameObject.SetActive(false); }*/
}
