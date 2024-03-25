public enum UnitStateType
{
    none, idle, move, attack, hit, knockback, die, count
}

public enum PlayerUnitType
{
    farmer, miner, warrior, magician, count
}

public enum ComUnitType
{
    Plant, Mushroom, Dragon, Bat, Bunny, Golem, Wolf, Bee, Fairy, Guard, Goblin, count
}

public enum AnimationParameters
{
    action1, action2, action3, death, reset, walking, playing, running, knockback
}

public enum SceneName
{
    Lobby, Main
}

public enum PoolType
{
    ComPlant, ComMushroom, ComDragon, ComBat, ComBunny, ComGolem, ComWolf, ComBee, ComFairy, ComGuard, ComGoblin,
    PlayerFarmer, PlayerMiner, PlayerWarrior, PlayerMagician, PlayerFireball, Soil, MinimapIcon, cnt
}

public enum LayerName
{
    Ground=6, Tree, Building, Player, PlayerUnit, ComUnit, MonsterHitCollider, EnquequeUnit, Banner,cnt
}

public enum BuildType
{
    soil, count
}

public enum ItemName
{
    Fish, Knife, Pan, Rapira
}

public enum SceneChange
{
    StageClear, GameClear, GameOver
}