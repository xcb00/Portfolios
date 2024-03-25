using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public struct Test
{
    public int[] arr;
    public Test(int[] arr) { this.arr = arr; }
}
public class NetworkManager : MonoBehaviour
{
    const string ServerURL = "https://www.naver.com/";
    const string URL = "https://script.google.com";
    int connectTime = 60;
    // PlayerPrefab Key
    const string virsionKey = "Virsion";
    const string lastVisitKey = "lastVisit";

    string scaneryCooridante = string.Empty;
    public string ScaneryCoordinate => scaneryCooridante;

    bool[] loadData;

    [SerializeField] LoadingUI loadingUI = null;

    private void Start()
    {
        StartCoroutine(LoadingDBData());
    }

    IEnumerator LoadingDBData()
    {
        GameDatas.currentScene = SceneName.Lobby;
        GameDatas.LobbyScene = true;

        loadingUI.StartLoading();

        // Start 버튼을 누르기 전까지 게임 내 시간을 멈춤
        GameDatas.pause = true;

        Settings.Instance.nigthFadeTime = Settings.Instance.secondsPerMinute * 2f;

        // Resources 폴더에 있는 Sprite들을 게임에서 쉽게 사용할 수 있도록 GameDatas에 저장
        LoadSprites();

        float percent = Mathf.Floor((1f / ((int)DBData.cnt + 1)) * 100f) / 100f;



        // DB에서 데이터를 순차적으로 저장
        for (int i = 0; i < (int)DBData.cnt; i++)
        {
            loadingUI.MaxLoadingValue(percent * (i + 1));

            if (i == 1)
                yield return StartCoroutine(LoadingNetworkTime());
            else if (i == 0)
                yield return StartCoroutine(LoadingJsonData(DBData.CheckVirsion, true));
            else
                yield return StartCoroutine(LoadingJsonData((DBData)i, loadData[i - 2]));
        }

        float t = 0.2f;
        loadingUI.EndLoading(t);
        while (t > -0.1f) { t -= Time.deltaTime; yield return null; }

        EventHandler.CallDBDataLoadEvent();
        SceneControlManager.Instance.LoadLobby();
    }

    void LoadSprites()
    {
        GameDatas.crops = Resources.LoadAll<Sprite>($"Sprite/crops");
        GameDatas.cropIcon = Resources.LoadAll<Sprite>($"Sprite/cropIcon");
        GameDatas.seeds = Resources.LoadAll<Sprite>($"Sprite/seeds");
        GameDatas.mining = Resources.LoadAll<Sprite>($"Sprite/mining");
        GameDatas.trees = Resources.LoadAll<Sprite>($"Sprite/trees");
        GameDatas.items = Resources.LoadAll<Sprite>($"Sprite/items");
        GameDatas.fish = Resources.LoadAll<Sprite>($"Sprite/fish");
        GameDatas.coin = Resources.LoadAll<Sprite>($"Sprite/coin");
        GameDatas.tools16 = Resources.LoadAll<Sprite>($"Sprite/tools16");
    }

    IEnumerator LoadingJsonData(DBData name, bool loadData)
    {
        string jsonData = string.Empty;
        bool getData = false; // 로컬 저장소에 저장된 DBData가 존재하는지 확인 >> 없다면 서버에서 다시 가져와 저장함

        if (!loadData)
            jsonData = DataManager.Instance.DBJson((JsonDataName)((int)name + 11), ref getData);
        else
            getData = loadData;

        if(getData)
        {
            WWWForm form = new WWWForm();

            form.AddField("dataType", EnumCaching.ToString(name));
            using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
            {
                www.timeout = connectTime;

                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("Network Error");
                    EventHandler.CallNetworkErrorEvent();
                }
                else
                {
                    jsonData = www.downloadHandler.text;
                }
                www.Dispose();
            }
        }
        switch (name)
        {
            case DBData.CheckVirsion: ParseVirsion(jsonData); break;
            case DBData.ItemData: ParseItemList(jsonData); break;
            case DBData.CropData: ParseCropList(jsonData); break;
            case DBData.ShopItemData: ParseShopItemList(jsonData); break;
            case DBData.DIYData: ParseDIYList(jsonData); break;
            case DBData.FishData: ParseFishList(jsonData); break;
            case DBData.GradeData: ParseGradeData(jsonData); break;
            case DBData.AttendanceReward: ParseAttendanceReward(jsonData); break;
            case DBData.OreData: ParseOreData(jsonData); break;
            case DBData.TreeData: ParseTreeData(jsonData); break;
        }
    }
    #region DB Data 파싱
    IEnumerator LoadingNetworkTime()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(ServerURL))
        {
            www.timeout = connectTime;

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Network Error");
                EventHandler.CallNetworkErrorEvent();
            }
            else
            {
                DateTime dateTime = DateTime.Parse(www.GetResponseHeader("date")).ToUniversalTime();
                GameDatas.nowDay = int.Parse(dateTime.ToString("dd"));
                /*int last = 0;

                if (PlayerPrefs.HasKey(lastVisitKey))
                    last = PlayerPrefs.GetInt(lastVisitKey);
                else
                {
                    Debug.Log("Create LastVisitKey");
                    last = 0;
                    PlayerPrefs.SetInt(lastVisitKey, last);
                }

                if (last == day) Debug.Log($"Already Visit - Day{day} / Last{last}");
                else Debug.Log($"First Visit - Day{day} / Last{last}");*/
            }
            www.Dispose();
        }
    }

    void ParseVirsion(string json)
    {
        List<JsonVirsion> data = JsonUtility.FromJson<Data<JsonVirsion>>(json).dataList;
        string[] saveVirsion;
        loadData = new bool[data.Count];

        for (int i = 0; i < loadData.Length; i++) loadData[i] = false;

        if (string.IsNullOrEmpty(PlayerPrefs.GetString(virsionKey)))
        {
            saveVirsion = new string[data.Count];
            for (int i = 0; i < saveVirsion.Length; i++) saveVirsion[i] = "-1";
        }
        else
            saveVirsion = PlayerPrefs.GetString(virsionKey).Split("_");

        if (saveVirsion.Length != data.Count)
        {
            saveVirsion = new string[data.Count];
            for (int i = 0; i < saveVirsion.Length; i++)
                saveVirsion[i] = "-1";
        }

        string _virsion = string.Empty;

        for (int i = 0; i < loadData.Length; i++)
        {
            _virsion += (i == 0 ? data[i].virsion.ToString() : $"_{data[i].virsion.ToString()}");
            loadData[i] = int.Parse(saveVirsion[i]) != data[i].virsion;
        }

        PlayerPrefs.SetString(virsionKey, _virsion);
    }

    void ParseItemList(string json)
    {
        DataManager.Instance.SaveJson(JsonDataName.DBItem, json);

        List<JsonItemData> itemDatas = JsonUtility.FromJson<Data<JsonItemData>>(json).dataList;
        GameDatas.itemDetailsList = new List<ItemDetails>();
        GameDatas.seedCodes = new List<int>();
        for (int i = 0; i < itemDatas.Count; i++)
        {
            ItemType type = ItemType.none;
            switch (itemDatas[i].itemType)
            {
                case "tool": type = ItemType.tool; break;
                case "seed": type = ItemType.seed; GameDatas.seedCodes.Add(int.Parse(itemDatas[i].itemCode)); break;
                case "crop": type = ItemType.crop; break;
                case "commodity": type = ItemType.commodity; break;
                case "food": type = ItemType.food; break;
                case "fish": type = ItemType.fish; break;
                case "fruit": type = ItemType.fruit; break;
                case "none":
                default: type = ItemType.none; break;
            }

            ItemDetails item = new ItemDetails(
                int.Parse(itemDatas[i].itemCode),
                int.Parse(itemDatas[i].price),
                StringToBool(itemDatas[i].isStackable),
                StringToBool(itemDatas[i].isStartingItem),
                StringToBool(itemDatas[i].sellShop),
                StringToBool(itemDatas[i].canCropped),
                StringToBool(itemDatas[i].canEat),
                itemDatas[i].itemName,
                itemDatas[i].itemDescription,
                type,
                Utility.GetSprite(itemDatas[i].itemSprite)
                );

            GameDatas.itemDetailsList.Add(item);
        }
    }

    void ParseCropList(string json)
    {
        DataManager.Instance.SaveJson(JsonDataName.DBCrop, json);

        List<JsonCropData> cropDatas = JsonUtility.FromJson<Data<JsonCropData>>(json).dataList;
        GameDatas.cropDetailsList = new List<CropDetails>();
        for (int i = 0; i < cropDatas.Count; i++)
        {
            // 배열 파싱
            string[] _days = cropDatas[i].days.Split('+');
            string[] _sprites = cropDatas[i].sprites.Split('+');
            int[] growthDay = new int[_days.Length];
            Sprite[] growthSprite = new Sprite[_sprites.Length];
            int total = 0;
            for (int j = 0; j < _days.Length; j++)
            {
                growthDay[j] = int.Parse(_days[j]);
                total += growthDay[j];
                growthSprite[j] = Utility.GetSprite(_sprites[j]);
            }

            Season season = Season.none;
            switch (cropDatas[i].season.ToLower())
            {
                case "spring": season = Season.Spring; break;
                case "summer": season = Season.Summer; break;
                case "autumn": season = Season.Autumn; break;
                case "winter": season = Season.Winter; break;
                default: break;
            }

            CropDetails crop = new CropDetails(int.Parse(cropDatas[i].seedCode), int.Parse(cropDatas[i].harvestCode), total, season, growthDay, growthSprite);
            GameDatas.cropDetailsList.Add(crop);
        }
    }


    void ParseShopItemList(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        DataManager.Instance.SaveJson(JsonDataName.DBShop, json);

        List<JsonShopItemData> shopData = JsonUtility.FromJson<Data<JsonShopItemData>>(json).dataList;
        GameDatas.shopItemList = new List<ShopItem>();
        int max = Settings.Instance.maxQuantity;
        for (int i = 0; i < shopData.Count; i++)
        {
            int _min = shopData[i].minQuantity < 1 ? 1 : (shopData[i].minQuantity >= max ? max : shopData[i].minQuantity);
            int _max = shopData[i].maxQuantity >= max ? max : (shopData[i].maxQuantity < 1 ? 1 : shopData[i].maxQuantity);
            GameDatas.shopItemList.Add(new ShopItem(shopData[i].itemCode, shopData[i].rarity, _min, _max));
        }
    }

    void ParseDIYList(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        DataManager.Instance.SaveJson(JsonDataName.DBDIY, json);

        GameDatas.diyRecipeList = new List<DIYRecipe>();
        List<JsonDIYData> diyData = JsonUtility.FromJson<Data<JsonDIYData>>(json).dataList;
        for(int i = 0; i < diyData.Count; i++)
        {
            string[] materialStr = diyData[i].materials.Split("+");
            DIYMaterial[] materials = new DIYMaterial[Mathf.RoundToInt(materialStr.Length * 0.5f)];

            for(int j = 0; j < materials.Length; j++)
                materials[j] = new DIYMaterial(int.Parse(materialStr[j * 2]), int.Parse(materialStr[j * 2 + 1]));

            GameDatas.diyRecipeList.Add(new DIYRecipe(diyData[i].productCode, materials));
        }
    }

    void ParseFishList(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        DataManager.Instance.SaveJson(JsonDataName.DBFish, json);

        GameDatas.fishLists = new List<int>[(int)FishType.count, (int)Grade.count];

        for(int i = 0; i < (int)FishType.count; i++)
        {
            for (int j = 0; j < (int)Grade.count; j++)
                GameDatas.fishLists[i, j] = new List<int>();
        }

        List<JsonFishData> fishData = JsonUtility.FromJson<Data<JsonFishData>>(json).dataList;

        foreach(JsonFishData data in fishData)
            GameDatas.fishLists[data.type, data.grade].Add(data.itemCode);
    }

    void ParseGradeData(string json)
    {
        if (string.IsNullOrEmpty(json)) return;
        DataManager.Instance.SaveJson(JsonDataName.DBGrade, json);
        int cnt = (int)Grade.count;
        GameDatas.gradePercentage = new int[cnt];
        GameDatas.gradeStackPercentage = new int[cnt];
        GameDatas.fishGrades = new FishGradeInfo[cnt];

        List<JsonGradeData> gradeData = JsonUtility.FromJson<Data<JsonGradeData>>(json).dataList;

        foreach (JsonGradeData data in gradeData)
        {
            GameDatas.gradePercentage[data.grade] = data.percentage;
            GameDatas.fishGrades[data.grade] = new FishGradeInfo(data.count, data.time, data.baseDist);
        }

        for(int i = cnt - 1; i >= 0; i--)
            for (int j = 0; j <= i; j++)
                GameDatas.gradeStackPercentage[cnt - 1 - i] += GameDatas.gradePercentage[cnt - j - 1];

    }

    void ParseAttendanceReward(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        DataManager.Instance.SaveJson(JsonDataName.DBAttendance, json);

        List<JsonAttendanceRewardData> data = JsonUtility.FromJson<Data<JsonAttendanceRewardData>>(json).dataList;
        GameDatas.attendanceReward = new Vector2Int[data.Count];
        for(int i = 0; i < data.Count; i++)
            GameDatas.attendanceReward[i] = new Vector2Int(data[i].code, data[i].amount);
        
    }


    void ParseOreData(string json)
    {
        if (string.IsNullOrEmpty(json)) return;
        //Debug.Log("Create Ore Dic");
        DataManager.Instance.SaveJson(JsonDataName.DBOreData, json);

        List<JsonOreData> data = JsonUtility.FromJson<Data<JsonOreData>>(json).dataList;
        GameDatas.oreDataDic = new Dictionary<int, OreData>();

        foreach(JsonOreData d in data)
        {
            string[] _sprites = d.sprites.Split("+");
            Sprite[] _spriteArr = new Sprite[_sprites.Length];
            for (int i = 0; i < _sprites.Length; i++)
            {
                _spriteArr[i] = Utility.GetSprite(_sprites[i]);
            }
            GameDatas.oreDataDic.Add(d.oreType, new OreData((OreType)d.oreType, d.dropItemCode, d.grade, new Vector2Int(d.hp1, d.hp2), _spriteArr));
        }
    }

    

    void ParseTreeData(string json)
    {
        if (string.IsNullOrEmpty(json)) return;
        DataManager.Instance.SaveJson(JsonDataName.DBTreeData, json);

        List<TreeData> data = JsonUtility.FromJson<Data<TreeData>>(json).dataList;
        GameDatas.treeDataDic = new Dictionary<int, TreeData>();

        foreach(TreeData d in data)
            GameDatas.treeDataDic.Add(d.fruitCode, d);


        /*for (int i = 0; i < data.Count; i++)
        {
            int idx = data[i].fruitCode;
        }

        int[] iArr = new int[4] { 1, 2, 3, 4 };
        List<Test> test = new List<Test>();
        test.Add(new Test(iArr));
        foreach (int i in test[0].arr) Debug.Log(i);

        string str = JsonUtility.ToJson(new Data<Test>(test));
        Debug.Log(str);

        List<Test> from = JsonUtility.FromJson<Data<Test>>(str).dataList;
        foreach (int i in from[0].arr) Debug.Log(i);*/
    }

    /*Sprite GetSprite(string name_idx)
    {
        string[] str = name_idx.Split('_');
        switch (str[0])
        {
            case "crops": return GameDatas.crops[int.Parse(str[1])];
            case "cropIcon": return GameDatas.cropIcon[int.Parse(str[1])];
            case "seeds": return GameDatas.seeds[int.Parse(str[1])];
            case "mining"://mining
                return GameDatas.mining[int.Parse(str[1])];
            case "trees": return GameDatas.trees[int.Parse(str[1])];
            case "items": return GameDatas.items[int.Parse(str[1])];
            case "fish": return GameDatas.fish[int.Parse(str[1])];
            case "coin": return GameDatas.coin[int.Parse(str[1])];
            default: return null;
        }
    }*/

    bool StringToBool(string str)
    {
        str.ToLower();
        if (str.CompareTo("true") == 0) return true;
        else return false;
    }
    #endregion
}



/*void ParseScaneryList(string json)
{
    List<JsonScaneryData> scaneryData = JsonUtility.FromJson<Data<JsonScaneryData>>(json).dataList;
    GameDatas.scaneryDetailsList = new List<ScaneryDetails>();
    for (int i = 0; i < scaneryData.Count; i++)
    {
        PlayerTool tool = PlayerTool.none;
        if (scaneryData[i].destroyTool == "Axe") tool = PlayerTool.Axe;
        else tool = PlayerTool.PickAxe;
        string[] _sprites = scaneryData[i].sprites.Split('+');
        ScaneryDetails scanery = new ScaneryDetails(
            int.Parse(scaneryData[i].dropItemCode),
            int.Parse(scaneryData[i].hp),
            int.Parse(scaneryData[i].reGrowth),
            tool,
            GetSprite(_sprites[0]),
            GetSprite(_sprites[1]));
        GameDatas.scaneryDetailsList.Add(scanery);
    }
}

void ParseScaneryCoordinateList(string json)
{
    if (string.IsNullOrEmpty(json)) return;

    List<JsonScaneryCoordinateData> dataList = new List<JsonScaneryCoordinateData>();
    dataList = JsonUtility.FromJson<Data<JsonScaneryCoordinateData>>(json).dataList;
    DataManager.Instance.SaveDataToJson(JsonDataName.ScaneryCoordianteData, dataList);
}*/