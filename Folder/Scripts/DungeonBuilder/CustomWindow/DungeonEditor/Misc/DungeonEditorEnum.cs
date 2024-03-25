#region Dungeon Map
public enum RoomType
{
    None = 0,
    SmallRoom,  // Index : 0
    MediumRoom, // Index : 1
    LargeRoom,  // Index : 2
    ChestRoom,  // Index : 3
    Visible,
    Entrance,   // Index : 4
    BossRoom    // Index : 5
}

public enum Orientation
{
    north, east, south, west, none
}

public enum DungeonBuilderLayerMask
{
    PlayerCollider=30,
    Minimap=31
}

#endregion