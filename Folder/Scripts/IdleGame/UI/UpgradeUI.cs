using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public Transform slotTrans;

    UpgradeSlot[] slots;

    private void Awake()
    {
        slots = new UpgradeSlot[(int)UpgradeValue.count];
        for(int i=0;i<slots.Length;i++)
            slots[i] = slotTrans.GetChild(i).GetComponent<UpgradeSlot>();
    }

    private void OnEnable()
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].SetSlot(DataManager.inst.GetUpgradeLevel(i));
    }

    public void UpgradeBtn(int upgradeValue)
    {
        if (!DataManager.inst.UseGold(DataManager.inst.GetUpgradeLevel(upgradeValue) * 100))
            return;

        DataManager.inst.UpgradeLevelUp(upgradeValue);
        slots[upgradeValue].SetSlot(DataManager.inst.GetUpgradeLevel(upgradeValue));
        DataManager.inst.SavePlayerData();
    }
}
