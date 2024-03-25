using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct Data<T>
{
    public List<T> dataList;
    public Data(List<T> list) => dataList = list;
}

#region DBData
[System.Serializable]
public struct JsonVirsion { public int virsion; }

[System.Serializable]
public struct JsonItemData
{
    public string itemCode, itemType, itemName, itemSprite, itemDescription, isStackable, isStartingItem, sellShop, canCropped, canEat, price;
}

[System.Serializable]
public struct JsonCropData
{
    public string seedCode, harvestCode, season, days, sprites;
}

[System.Serializable]
public struct JsonScaneryData // ScaneryDetails에 들어갈 내용을 DB로부터 가져옴
{
    public string type, dropItemCode, hp, reGrowth, destroyTool, sprites;
}

[System.Serializable]
public struct JsonScaneryCoordinateData // 맵에 생성할 Scanery의 위치와 종류 가져오기
{
    public int x, y, dropItemCode, scene;
    public JsonScaneryCoordinateData(int x, int y, int dropItemCode, int scene)
    {
        this.x = x; this.y = y; this.dropItemCode = dropItemCode; this.scene = scene;
    }
}

[System.Serializable]
public struct JsonInventoryData
{
    public int itemCode;
    public int quantity;
    public int index;
    public JsonInventoryData(int itemCode, int quantity, int index)
    {
        this.itemCode = itemCode; this.quantity = quantity; this.index = index;
    }
}

[System.Serializable]
public struct JsonShopItemData
{
    public int itemCode;
    public int rarity;
    public int minQuantity;
    public int maxQuantity;

    public JsonShopItemData(int itemCode, int rarity, int minQuantity, int maxQuantity)
    {
        this.itemCode = itemCode;
        this.rarity = rarity;
        this.minQuantity = minQuantity;
        this.maxQuantity = maxQuantity;
    }
}

[System.Serializable]
public struct JsonDIYData
{
    public int productCode;
    public string materials;
    public JsonDIYData(int productCode, string materials)
    {
        this.productCode = productCode;
        this.materials = materials;
    }
}

[System.Serializable]
public struct JsonFishData
{
    public int itemCode;
    public int grade;
    public int type;
    public JsonFishData(int itemCode, int grade, int type)
    {
        this.itemCode = itemCode;
        this.grade = grade;
        this.type = type;
    }
}

[System.Serializable]
public struct JsonGradeData
{
    public int grade;
    public int percentage;
    public int count;
    public float time;
    public float baseDist;
    public JsonGradeData(int grade, int percentage, int count, float time, float baseDist)
    {
        this.grade = grade;
        this.percentage = percentage;
        this.count = count;
        this.time = time;
        this.baseDist = baseDist;
    }
}

[System.Serializable]
public struct JsonAttendanceRewardData
{
    public int amount;
    public int code;
    public JsonAttendanceRewardData(int amount, int code)
    {
        this.amount = amount;
        this.code = code;
    }
}

[System.Serializable]
public struct JsonOreData
{
    public int oreType;
    public int dropItemCode;
    public int grade;
    public int hp1;
    public int hp2;
    public string sprites;
    public JsonOreData(int oreType, int dropItemCode, int grade, int hp1, int hp2, string sprites)
    {
        this.oreType = oreType;
        this.dropItemCode = dropItemCode;
        this.grade = grade;
        this.hp1 = hp1;
        this.hp2 = hp2;
        this.sprites = sprites;
    }
}

#endregion

#region Details
[System.Serializable]
public class CropDetails
{
    public int seedCode { get; private set; }
    public int harvestCode { get; private set; }
    public int totalGrowthDay { get; private set; }
    public Season season { get; private set; }
    public int[] growthDay { get; private set; }
    public Sprite[] growthSprite { get; private set; }

    public CropDetails(int seed, int harvest, int total, Season season, int[] growth, Sprite[] sprites)
    {
        seedCode = seed; harvestCode = harvest; totalGrowthDay = total;
        this.season = season; growthDay = growth; growthSprite = sprites;
    }
}

[System.Serializable]
public class ItemDetails
{
    public int code { get; private set; }
    public int price { get; private set; }

    public bool isStackable { get; private set; }
    public bool isStartingItem { get; private set; }
    public bool canPickup { get; private set; }
    public bool canCrop { get; private set; }
    public bool canEat { get; private set; }

    public string name { get; private set; }
    public string description { get; private set; }

    public ItemType type { get; private set; }
    public Sprite sprite { get; private set; }

    public ItemDetails(int _code, int _price,
        bool _isStackable, bool _isStartingItem, bool _canPickup, bool _canCrop, bool _canEat,
        string _name, string _description, ItemType _type, Sprite _sprite)
    {
        code = _code; price = _price;
        isStackable = _isStackable; isStartingItem = _isStartingItem; canPickup = _canPickup; canCrop = _canCrop; canEat = _canEat;
        name = _name; description = _description; type = _type; sprite = _sprite;
    }
}

[System.Serializable]
public class ScaneryDetails
{
    public int dropItemCode;
    public int hp;
    public int reGrowth;
    public PlayerTool destroyTool;
    public Sprite destroyableSprite;
    public Sprite undestoryableSprite;

    public ScaneryDetails(int code, int hp, int reGrowth, PlayerTool tool, Sprite destroyable, Sprite undestroyable)
    {
        dropItemCode = code;
        this.hp = hp;
        this.reGrowth = reGrowth;
        destroyTool = tool;
        destroyableSprite = destroyable;
        undestoryableSprite = undestroyable;
    }
}

[System.Serializable]
public class CropTileDetails
{
    public int seedCode;
    public int growthDay;
    public bool todayWater;
    public Vector2Int coordinate;
    public Vector2Int remain;// { get; private set; }
    public Vector3Int lastDay;
    int gap => Utility.DayGap(lastDay);

    public CropTileDetails(Vector2Int coordinate)
    {
        this.coordinate = coordinate;
        remain = new Vector2Int(2, 0);
        lastDay = GameDatas.YearSeasonDay;
        seedCode = -1;
        growthDay = -1;
        todayWater = false;
    }

    public CropTileDetails( CropTileDetails tile)
    {
        Debug.Log("Crop Tile Details's 생성자");
        coordinate = tile.coordinate;
        remain = tile.remain;
        todayWater = tile.todayWater;
        growthDay = tile.growthDay;
        lastDay = tile.lastDay;
    }

    public void SetLastDay() { lastDay = GameDatas.YearSeasonDay; }

    public Vector2Int CheckRemain()
    {
        todayWater = false;
        remain = new Vector2Int(remain.x - gap < 0 ? -1 : remain.x - gap, remain.y - gap < 0 ? -1 : remain.y - gap);
        growthDay = seedCode > 0 ? growthDay + gap : -1;
        return remain;
    }

    public bool WaterGround(int count)
    {
        if (todayWater) { Debug.Log("이미 물을 줌"); return true; }
        remain = new Vector2Int(remain.x + count > 6 ? 6 : remain.x + count,
            remain.y < 0 ? count : (remain.y + count > 3 ? 3 : remain.y + count));
        todayWater = true;
        return false;
    }

    public void Wither()
    {
        seedCode = -1;
        growthDay = -1;
    }
}

#endregion

#region System Data
[System.Serializable]
public struct PlayerData
{
    public SceneName scene;
    public CharacterDirection direction;
    public Vector3 position;
    public PlayerData(SceneName scene, CharacterDirection direction, Vector3 position)
    {
        this.scene = scene;
        this.direction = direction;
        this.position = position;
    }
}

[System.Serializable]
public struct TimeData
{
    public Vector3Int YearSeasonDay;
    public Vector3Int HourMinuteGold;
    public Vector3Int VisitShop;
    public Vector2Int Loan;

    public TimeData(Vector3Int YearSeasonDay, Vector3Int HourMinuteGold, Vector3Int VisitShop, Vector2Int loan)
    {
        this.YearSeasonDay = YearSeasonDay;
        this.HourMinuteGold = HourMinuteGold;
        this.VisitShop = VisitShop;
        this.Loan = loan;
    }
}

[System.Serializable]
public struct BankData
{
    // x : 대출금 / y : 1년동안 상환해야할 이자
    public Vector2Int loanData;
    public BankData(Vector2Int data) { loanData = data; }
}

[System.Serializable]
public struct Pool { public GameObject prefab; public PoolPrefabName index; }

/*[System.Serializable]
public struct ShopTime { public Vector3Int YearSeasonDay; public ShopTime(Vector3Int YearSeasonDay) { this.YearSeasonDay = YearSeasonDay; } }*/
#endregion

#region Prefab Data

[System.Serializable]
public struct CropPrefabJson
{
    public int seedCode;
    public Vector2Int coordinate;
    public CropPrefabJson(int code, Vector2Int coordinate)
    {
        seedCode = code;
        this.coordinate = coordinate;
    }
}

[System.Serializable]
public struct ScaneryPrefabJson
{
    public int dropItemCode;
    public int hp;
    public int count;
    public Vector2Int coordinate;
    public Vector3Int lastDay;
    public ScaneryPrefabJson(int code, Vector2Int coordinate, int count, int hp, Vector3Int lastDay)
    {
        dropItemCode = code;
        this.coordinate = coordinate;
        this.count = count;
        this.lastDay = lastDay;
        this.hp = hp;
    }
}
#endregion

#region Tile Data
[System.Serializable]
public struct MapTileData
{
    public Vector2Int coordinate;
    public TilemapType type;
    public MapTileData(Vector2Int coordinate, TilemapType type)
    {
        this.coordinate = coordinate;
        this.type = type;
    }
}

[System.Serializable]
public struct TilemapTypePair
{
    public Tilemap map;
    public TilemapType type;
}


[System.Serializable]
public class AStarNode
{
    public int G, H;
    public Vector2Int coordinate;
    public AStarNode parent;
    public AStarNode(Vector2Int coordinate, int G, int H, AStarNode parent)
    { this.coordinate = coordinate; this.G = G; this.H = H; this.parent = parent; }
    public AStarNode(Vector3Int coordinate)
    {
        this.coordinate = new Vector2Int(coordinate.x + 1, coordinate.y + 1);
        G = 0; H = 0;
    }
    public void InitNode(Vector3Int coordinate)
    {
        this.coordinate = new Vector2Int(coordinate.x + 1, coordinate.y + 1);
        G = 0; H = 0;
    }
    public int F { get => G + H; }
}
#endregion

#region Inventory Data
/*[System.Serializable]
public struct InventoryType
{
    public InventoryItem[] inventory;
    public List<InventoryItem>[] invenList;
    public InventoryItem[][] testInven;
}*/

[System.Serializable]
public struct InventoryItem
{
    public int itemCode;
    public int quantity;
    public bool stackable;
}

[System.Serializable]
public struct InventoryCapacity
{
    public Inventories type;
    public int capacity;
}

[System.Serializable]
public struct ShopItem
{
    public int itemCode;
    public int rarity;
    public int min;
    public int max;

    public ShopItem(int itemCode, int rarity, int min, int max)
    {
        this.itemCode = itemCode;
        this.rarity = rarity;
        this.min = min;
        this.max = max;
    }
}
#endregion

#region DIY
[System.Serializable]
public struct DIYRecipe
{
    public int productCode;
    public DIYMaterial[] materials;

    public DIYRecipe(int productCode, DIYMaterial[] materials)
    {
        this.productCode = productCode;
        this.materials = materials;
    }
}

[System.Serializable]
public struct DIYMaterial
{
    public int materialCode;
    public int materialCount;
    public DIYMaterial(int materialCode, int materialCount)
    {
        this.materialCode = materialCode;
        this.materialCount = materialCount;
    }
}
#endregion

[System.Serializable]
public struct FishGradeInfo
{
    public int count;
    public float time;
    public float baseDist;

    public FishGradeInfo(int count, float time, float baseDist)
    {
        this.count = count; this.time = time; this.baseDist = baseDist;
    }
}

[System.Serializable]
public struct InteractorPanel
{
    public Panel panel;
    public InteractionType type;
}

[System.Serializable]
public struct AttendanceData
{
    public int lastVisit;
    public int visitCount;
    public int receiveCount;
    //public string visitedDay;

    public AttendanceData(int day, int vCnt, int rCnt/*, string dayArr*/)
    {
        lastVisit = day;
        visitCount = vCnt;
        receiveCount = rCnt;
        //visitedDay = dayArr;
    }
}

[System.Serializable]
public struct MenuBinding
{
    public Panel panel;
    public MenuButton button;
}

[System.Serializable]
public struct SettingValues
{
    public bool activeCursor;
    public bool activeToolMenu;
    public bool useJoystick;
    public bool interactingAnimation;
    public int volume;
    public float stickSize;

    public SettingValues(bool activeCursor, bool activeToolMenu, bool useJoystick, bool interactingAnimation, int volume, float stickSize)
    {
        this.activeCursor = activeCursor;
        this.activeToolMenu = activeToolMenu;
        this.useJoystick = useJoystick;
        this.interactingAnimation = interactingAnimation;
        this.stickSize = stickSize;
        this.volume = volume;
    }
}

[System.Serializable]
public struct EnemySpawnData
{
    public SceneName spawnScene;
    public int startEnemy;
    public int maxEnemy;
    public float spawnTime;
    public PoolPrefabName[] enemies;
}

[System.Serializable]
public struct SceneConfiner
{
    public SceneName scene;
    public Vector2[] points;
}


[System.Serializable]
public struct OreData
{
    public OreType type;
    public int dropItem;
    public int grade;
    public Vector2Int hp;
    public Sprite[] sprites;

    public OreData(OreType type, int dropItem, int grade, Vector2Int hp, Sprite[] sprites)
    {
        this.type = type;
        this.dropItem = dropItem;
        this.grade = grade;
        this.hp = hp;
        this.sprites = sprites;
    }

    //public OreData CopyData(OreData origin) => new OreData(origin.type, origin.dropItem, origin.grade, origin.sprites);
}

[System.Serializable]
public struct SpawnOreData
{
    [System.Serializable]
    public struct SpawnOre
    {
        public OreType type;
        public Grade grade;
    }

    public SceneName scene;
    public int oreNum;
    public SpawnOre[] oreList;
}

[System.Serializable]
public struct MineDatas
{
    public int sceneNum;
    public Vector3Int lastVisit;
    public string orePos; // ore의 위치를 List<Vector3Int>로 저장(xCoordinate, yCoordinate, oreType) >> string을 List<Vector3Int>로 변환해 사용

    public MineDatas(int sceneNum, Vector3Int lastVisit, string orePos)
    {
        this.sceneNum = sceneNum;
        this.lastVisit = lastVisit;
        this.orePos = orePos;
    }
}

[System.Serializable]
public struct OrePrefabData
{
    public int oreType;
    public int hp;
    public Vector2Int coordinate;
    public bool firstDestroy;
    public OrePrefabData(int oreType, int hp, Vector2Int coordinate, bool firstDestroy)
    {
        this.oreType = oreType;
        this.hp = hp;
        this.coordinate = coordinate; 
        this.firstDestroy = firstDestroy;
    }

    public OrePrefabData(OrePrefabData var)
    {
        oreType = var.oreType;
        hp = var.hp;
        coordinate = var.coordinate;
        firstDestroy = var.firstDestroy;
    }

    public void FirstDestroy()
    {
        Debug.Log("Fist Destroy");
        firstDestroy = true;
    }

    public void OnHit() { --hp; }

}

[System.Serializable]
public struct TreeData
{
    public int fruitCode;
    public int firstGrowth;
    public int secondGrowth;
    public int stumpHP;
    public int treeHP;
    public int fruitSeason;
    public string spriteName;
    public TreeData(int fruitCode, int firstGrowth, int secondGrowth, int stumpHP, int treeHP, int fruitSeason, string spriteName)
    {
        this.fruitCode = fruitCode;
        this.firstGrowth = firstGrowth;
        this.secondGrowth = secondGrowth;
        this.stumpHP = stumpHP;
        this.treeHP = treeHP;
        this.spriteName = spriteName;
        this.fruitSeason = fruitSeason;
    }
}

/*[System.Serializable]
public struct TreeData
{
    public int fruitCode;
    public Vector2Int growth;
    public Vector4 hp;
    public string spriteName;
    public TreeData(JsonTreeData data)
    {
        fruitCode = data.fruitCode;

    }
}*/

[System.Serializable]
public struct TreePrefabData
{
    public int fruitCode;
    public int dayCount;
    public int state;
    public int stateHP;
    public Vector2Int coordinate;
    public Vector3Int lastVisit;
    public TreePrefabData(int fruitCode, int dayCount, int state, int stateHP, Vector2Int coordinate, Vector3Int lastVisit)
    {
        this.fruitCode = fruitCode;
        this.dayCount = dayCount;
        this.state = state;
        this.stateHP = stateHP;
        this.coordinate = coordinate;
        this.lastVisit = lastVisit;
    }
}

[System.Serializable]
public struct GPGSData
{
    public string time;             // TimeData
    public string playerPosition;   // PlayerPositionData
    public string inventory;        // InventoryData_Farm
    public string chest;            // InventoryData_Lobby
    public string shop;
    public string cropP;             // CropPrefab
    public string cropT;
    public string dugT;
    public string waterT;
    public string tree;             // ScaneryPrefab
    public string mine;
    public string attendance;
}

public interface IHit
{
    public void OnHit(int dmg = 1);
}

/*public struct GradePercentage
{
    public int grade;
    public int percentage;
}*/


/*
[System.Serializable]
public struct MapTileData
{
    public Vector2Int coordinate;
    public TilemapType type;

    public MapTileData(Vector2Int pos, TilemapType type)
    {
        coordinate = pos;
        this.type = type;
    }
}

[System.Serializable]
public struct CropPrefabData
{
    public int seedCode;
    public Vector2Int coordinate;
    public CropPrefabData(int code, Vector2Int coordinate)
    {
        seedCode = code;
        this.coordinate = coordinate;
    }
}

[System.Serializable]
public struct ScaneryPrefabData
{
    public int dropItemCode;
    public int dayCounter;
    public int hp;
    public Vector2Int coordinate;
    public ScaneryPrefabData(int code, Vector2Int coordinate, int count, int hp)
    {
        dropItemCode = code;
        this.coordinate = coordinate;
        dayCounter = count;
        this.hp = hp;
    }
}




*/



