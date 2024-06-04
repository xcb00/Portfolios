using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UIDictionary
{
    public UIType type;
    public CanvasGroup group;
}

public class UIManager : MonoBehaviour
{
    [SerializeField] UIDictionary[] uiDicStruct;
    Dictionary<UIType, CanvasGroup> uiDic;
    private void Awake()
    {
        if (uiDic == null)
            InitUIDictionary();
    }

    private void OnEnable()
    {
        EventHandler.ActiveUIPanelEvent += ActiveUIPanel;
        //EventHandler.SetUIAlphaEvent += SetUIAlpha;
    }

    private void OnDisable()
    {
        EventHandler.ActiveUIPanelEvent -= ActiveUIPanel;
        //EventHandler.SetUIAlphaEvent -= SetUIAlpha;
    }

    void InitUIDictionary()
    {
        uiDic = new Dictionary<UIType, CanvasGroup>();
        foreach (UIDictionary ui in uiDicStruct)
            if(ui.type!=UIType.None) uiDic[ui.type] = ui.group;
    }

    void ActiveUIPanel(UIType type, bool active)
    {
        if (!uiDic.ContainsKey(type)) return;
        uiDic[type].alpha = active ? 1.0f : 0.0f;
        uiDic[type].interactable = active;
        uiDic[type].blocksRaycasts = active;
        uiDic[type].gameObject.SetActive(active);
    }

    void SetUIAlpha(UIType type, bool active)
    {
        if (!uiDic.ContainsKey(type)) return;
        uiDic[type].alpha = active ? 1.0f : 0.0f;
        uiDic[type].interactable = active;
        uiDic[type].blocksRaycasts = active;
    }
}
