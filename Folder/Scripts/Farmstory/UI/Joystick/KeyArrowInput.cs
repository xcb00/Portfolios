using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyArrowInput : MonoBehaviour
{
    [HideInInspector]
    public CharacterDirection inputDir;// { get; private set; }
    [SerializeField] Image keyDown;
    RectTransform rect;

    public bool isMove { get; private set; }

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            isMove = true;
            inputDir = CharacterDirection.up;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            isMove = true;
            inputDir = CharacterDirection.right;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            isMove = true;
            inputDir = CharacterDirection.down;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            isMove = true;
            inputDir = CharacterDirection.left;
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            isMove = false;
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            isMove = false;
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            isMove = false;
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            isMove = false;
        }

    }
#endif

    void OnEnable()
    {
        EventHandler.ChangeSettingEvent += SetSize;
    }
    void OnDisable()
    {
        EventHandler.ChangeSettingEvent -= SetSize;
    }

    void SetSize()
    {
        // 조이스틱 크기 설정
        rect.sizeDelta = new Vector3(Settings.Instance.padSize, Settings.Instance.padSize, 0f) * 1.5f;
        ArrowKeyUp();
    }

    public void DirectionInput(int dir)
    {
        isMove = true;
        inputDir = (CharacterDirection)dir;
        keyDown.transform.rotation = Quaternion.identity;
        keyDown.transform.Rotate(Vector3.forward * 90.0f * dir * (dir % 2 == 0 ? 1 : -1));
        keyDown.enabled = true;
    }

    public void ArrowKeyUp()
    {
        keyDown.enabled = false;
        isMove = false;
    }


}
