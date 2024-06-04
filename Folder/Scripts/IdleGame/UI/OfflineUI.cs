using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OfflineUI : MonoBehaviour
{
    [SerializeField] Button offlineBtn;
    [SerializeField] TextMeshProUGUI offlineRewardTxt;
    int timeDiff = 9;
    int reward;

    public void CalculateOffline(Vector2Int startTime)
    {
        offlineBtn.interactable = true;
        Vector2Int exitTime = DataManager.inst.GetExitTime;
        int result = (startTime.x - exitTime.x + timeDiff) * 60 + (startTime.y - exitTime.y);
        result = result < 0 ? 1440 - result : result;
        reward = (result > 180 ? 180 : result) * 10;
        offlineRewardTxt.SetText(System.String.Format("{0:#,##0}G", reward));
    }

    public void GetOfflineReward()
    {
        offlineBtn.interactable = false;
        DataManager.inst.UseGold(reward, true);
    }
}
