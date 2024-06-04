using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lvTxt;
    [SerializeField] TextMeshProUGUI goldTxt;
    public void SetSlot(int level)
    {
        lvTxt.SetText(System.String.Format("Lv{0}", level));
        goldTxt.SetText(System.String.Format("{0:#,##0}G", level * 100));
    }
}
