using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    // Interaction Point ���
    // 1. Interactor�� �ڽ����� InteractionPoint�� ������ �� InteractionPoint ��ũ��Ʈ���� activeScene�� inveractionType�� ����
    // 2. panels�� inveraction �� Ȱ��ȭ�� panel�� ����� �� ineractionPoint�� �����Ѱ� inveractionType ����

    [SerializeField] InteractorPanel[] panels;
    Dictionary<InteractionType, Panel> panelDic;
    InteractionType type;
    SceneName activeScene;

    private void Start()
    {
        panelDic = new Dictionary<InteractionType, Panel>();
        for (int i = 0; i < panels.Length; i++)
        {
            if (panelDic.ContainsKey(panels[i].type)) Debug.LogError($"Interactor.cs Error : panels�� {EnumCaching.ToString(panels[i].type)}�� �ߺ��Ǿ�����");
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
