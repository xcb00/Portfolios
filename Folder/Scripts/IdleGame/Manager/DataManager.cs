using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

[System.Serializable]
public struct Data<T> // 리스트 형식의 데이터를 Json으로 저장하기 위한 구조체
{
    public List<T> dataList;
    public Data(List<T> list) => dataList = list;
}

[System.Serializable]
public class PlayerData
{
    public int gold;
    public Vector2Int exitTime;
    public Vector3Int attanceData;
    public List<int> upgrades;

    public PlayerData() { }

    public PlayerData(int gold, Vector3Int attanceData, Vector2Int exitTime, List<int> upgrades)
    {
        this.gold = gold;
        this.attanceData = attanceData;
        this.exitTime = exitTime;
        this.upgrades = upgrades;
    }   
}

[System.Serializable]
public struct PlayerDataStruct
{
    public int gold;
    public Vector2Int exitTime;
    public Vector3Int attanceData;
    public string upgradesString;

    public PlayerDataStruct(int gold, Vector3Int attanceData, Vector2Int exitTime, string upgradesString)
    {
        this.gold = gold;
        this.attanceData = attanceData;
        this.exitTime = exitTime;
        this.upgradesString = upgradesString;
    }
}

public class DataManager : Singleton<DataManager>
{
    public int today;
    [SerializeField]
    PlayerData playerData;
    const string FILEPATHFORMAT = "{0}/PlayerDataJson.txt";

    public Vector2Int GetExitTime => playerData.exitTime;
    public int GetGold => playerData.gold;

    public int GetUpgradeLevel(int upgradeValue) => playerData.upgrades[upgradeValue];


    public void UpgradeLevelUp(int upgradeValue) => playerData.upgrades[upgradeValue]++;

    protected override void Awake()
    {
        base.Awake();

        LoadPlayerData();
    }

    void OnApplicationPause(bool pause)
    {
        SavePlayerData();
    }

    Vector2Int GetDateTimeToTime() => new Vector2Int(int.Parse(DateTime.Now.ToString("HH")), int.Parse(DateTime.Now.ToString("mm")));

    public void SavePlayerData() => File.WriteAllText(System.String.Format(FILEPATHFORMAT, Application.persistentDataPath), Encryption(PlayerDataToJson(playerData)));
    public void LoadPlayerData()
    {
        string path = System.String.Format(FILEPATHFORMAT, Application.persistentDataPath);
        if (!File.Exists(path))
        {
            List<int> upgrades = new List<int>();
            for (int i = 0; i < (int)UpgradeValue.count; i++)
                upgrades.Add(1);
            playerData = new PlayerData(100, Vector3Int.zero, GetDateTimeToTime(), upgrades);
            return;
        }

        playerData = JsonToPlayerData(Decryption(File.ReadAllText(path)));
        EventHandler.CallUpdateGoldEvent();
    }

    // 암호화
    string Encryption(string json)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        return System.Convert.ToBase64String(bytes);
    }

    // 복호화
    string Decryption(string code)
    {
        byte[] bytes = System.Convert.FromBase64String(code);
        return Encoding.UTF8.GetString(bytes);
    }

    string UpgradeDataToJson(List<int> upgrades) => JsonUtility.ToJson(new Data<int>(upgrades));
    string PlayerDataToJson(PlayerData data) => JsonUtility.ToJson(new PlayerDataStruct(data.gold, data.attanceData, GetDateTimeToTime(), UpgradeDataToJson(data.upgrades)));

    List<T> JsonToList<T>(string json) => (JsonUtility.FromJson<Data<T>>(json)).dataList;
    PlayerData JsonToPlayerData(string json)
    {
        PlayerDataStruct loadData = JsonUtility.FromJson<PlayerDataStruct>(json);
        return new PlayerData(loadData.gold, loadData.attanceData, loadData.exitTime, JsonToList<int>(loadData.upgradesString));
    }

    public bool UseGold(int gold, bool addGold = false)
    {
        if (addGold)
        {
            playerData.gold += gold;
            SavePlayerData();
            EventHandler.CallUpdateGoldEvent();
            return true;
        }

        if (playerData.gold < gold)
            return false;

        playerData.gold -= gold;
        EventHandler.CallUpdateGoldEvent();
        return true;
    }

}
