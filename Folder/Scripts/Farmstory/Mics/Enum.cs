public enum AnimatorParameters
{
    isWalking, isFishing, Attack, Hit, Dead, Reset, playing, length
}

public enum CharacterDirection
{
     up, right, down, left, zero
     //up, left, down, right, zero
}

public enum CharacterState
{
    // AI의 경우 action = attack
    // Player는 Action 상태에서 PlayerAction을 동작
    idle, walk, action, none
}

public enum PlayerTool
{
    Hoe, Water, Axe, PickAxe, Seed, Pickup, Attack, Fish, Interactor, none
}

public enum JsonDataName
{
    DugTile, WaterTile, CropTile, DynaicColliderTile, 
    TreePrefab, OrePrefab, CropPrefab, ScaneryPrefab, ScaneryCoordianteData,
    TimeData, PlayerPositionData, InventoryData, ShopTimeData,
    DBItem, DBCrop, DBShop, DBDIY, DBFish, DBGrade, DBAttendance, DBOreData, DBTreeData,
    Attendance, SettingValue, BankData, OreData, MineData,
    none
}

public enum DBData
{
    CheckVirsion, ServerTime, ItemData, CropData, ShopItemData, DIYData, FishData, GradeData, AttendanceReward, OreData, TreeData
        , cnt
}

public enum TilemapType
{
    none, dugGround, waterGround, staticCollider, dynamicCollider, diggable, ocean, river, pond, oreArea, enemySpawn, count
}

public enum ItemType
{
    tool, seed, crop, commodity, food, fish, fruit, none
}

public enum InventoryLocation
{
    player, chest, count
}
public enum Season
{
    Spring, Summer, Autumn, Winter, none
}

public enum Tag
{
    house, player, enemy
}

public enum SceneName
{
    System, 
    Lobby,  
    Farm, 
    Village, 
    Beach,
    MineF,
    MineB1, 
    MineB2,
    Home,
    Shop,
    Casino,
    count
}

public enum PoolPrefabName
{
    item, crop, tree, ore, slime, chicken, 
    count
}

public enum ScaneryType
{
    none, tree, ore
}

public enum AIStateType
{
    create, idle, move, follow, attack, die, none
}

public enum LayerName
{
    Player=6, Enemy, Teleport, Item, CropPrefab, InteractionPoint, EnemyDetector, LobbyCheckPoint
}

public enum Inventories
{
    bag, chest, shop, cnt
}

public enum InteractionType
{
    chest, shop, diy, game, sleep
}

public enum FishType
{
    ocean, river, pond, count
}

public enum Grade
{
    normal, rare, unique, epic, legendary, count
}

public enum SettingValue
{
    volume, joystickSize, cursor, none
}

public enum OreType
{
    Stone, Copper, Steel, Silver, Gold, Diamond, count
}

public enum TreeState
{
    sapling, stump, tree, fruit, die
}

public enum TreeDropItem
{
    none, wood, fruit
}

public enum AdType
{
    front, reward
}

public enum ProductID
{
    none,
    tmp_ci,
    tmp_nci,
    tmp_si_,
    count
}

public enum MenuIndex
{
    inventory, settings, bank, attendance, market
}

public enum MiniGameIndex
{
    close = -1, baseball, custom
}