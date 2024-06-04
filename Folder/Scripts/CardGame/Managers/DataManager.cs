using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    bool encryption = false;
    PlayerData playerData;
    const string FILEPATHFORMAT = "{0}/PlayerDataJson.txt";

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SavePlayerData();
    }

    #region Save
    string PlayerDataToJson() => JsonUtility.ToJson(new PlayerDataJson(playerData)); 
    string Encryption(string json)
    {
        if (!encryption) return json;

        byte[] bytes = Encoding.UTF8.GetBytes(json);
        return System.Convert.ToBase64String(bytes);
    }

    public byte[] PlayerDataToByte()=> Encoding.UTF8.GetBytes(PlayerDataToJson());

    public void SavePlayerData() => File.WriteAllText(System.String.Format(FILEPATHFORMAT, Application.persistentDataPath), Encryption(PlayerDataToJson()));
    #endregion

    #region Load
    PlayerData JsonToPlayerData(string json) => new PlayerData(JsonUtility.FromJson<PlayerDataJson>(json));
    string Decryption(string code)
    {
        if(!encryption) return code;

        byte[] bytes = System.Convert.FromBase64String(code);
        return Encoding.UTF8.GetString(bytes);
    }

    public void ByteToPlayerData(byte[] bytes) => playerData = JsonToPlayerData(Encoding.UTF8.GetString(bytes));

    public void LoadPlayerData()
    {
        string path = System.String.Format(FILEPATHFORMAT, Application.persistentDataPath);

        if (!File.Exists(path))
        {
            playerData = new PlayerData();
            SavePlayerData();
            return;
        }

        playerData = JsonToPlayerData(Decryption(File.ReadAllText(path)));
    }
    #endregion

    #region Get Player Data
    public int GetPlayerGold => playerData.gold;
    public Vector3Int GetPlayerSingle => playerData.singleRecord;
    public Vector3Int GetPlayerMulti => playerData.multiRecord;
    #endregion

    #region UpdatePlayerData

    public void UpdatePlayerData(int gold) => playerData.gold = gold;

    public void UpdateSingleRecord(GameResult result) => playerData.singleRecord += AddRecord(result);

    public void UpdateMultiRecord(GameResult result) => playerData.multiRecord += AddRecord(result);

    Vector3Int AddRecord(GameResult result)
    {
        switch (result)
        {
            case GameResult.Win:return Vector3Int.right;
            case GameResult.Draw:return Vector3Int.up;
            case GameResult.Lose:return Vector3Int.forward;
            default: return Vector3Int.zero;
        }
    }
    #endregion
}
