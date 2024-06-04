public enum AnimPara
{
    walking, skilling, attack, hurt, casting, die, victory, idle
}

public enum PlayerCharacter
{
    None, Knight, Archer, Priest, Thief
}

public enum UIType
{
    None, Game, Main, Menu, Reward, Upgrade, Offline
}

public enum CharacterState
{
    none, idle, move, follow, attack, hurt, die
}

public enum MonsterCharacter 
{
    None, Goblin, Count
}

public enum LayerName
{
    PlayerCollider = 7,
    MonsterCollider = 8,
    TileCollider = 9,
    CameraArea = 10,
}

public enum UpgradeValue
{
    KnightHP, 
    KnightDamage,
    ArcherHP, 
    ArcherDamage, 
    PriestHP, 
    PriestDamage, 
    ThiefHP, 
    ThiefDamage, 
    Gold, 
    count
}