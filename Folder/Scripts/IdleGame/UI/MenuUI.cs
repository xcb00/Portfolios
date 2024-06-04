using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI goldTxt;

    private void OnEnable() { EventHandler.UpdateGoldEvent += UpdateGold; UpdateGold(); }
    private void OnDisable() => EventHandler.UpdateGoldEvent -= UpdateGold;

    void UpdateGold() 
    {
        if (DataManager.inst != null) 
            goldTxt.SetText(System.String.Format("{0:#,##0}G", DataManager.inst.GetGold)); 
    }

    public void ActiveUI(int menuIdx)
    {
        EventHandler.CallActiveUIPanelEvent(UIType.Main, menuIdx==2);
        EventHandler.CallActiveUIPanelEvent(UIType.Upgrade, menuIdx==1);
        EventHandler.CallActiveUIPanelEvent(UIType.Offline, menuIdx==3);
    }
}
