using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    StringBuilder strBuilder = new StringBuilder();
    bool encryption = false;
    public bool PreviousDataExist { get; private set; }
    private void OnEnable()
    {
        EventHandler.DBDataLoadEvent += FirstLoadData;
        EventHandler.BeforeSceneUnloadFadeOutEvent += SaveMapTileData;
        EventHandler.BeforeSceneUnloadFadeOutEvent += SavePrefabData;
        EventHandler.BeforeSceneUnloadFadeOutEvent += SaveInventoryData;
    }

    private void OnDisable()
    {
        EventHandler.DBDataLoadEvent -= FirstLoadData;
        EventHandler.BeforeSceneUnloadFadeOutEvent -= SaveMapTileData;
        EventHandler.BeforeSceneUnloadFadeOutEvent -= SavePrefabData;
        EventHandler.BeforeSceneUnloadFadeOutEvent -= SaveInventoryData;
    }



    #region Utility
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

    public bool ExceptScene(SceneName scene)
    {
        // 농사와 관련된 정보(CropData, DugTile, WaterTile을 저장하는 씬일 경우 true 반환
        /*switch (scene)
        {
            case SceneName.Home:
            case SceneName.Shop:
            case SceneName.Casino:
            case SceneName.Village:
            case SceneName.Lobby:
            case SceneName.Beach:
                return false;
            default: return true;
        }*/
        return scene == SceneName.Farm;
    }

    //string GetFilePath(string name) => $"{Application.persistentDataPath}/{name}.txt";
    public string GetFilePath(JsonDataName name, SceneName scene)
    {
        strBuilder.Clear();
        strBuilder.Append(Application.persistentDataPath);
        strBuilder.Append("/");
        strBuilder.Append(EnumCaching.ToString(name));
        if (scene != SceneName.count)
        {
            strBuilder.Append("_");
            strBuilder.Append(EnumCaching.ToString(scene));
        }
        strBuilder.Append(".txt");
        return strBuilder.ToString();
    }
    #endregion

    #region DataToJson
    public void SaveJson(JsonDataName name, string json, SceneName scene = SceneName.count)
    {
        if (encryption) File.WriteAllText(GetFilePath(name, scene), Encryption(json));
        else File.WriteAllText(GetFilePath(name, SceneName.count), json);
    }


    public string DBJson(JsonDataName name, ref bool getData)
    {
        string path = GetFilePath(name, SceneName.count);

        if (!File.Exists(path)) { getData = true; return string.Empty; }
        else getData = false;

        if (encryption)
            return Decryption(File.ReadAllText(path));
        else
            return File.ReadAllText(path);
    }

    public void SaveDataToJson<T>(JsonDataName name, List<T> data, SceneName scene = SceneName.count)
    {
        string json = JsonUtility.ToJson(new Data<T>(data));
        if (encryption) File.WriteAllText(GetFilePath(name, scene), Encryption(json));
        else File.WriteAllText(GetFilePath(name, scene), json);
    }

    public List<T> LoadDataToJson<T>(JsonDataName name, SceneName scene = SceneName.count)
    {
        string path = GetFilePath(name, scene);
        if (!File.Exists(path))
            return new List<T>();

        if (encryption)
            return JsonUtility.FromJson<Data<T>>(Decryption(File.ReadAllText(path))).dataList;
        else
            return JsonUtility.FromJson<Data<T>>(File.ReadAllText(path)).dataList;
    }

    public string LoadDataToString<T>(JsonDataName name, SceneName scene = SceneName.count)
    {
        string path = GetFilePath(name, scene);
        if (!File.Exists(path))
            return string.Empty;

        if (encryption)
            return Decryption(File.ReadAllText(path));
        else
            return File.ReadAllText(path);
    }
    #endregion
    #region Delete Json
    public void DeleteJson()
    {
        InitPlayerData();
        InitTimeData();
        PlayerPrefs.SetInt("PlayerHP", 5);
        //ResetScaneryPrefabData();
        ResetSetting();
        // 출석 데이터 초기화
        File.Delete(GetFilePath(JsonDataName.Attendance, SceneName.count));
        File.Delete(GetFilePath(JsonDataName.MineData, SceneName.count));
        GameDatas.mineDataList.Clear();
        for (int i=0;i<(int)SceneName.count;i++)
        {
            // 인벤토리 초기화
            if(i<(int)Inventories.cnt)
                InitInventory((Inventories)i);



            // 씬의 타일/Crop 데이터 초기화
            if (!ExceptScene((SceneName)i)) continue;
            //if (i!=(int)SceneName.Farm) continue;
            else 
            {
                File.Delete(GetFilePath(JsonDataName.DugTile, (SceneName)i));
                File.Delete(GetFilePath(JsonDataName.WaterTile, (SceneName)i));
                File.Delete(GetFilePath(JsonDataName.CropTile, (SceneName)i));
                File.Delete(GetFilePath(JsonDataName.CropPrefab, (SceneName)i));
                File.Delete(GetFilePath(JsonDataName.TreePrefab, (SceneName)i));
            }
        }
    }

    void ResetSetting()
    {
        Settings.Instance.padSize = 150f;
        Settings.Instance.activeCursor = true;
        Settings.Instance.volume = 10;
        Settings.Instance.activeToolMenu = true;
        Settings.Instance.useJoystick = false;
        Settings.Instance.interactingAnimation = true;
        SaveSettingValue();
    }
    #endregion

    #region Load Data : Json 데이터를 로드해 게임에서 사용할 수 있도록 변환하는 함수
    // DB 데이터가 로드된 후 Json 형식으로 저장된 데이터를 GameDatas에 저장하는 함수
    // GameDatas에 있는 데이터 리스트들을 초기화함
    void FirstLoadData()
    {
        // PlayerData
        GameDatas.playerData = LoadDataToJson<PlayerData>(JsonDataName.PlayerPositionData);
        // 저장된 데이터가 없다면 PlayerData 초기화
        if (GameDatas.playerData.Count < 1)
        {
            InitPlayerData();
            PreviousDataExist = false;
        }
        else
        {
            PreviousDataExist = true;
        }

        // BankData
        //List<BankData> bankData = LoadDataToJson<BankData>(JsonDataName.BankData);
        //if (bankData.Count < 1) GameDatas.loan = new Vector2Int(Settings.Instance.startLoan, Mathf.RoundToInt(Settings.Instance.startLoan * Settings.Instance.loanInterest * 0.01f));

        // TimeData
        List<TimeData> time = LoadDataToJson<TimeData>(JsonDataName.TimeData);
        // 저장된 데이터가 없다면 데이터 초기화
        if (time.Count < 1) InitTimeData();
        // 저장된 데이터가 있다면 GameDatas에 저장
        else ChangeTimeData(time[0].YearSeasonDay, time[0].HourMinuteGold, time[0].VisitShop, time[0].Loan);
        // 저장된 시간이 낮 시간이라면 NightMask 비활성화
        //if (GameDatas.HourMinuteGold.x > 7 && GameDatas.HourMinuteGold.x < 19) EventHandler.CallNightEvent(false);
        // 저장된 시간이 저녁 시간이라면 NightMask 활성화
        //else EventHandler.CallNightEvent(true);
        EventHandler.CallNightEvent();

        // GameDatas의 List들 초기화
        // GC를 최소화하기 위해 최초 실행 시 리스트를 선언하고, 이후 Clear를 사용
        GameDatas.mapTileList = new List<MapTileData>[(int)TilemapType.count - 1];
        for (int i = 0; i < (int)TilemapType.count - 1; i++)
            GameDatas.mapTileList[i] = new List<MapTileData>();
        GameDatas.cropTileList = new List<CropTileDetails>();

        GameDatas.cropPrefabList = new List<CropPrefab>();

        GameDatas.removeCropCoordinateList = new List<Vector2Int>();

        GameDatas.scaneryPrefabList = new List<O_ScaneryPrefab>();

        GameDatas.mineDataList = LoadDataToJson<MineDatas>(JsonDataName.MineData);

        GameDatas.treePrefabList = new List<TreePrefabData>();

        CreateInventories();
    }

    void CreateInventories()
    {
        GameDatas.inventories = new List<InventoryItem>[(int)Inventories.cnt];

        // 인벤토리의 용량만큼 빈 슬롯을 생성
        for (int i = 0; i < GameDatas.inventories.Length; i++)
        {
            GameDatas.inventories[i] = new List<InventoryItem>();

            if (Settings.Instance.inventoryCapacity[i].type != (Inventories)i) continue;

            for (int j = 0; j < Settings.Instance.inventoryCapacity[i].capacity; j++)
                GameDatas.inventories[i].Add(new InventoryItem());
        }
    }


    public void LoadCropTileData(SceneName scene)
    {
        for (int i = 0; i < GameDatas.mapTileList.Length; i++)
            GameDatas.mapTileList[i].Clear();

        //GameDatas.mapTileList[(int)TilemapType.dugGround - 1] = LoadDataToJson<MapTileData>(JsonDataName.DugTile, scene);
        //GameDatas.mapTileList[(int)TilemapType.waterGround - 1] = LoadDataToJson<MapTileData>(JsonDataName.WaterTile, scene);
        //GameDatas.mapTileList[(int)TilemapType.dynamicCollider - 1] = LoadDataToJson<MapTileData>(JsonDataName.DynaicColliderTile, scene);

        GameDatas.cropTileList.Clear();
        GameDatas.cropTileList = LoadDataToJson<CropTileDetails>(JsonDataName.CropTile, scene);
    }

    public void LoadCropPrefabData(SceneName scene)
    {
        if (!ExceptScene(GameDatas.currentScene))
        /*if (GameDatas.currentScene!=SceneName.Farm)
            return;*/

        GameDatas.cropPrefabList.Clear();
        List<CropPrefabJson> load = LoadDataToJson<CropPrefabJson>(JsonDataName.CropPrefab, scene);
        foreach (CropPrefabJson data in load)
            GameDatas.cropPrefabList.Add(new CropPrefab(data));
        //GameDatas.cropPrefabList = 
    }

    public void LoadScaneryPrefabData(SceneName scene)
    {
        if (!ExceptScene(GameDatas.currentScene))
            return;

        /*if (!File.Exists(GetFilePath(JsonDataName.ScaneryPrefab, GameDatas.currentScene)))
            ResetScaneryPrefabData();*/

        GameDatas.scaneryPrefabList.Clear();
        List<ScaneryPrefabJson> load = LoadDataToJson<ScaneryPrefabJson>(JsonDataName.ScaneryPrefab, scene);
        foreach (ScaneryPrefabJson data in load) 
        {
            O_ScaneryPrefab _prefab = new O_ScaneryPrefab();
            //GameDatas.scaneryPrefabList.Add(_prefab.AddList(data));
        }
    } 

    public void LoadInventory()
    {
        for (int i = 0; i < (int)Inventories.cnt; i++) LoadInventory(i);

        /*foreach (InventoryItem item in GameDatas.inventories[0])
            Debug.Log($"{item.itemCode} : {item.quantity} ({item.stackable})");*/
    }

    public void LoadInventory(int type)
    {
        InitInventory((Inventories)type);
        List<JsonInventoryData> load = LoadDataToJson<JsonInventoryData>(JsonDataName.InventoryData, (SceneName)type);
        foreach (JsonInventoryData data in load)
        {
            InventoryItem item = GameDatas.inventories[type][data.index];
            if (data.itemCode <= 0) continue;
            if (data.quantity <= 0) continue;

            item.itemCode = data.itemCode;
            item.stackable = GameDatas.itemDetailsList.Find(x => x.code == data.itemCode).isStackable;
            item.quantity = item.stackable ? data.quantity : 1;
            GameDatas.inventories[type][data.index] = item;
        }
    }

    public AttendanceData LoadAttendance()
    {
        if (!File.Exists(GetFilePath(JsonDataName.Attendance, SceneName.count))) return new AttendanceData(-1, 0, 0);
        else return LoadDataToJson<AttendanceData>(JsonDataName.Attendance)[0];
    }

    public void LoadSettingValue()
    {
        if (!File.Exists(GetFilePath(JsonDataName.SettingValue, SceneName.count)))
        {
            ResetSetting();
            return;
        }
        else
        {
            List<SettingValues> load = LoadDataToJson<SettingValues>(JsonDataName.SettingValue);
            Settings.Instance.activeCursor = load[0].activeCursor;
            Settings.Instance.volume = load[0].volume;
            Settings.Instance.padSize = load[0].stickSize;
            Settings.Instance.activeToolMenu = load[0].activeToolMenu;
            Settings.Instance.useJoystick = load[0].useJoystick;
            Settings.Instance.interactingAnimation = load[0].interactingAnimation;
        }

        EventHandler.CallChangeSetingEvent();
    }

    public void LoadTreePrefabData(SceneName scene)
    {
        GameDatas.treePrefabList.Clear();
        GameDatas.treePrefabList = LoadDataToJson<TreePrefabData>(JsonDataName.TreePrefab, scene);
    }
        
    #endregion

    #region Change Data : GameDatas에 있는 데이터를 변경하는 함수

    public void ChangePlayerData(PlayerData data, bool saveJson = false)
    {
        GameDatas.playerData[0] = data;
        GameDatas.currentScene = data.scene;
        if (saveJson)
            SavePlayerData();
        SaveTimeData();
    }

    public void ChangeTimeData(Vector3Int ysd, Vector3Int hmg, Vector3Int visitShop, Vector2Int loan)
    {
        ChangeYSD(ysd);
        ChangeHMG(hmg);
        ChangeLoan(loan);
        GameDatas.VisitShop = visitShop;
    }

    public void ChangeYSD(Vector3Int ysd) { GameDatas.YearSeasonDay = ysd; }
    public void ChangeHMG(Vector3Int hmg) { GameDatas.HourMinuteGold = hmg; }
    public void ChangeLoan(Vector2Int loan) { GameDatas.loan = loan; }


    #endregion

    #region Save Data to Json : GameDatas에 있는 데이터를 Json 형식으로 저장
    #region System Data
    public void SavePlayerData() { SaveDataToJson(JsonDataName.PlayerPositionData, GameDatas.playerData); }
    public void SaveTimeData() 
    {
        List<TimeData> timeData = new List<TimeData>();
        timeData.Add(new TimeData(GameDatas.YearSeasonDay, GameDatas.HourMinuteGold, GameDatas.VisitShop, GameDatas.loan));
        SaveDataToJson(JsonDataName.TimeData, timeData);

    }
    #endregion
    #region Map Data
    public void SaveDugTileData()
    {

        //if (GameDatas.currentScene != SceneName.Farm) return;

        // 현재 씬의 DugTileData 저장
        SaveDataToJson(JsonDataName.DugTile, GameDatas.mapTileList[(int)TilemapType.dugGround - 1], GameDatas.currentScene);
    }

    public void SaveWaterTileData()
    {
        // 현재 씬의 WaterTileData 저장
        SaveDataToJson(JsonDataName.WaterTile, GameDatas.mapTileList[(int)TilemapType.waterGround - 1], GameDatas.currentScene);
    }

    public void SaveCropTileData()
    {
        if (GameDatas.cropTileList.Count > 0)
        {
            foreach (CropTileDetails tile in GameDatas.cropTileList)
                tile.SetLastDay();
            
        }
        SaveDataToJson(JsonDataName.CropTile, GameDatas.cropTileList, GameDatas.currentScene);
    }

    public void SaveMapTileData()
    {
        if (!ExceptScene(GameDatas.currentScene))
            return;
        SaveDugTileData();
        SaveWaterTileData();
        SaveCropTileData();
    }
    #endregion
    #region Prefab Data
    public void SavePrefabData()
    {
        SaveCropPrefabData();
        //SaveScaneryPrefabData();
    }
    public void SaveCropPrefabData()
    {
        //if (!ExceptScene(GameDatas.currentScene))
        if (GameDatas.currentScene!=SceneName.Farm)
            return;

        List<CropPrefabJson> save = new List<CropPrefabJson>();
        foreach(CropPrefab crop in GameDatas.cropPrefabList)
            save.Add(new CropPrefabJson(crop.GetSeedCode(), crop.coordinate));
        SaveDataToJson(JsonDataName.CropPrefab, save, GameDatas.currentScene);
    }

    public void SaveScaneryPrefabData()
    {
        if (!ExceptScene(GameDatas.currentScene))
            return;

        List<ScaneryPrefabJson> save = new List<ScaneryPrefabJson>();
        foreach (O_ScaneryPrefab scanery in GameDatas.scaneryPrefabList)
            save.Add(scanery.GetScaneryPrefabData());
        SaveDataToJson(JsonDataName.ScaneryPrefab, save, GameDatas.currentScene);
    }

    #endregion
    #region Inventory Data
    public void SaveInventoryData()
    {
        for (int i = 0; i < (int)Inventories.cnt; i++) SaveInventoryData(i);
    }

    public void SaveInventoryData(int inventoryType)
    {
        List<JsonInventoryData> save = new List<JsonInventoryData>();
        for (int i = 0; i < GameDatas.inventories[inventoryType].Count; i++)
        {
            if(GameDatas.inventories[inventoryType][i].itemCode > 0)
                save.Add(new JsonInventoryData(GameDatas.inventories[inventoryType][i].itemCode, GameDatas.inventories[inventoryType][i].quantity, i));
        }
        SaveDataToJson(JsonDataName.InventoryData, save, (SceneName)inventoryType);
    }
    #endregion

    public void SaveAttendance(AttendanceData data)
    {
        List<AttendanceData> save = new List<AttendanceData>();
        save.Add(data);
        SaveDataToJson(JsonDataName.Attendance, save);
    }


    public void SaveSettingValue()
    {
        List<SettingValues> save = new List<SettingValues>();
        save.Add(new SettingValues(Settings.Instance.activeCursor, Settings.Instance.activeToolMenu, Settings.Instance.useJoystick, Settings.Instance.interactingAnimation, Settings.Instance.volume, Settings.Instance.padSize));
        SaveDataToJson(JsonDataName.SettingValue, save);
    }

    public void SaveMineData()
    {
        SaveDataToJson(JsonDataName.MineData, GameDatas.mineDataList);
    }

    public void SaveTreeData()
    {
        SaveDataToJson(JsonDataName.TreePrefab, GameDatas.treePrefabList, GameDatas.currentScene);
    }
    #endregion

    #region Init Data : GameData에 있는 데이터들을 Default 값으로 초기화
    public void InitPlayerData() 
    {
        GameDatas.playerData.Clear();
        GameDatas.playerData.Add(new PlayerData(SceneName.Village, CharacterDirection.down, Vector3.zero));
        SavePlayerData();
    }

    public void InitTimeData()
    {
        GameDatas.YearSeasonDay = new Vector3Int(1, 0, 1);
        GameDatas.VisitShop = Vector3Int.zero;    
        GameDatas.HourMinuteGold = new Vector3Int(9, 2, 200);
        GameDatas.loan = new Vector2Int(Settings.Instance.startLoan, Mathf.RoundToInt(Settings.Instance.startLoan * Settings.Instance.loanInterest * 0.01f));
        SaveTimeData();
    }

    /*public void ResetScaneryPrefabData()
    {
        List<JsonScaneryCoordinateData> dataList = LoadDataToJson<JsonScaneryCoordinateData>(JsonDataName.ScaneryCoordianteData);
        // 7 - 3 = 4
        List<ScaneryPrefabJson>[] scaneryPrefabs = new List<ScaneryPrefabJson>[(int)SceneName.Home - (int)SceneName.Farm];

        for (int i = 0; i < scaneryPrefabs.Length; i++)
        {
            scaneryPrefabs[i] = new List<ScaneryPrefabJson>();
            File.Delete(GetFilePath(JsonDataName.ScaneryPrefab, (SceneName)(i + 3)));
        }

        foreach (JsonScaneryCoordinateData data in dataList)
            scaneryPrefabs[data.scene - 3].Add(new ScaneryPrefabJson(data.dropItemCode, new Vector2Int(data.x, data.y), 99, 99, GameDatas.YearSeasonDay));

        for (int i = 0; i < scaneryPrefabs.Length; i++)
            SaveDataToJson(JsonDataName.ScaneryPrefab, scaneryPrefabs[i], (SceneName)(i + 3));

        return;
    }*/

    public void InitInventoryItem(Inventories type, int index)
    {
        InventoryItem item = GameDatas.inventories[(int)type][index];
        item.itemCode = 0;
        item.quantity = 0;
        item.stackable = false;
        GameDatas.inventories[(int)type][index] = item;
    }

    public void InitInventory(Inventories type)
    {
        for (int i = 0; i < GameDatas.inventories[(int)type].Count; i++)
            Instance.InitInventoryItem(type, i);
    }
    #endregion

    // 리스트를 Json으로 변환
    void TestJson()
    {
        List<Vector3Int> data = new List<Vector3Int>();
        Vector3Int[] arr = new Vector3Int[5];
        for (int i = 0; i < 5; i++)
        {
            data.Add(new Vector3Int(i, -i, i));
        }
        string json = JsonUtility.ToJson(new Data<Vector3Int>(data));
        File.WriteAllText(GetFilePath(JsonDataName.none, SceneName.System), json);
    }


    public string SaveGPGS()
    {
        GPGSData gpgs = new GPGSData();

        EventHandler.CallSavePlayerPosition();
        gpgs.time = LoadDataToString<TimeData>(JsonDataName.TimeData);
        gpgs.playerPosition = LoadDataToString<PlayerData>(JsonDataName.PlayerPositionData);
        gpgs.attendance = LoadDataToString<AttendanceData>(JsonDataName.Attendance);

        SaveInventoryData();
        gpgs.inventory = LoadDataToString<JsonInventoryData>(JsonDataName.InventoryData, SceneName.System);
        gpgs.chest = LoadDataToString<JsonInventoryData>(JsonDataName.InventoryData, SceneName.Lobby);
        gpgs.shop = LoadDataToString<JsonInventoryData>(JsonDataName.InventoryData, SceneName.Farm);

        // Save Crop
        SaveCropPrefabData();
        SaveCropTileData();
        SaveWaterTileData();
        SaveDugTileData();

        // CropPrefab
        gpgs.cropP = LoadDataToString<CropPrefabJson>(JsonDataName.CropPrefab);
        // CropTile
        gpgs.cropT = LoadDataToString<CropTileDetails>(JsonDataName.CropTile);
        // WaterTile
        gpgs.dugT = LoadDataToString<MapTileData>(JsonDataName.DugTile, SceneName.Farm);
        // DugTile
        gpgs.waterT = LoadDataToString<MapTileData>(JsonDataName.WaterTile, SceneName.Farm);

        // Mine
        SaveMineData();
        gpgs.mine = LoadDataToString<MineDatas>(JsonDataName.MineData);

        SaveTreeData();
        gpgs.tree = LoadDataToString<TreePrefabData>(JsonDataName.TreePrefab);

        List<GPGSData> data = new List<GPGSData>();
        data.Add(gpgs);
        string result = JsonUtility.ToJson(new Data<GPGSData>(data));

        return result;
    }

    public void LoadGPGS(string result)
    {
        List<GPGSData> data = JsonUtility.FromJson<Data<GPGSData>>(result).dataList;

        //List<TimeData> time = JsonUtility.FromJson<Data<TimeData>>(data[0].time).dataList;
        SaveJson(JsonDataName.TimeData, data[0].time);
        //List<PlayerData> pos = JsonUtility.FromJson<Data<PlayerData>>(data[0].playerPosition).dataList;
        SaveJson(JsonDataName.PlayerPositionData, data[0].playerPosition);
        SaveJson(JsonDataName.Attendance, data[0].attendance);

        if (!data[0].inventory.Equals(string.Empty))
            SaveJson(JsonDataName.InventoryData, data[0].inventory, SceneName.System);
        if (!data[0].chest.Equals(string.Empty))
            SaveJson(JsonDataName.InventoryData, data[0].inventory, SceneName.Lobby);
        if (!data[0].shop.Equals(string.Empty))
            SaveJson(JsonDataName.InventoryData, data[0].inventory, SceneName.Farm);
        if (!data[0].cropP.Equals(string.Empty))
            SaveJson(JsonDataName.CropPrefab, data[0].cropP, SceneName.Farm);
        if (!data[0].cropT.Equals(string.Empty))
            SaveJson(JsonDataName.CropTile, data[0].cropT, SceneName.Farm);
        if (!data[0].dugT.Equals(string.Empty))
            SaveJson(JsonDataName.DugTile, data[0].dugT, SceneName.Farm);
        if (!data[0].waterT.Equals(string.Empty))
            SaveJson(JsonDataName.WaterTile, data[0].waterT, SceneName.Farm);
        if (!data[0].mine.Equals(string.Empty))
            SaveJson(JsonDataName.MineData, data[0].mine);
        if (!data[0].tree.Equals(string.Empty))
            SaveJson(JsonDataName.TreePrefab, data[0].tree);

        StartCoroutine(Utility.DelayCall(0.3f, () => Application.Quit()));
    }
}