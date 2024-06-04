using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleResult : ResultPanel
{
    [SerializeField] GameObject adBtn;

    public override void SetResultPanel(GameResult result, bool perfect = false)
    {
        adBtn.SetActive(result == GameResult.Lose);
        base.SetResultPanel(result, perfect);
    }

    public void RestartButton()
    {
        if (GameManager.Inst.gold < 10)
            EventHandler.CallShowMoenyMessageEvent("Not enough money");
        else
            SceneChange.ChangeScene(SceneName.Single, SceneName.Single, null, null, null, null);
    }

    public void LobbyButton()=> SceneChange.ChangeScene(SceneName.Single, SceneName.Lobby, null, null, null, null);

    public void AdButton()
    {
        adBtn.SetActive(false);
        GameManager.Inst.ShowReward(true);
    }
}
