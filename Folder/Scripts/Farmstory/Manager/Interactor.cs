using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    // Interaction Point 등록
    // 1. Interactor의 자식으로 InteractionPoint를 생성한 후 InteractionPoint 스크립트에서 activeScene과 inveractionType을 설정
    // 2. panels에 inveraction 시 활성화할 panel을 등록한 후 ineractionPoint와 동일한게 inveractionType 설정

    [SerializeField] InteractorPanel[] panels;
    Dictionary<InteractionType, Panel> panelDic;
    InteractionType type;
    SceneName activeScene;

    private void Start()
    {
        panelDic = new Dictionary<InteractionType, Panel>();
        for (int i = 0; i < panels.Length; i++)
        {
            if (panelDic.ContainsKey(panels[i].type)) Debug.LogError($"Interactor.cs Error : panels에 {EnumCaching.ToString(panels[i].type)}이 중복되어있음");
            else panelDic.Add(panels[i].type, panels[i].panel);
        }
    }

    private void OnEnable()
    {
        EventHandler.SetInteractionTypeEvent += SetInteractionType;
        EventHandler.OpenPanelEvent += OpenPanel;
    }
    private void OnDisable()
    {
        EventHandler.SetInteractionTypeEvent -= SetInteractionType;
        EventHandler.OpenPanelEvent -= OpenPanel;
    }

    void SetInteractionType(InteractionType type, SceneName scene)
    {
        EventHandler.CallActiveInteractor(true);
        this.type = type;
        activeScene = scene;
    }

    void OpenPanel()
    {
        if (activeScene == GameDatas.currentScene)
        {
            panelDic[type].ActivePanel();
            EventHandler.CallActiveInteractor(false);
        }
    }
}
