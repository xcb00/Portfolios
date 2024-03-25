using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDatas// : Singleton<GameDatas>
{
    /*protected override void Awake()
    {
        base.Awake();
    }*/

    #region System Data
    public static SceneName currentScene;
    public static bool LobbyScene;
    public static List<PlayerData> playerData;
    public static Vector3Int YearSeasonDay;
    public static Vector3Int HourMinuteGold;
    public static Vector3Int VisitShop;
    public static Vector3Int minMap;
    public static Vector3Int maxMap;
    public static bool pause;
    #endregion

    #region Sprites
    public static Sprite[] crops;
    public static Sprite[] cropIcon;
    public static Sprite[] seeds;
    public static Sprite[] mining;
    public static Sprite[] trees;
    public static Sprite[] items;
    public static Sprite[] fish;
    public static Sprite[] coin;
    public static Sprite[] tools16;
    #endregion

    #region DetailsList
    public static List<ItemDetails> itemDetailsList;
    public static List<CropDetails> cropDetailsList;
    public static List<ScaneryDetails> scaneryDetailsList;
    public static List<DIYRecipe> diyRecipeList;
    public static List<MineDatas> mineDataList;
    public static List<ShopItem> shopItemList;
    public static List<TreePrefabData> treePrefabList;

    public static Dictionary<int, OreData> oreDataDic;
    public static Dictionary<int, TreeData> treeDataDic;
    #endregion

    #region Json으로 저장할 데이터
    // Tile
    public static List<MapTileData>[] mapTileList;
    public static List<CropTileDetails> cropTileList;

    // 인벤토리
    public static List<InventoryItem>[] inventories;

    // DB로 받은 ScaneryCoordinate 정보를 저장
    //public static List<JsonScaneryCoordinateData>[] scanryCoorinateList;
    #endregion

    #region Prefab Data
    public static List<CropPrefab> cropPrefabList;          // 저장할 때 CropPrefabJson으로 변환 후 저장
    public static List<Vector2Int> removeCropCoordinateList;


    public static List<O_ScaneryPrefab> scaneryPrefabList;    // 저장할 때 ScaneryPrefabJson으로 변환한 후 저장
    #endregion

    #region Item Data
    public static List<int> seedCodes;
    #endregion

    #region Fish Data
    public static List<int>[,] fishLists;
    public static FishGradeInfo[] fishGrades;
    #endregion


    #region Grade Data
    public static int[] gradePercentage;
    public static int[] gradeStackPercentage;
    #endregion

    public static int nowDay;

    public static Vector2Int[] attendanceReward;

    // Player
    public static bool playerDie;

    // Bank
    public static Vector2Int loan;

    public static bool yearChange = false;

    public static bool gpgsLogin = false;
}
