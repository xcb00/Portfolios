using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleTxt;
    [SerializeField] TextMeshProUGUI rewardTxt;
    [SerializeField] TextMeshProUGUI maxStageTxt;

    private void OnEnable() => EventHandler.OpenRewardWindonEvent += OpenRewardWindow;
    private void OnDisable() => EventHandler.OpenRewardWindonEvent -= OpenRewardWindow;

    void OpenRewardWindow(int maxStage, int reward)
    {
        titleTxt.SetText(maxStage > StaticVariables.inst.stageSO.stages.Length ? "Succeeded" : "Failed");

        if (maxStage > StaticVariables.inst.stageSO.stages.Length)
        {
            maxStage = StaticVariables.inst.stageSO.stages.Length;
            titleTxt.color = Color.green;
        }
        else 
            titleTxt.color = Color.red;

        reward = Mathf.RoundToInt(reward * (9 + DataManager.inst.GetUpgradeLevel((int)UpgradeValue.Gold)) * 0.1f);
        DataManager.inst.UseGold(reward, true);
        rewardTxt.SetText(reward.ToString());
        maxStageTxt.SetText(maxStage.ToString());
    }

    public void ConfirmBtn()
    {
        EventHandler.CallActiveUIPanelEvent(UIType.Reward, false);
    }
}
