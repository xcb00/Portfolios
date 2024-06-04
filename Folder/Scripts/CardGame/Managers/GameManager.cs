using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int gold { get; private set; }
    public bool login = false;
    DataManager dataManager = null;
    GPGSManager gpgsManager = null;
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] MoneyMessage moneyMessage;
    [SerializeField] LoadingPanel loadingPanel;
    int betMoney;

    protected override void Awake()
    {
        base.Awake();

        if (dataManager == null)
            dataManager = GetComponent<DataManager>();

        if(gpgsManager==null)
            gpgsManager = GetComponent<GPGSManager>();

        UnityEngine.SceneManagement.SceneManager.LoadScene(EnumCaching.ToString(SceneName.Regist), UnityEngine.SceneManagement.LoadSceneMode.Additive);

        LoadData();
    }
    void OnEnable()
    {
        EventHandler.ShowMoneyMessageEvent += ShowMoneyMessageEvent;
        EventHandler.LoadingPanelInactiveEvent += LoadingPanelInactive;
    }
    void OnDisable()
    {
        EventHandler.ShowMoneyMessageEvent -= ShowMoneyMessageEvent;
        EventHandler.LoadingPanelInactiveEvent -= LoadingPanelInactive;
    }
    void ShowMoneyMessageEvent(string msg) => moneyMessage.Active(msg);


    public void StartGame()
    {
        gpgsManager.StartGoogleSDK();
        LoadData();
    }

    public void StartPlaying(int bet) => betMoney = bet;

    public void SetGameRessult(GameResult result, bool isSingle, bool perfect)
    {
        LoadData();
        if (isSingle)
            dataManager.UpdateSingleRecord(result);
        else
            dataManager.UpdateMultiRecord(result);

        int _money = perfect ? betMoney * 2 : betMoney;

        if (result == GameResult.Draw) _money = 0;
        else if (result == GameResult.Lose)
            _money = _money > gold ? -gold : -_money;

        AddGold(_money);
    }

    public void SaveData()
    {
        dataManager.UpdatePlayerData(gold);
        dataManager.SavePlayerData();
    }

    public void LoadData()
    {
        dataManager.LoadPlayerData();

        gold = dataManager.GetPlayerGold;

        UpdateGoldText();
    }

    public void AddGold(int _g)
    {
        gold += _g;
        SaveData();
        UpdateGoldText();
    }

    void UpdateGoldText() => goldText.SetText(System.String.Format("{0}G", gold.ToString("N0")));

    public void ShowBanner() => gpgsManager.LoadBanner();
    public void DestoryBanner() => gpgsManager.DestoryBanner();
    public void ShowReward(bool single = false)
    {
        int rewardGold = single ? Mathf.RoundToInt(betMoney * 0.5f) : 500;
        gpgsManager.ShowReward(rewardGold, (g) => AddGold(g));
    }

    public void ShowPolicy() => Application.OpenURL("https://sites.google.com/view/geon-portfolio/terms");


    public void SaveCloud()
    {
        loadingPanel.gameObject.SetActive(true);
        gpgsManager.SaveData(dataManager.PlayerDataToByte());
    }

    public void LoadCloud()
    {
        loadingPanel.gameObject.SetActive(true);
        gpgsManager.LoadData();
    }

    public void LoadCloudData(byte[] data)
    {
        dataManager.ByteToPlayerData(data);
        EventHandler.CallShowMoenyMessageEvent(dataManager.GetPlayerGold.ToString());
        UpdateGoldText();
        SaveData();
    }

    void LoadingPanelInactive() => loadingPanel.gameObject.SetActive(false);

    public Vector3Int GetSingleRecord => dataManager.GetPlayerSingle;
    public Vector3Int GetMultiRectord => dataManager.GetPlayerMulti;
}
