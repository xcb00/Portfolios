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
        // ���̽�ƽ ũ�� ����
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

        #region �巹�׿� ���� ��ƽ ��ġ �̵�
        // �¿�� �巹���� ���̰� ���Ϸ� �巹���� ���̺��� �� ��� ��ƽ�� �¿�� �̵�
        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            stick.localPosition = new Vector3(Mathf.Clamp(x, -stickRange, stickRange), 0f, 0f);

            // stickDirection ����
            if (x > 0)  inputDir = CharacterDirection.right;
            else        inputDir = CharacterDirection.left;
        }
        // ���Ϸ� �巹���� ���̰� �¿�� �巹���� ���̺��� �� ��� ��ƽ�� ���Ϸ� �̵�
        else
        {
            stick.localPosition = new Vector3(0f, Mathf.Clamp(y, -stickRange, stickRange), 0f);

            // stickDirection ����
            if (y <= 0) inputDir = CharacterDirection.down;
            else inputDir = CharacterDirection.up;
        }
        #endregion

        // ��ƽ�� Ư�� ���� �̻� �������� ��� �÷��̾ �����̰� Ư�� ���� ���Ϸ� �����̸� ���⸸ ����
        // localPosition�� (x, 0, 0) �Ǵ� (0, y, 0)�̹Ƿ� x + y�� x �Ǵ� y�� �̵��Ÿ�
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

    // ���̽�ƽ �ʱ�ȭ
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
