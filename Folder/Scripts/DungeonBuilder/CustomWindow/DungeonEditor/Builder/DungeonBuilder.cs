//using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonBuilder : Singleton<DungeonBuilder>
{
#if UNITY_EDITOR
    // 실제 게임을 개발할 때는, GameManager에서 DungeonBuilder와 Player를 관리하는 것이 좋음
    public Sample3DPlayer p3 = null;
    public Sample2DPlayer p2 = null;
    #endif
    public LayerMask playerCollider;
    List<RoomDataSO>[] roomTypeArray = new List<RoomDataSO>[6];
    bool is2D = false;
    Dictionary<Vector2Int, InstantRoom2D> room2DDictionary;
    Dictionary<Vector2Int, InstantRoom3D> room3DDictionary;
    public Vector2Int currentRoom;
    public Minimap minimap = null;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        #region Edtior Code
        #if UNITY_EDITOR
        // 에디터 상에서 문 열고/닫힘을 쉽게 테스트 하기 위한 코드
        // 숫자 1을 눌렀을 경우 문이 열리고, 숫자 2를 눌렀을 경우 문이 닫힘
        if (Input.GetKeyDown(KeyCode.Alpha1))
            DungeonEditorEventHandler.CallRoomClearEvent();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            DungeonEditorEventHandler.CallRoomEnterEvent();
        if (Input.GetKeyDown(KeyCode.R))
            Start();
        #endif
        #endregion
    }

    public DungeonLevelSO _lv;

    private void Start()
    {
        GenerateDungeon(_lv);

#if UNITY_EDITOR
        if (is2D)
            p2.MovePosition(Change2DRoom());
        else
            p3.MovePosition(Change3DRoom());
#endif
    }

    public void GenerateDungeon(DungeonLevelSO level)
    {
        currentRoom = Vector2Int.zero;
        ClearDungeon(level.is2D);

        InitRoomTypeArray(level.roomList, level.name);

        if(level.is2D)
            Instantiate2DRoom(level.dungeonGraphList[Random.Range(0, level.dungeonGraphList.Count)]);
        else
            Instantiate3DRoom(level.dungeonGraphList[Random.Range(0, level.dungeonGraphList.Count)]);

        ClearRoomTypeArray();
    }

    void ClearDungeon(bool is2D)
    {
        this.is2D = is2D;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        if (is2D)
        {
            room3DDictionary = null;
            if (room2DDictionary == null)
                room2DDictionary = new Dictionary<Vector2Int, InstantRoom2D>();
            else
                room2DDictionary.Clear();
        }
        else
        {
            room2DDictionary = null;
            if (room3DDictionary == null)
                room3DDictionary = new Dictionary<Vector2Int, InstantRoom3D>();
            else
                room3DDictionary.Clear();
        }

    }

    void InitRoomTypeArray(List<RoomDataSO> roomList, string levelName)
    {
        for (int i = 0; i < roomTypeArray.Length; i++)
            roomTypeArray[i] = new List<RoomDataSO>();

        foreach(RoomDataSO data in roomList)
        {
            int idx = RoomTypeToIndex(data.roomType);
            if (idx < 0)
            {
                Debug.LogError($"The roomType of {data.name} is incorrectly set");
                continue;
            }

            if (!roomTypeArray[idx].Contains(data))
                roomTypeArray[idx].Add(data);
        }

        for (int i = 0; i < roomTypeArray.Length; i++)
        {
            if (roomTypeArray[i].Count < 1)
            {
                if (i < 4)
                    Debug.LogWarning($"There is no object of {EnumCaching.ToString((RoomType)(i + 1))} in {levelName}'s roomList");
                else
                    Debug.LogError($"There is no object of {EnumCaching.ToString((RoomType)(i + 2))} in {levelName}'s roomList");
            }
        }
    }

    int RoomTypeToIndex(RoomType type)
    {
        if (type == RoomType.None || type == RoomType.Visible)
            return -1;
        else if ((int)type < (int)RoomType.Visible)
            return (int)type - 1;
        else
            return (int)type - 2;
    }

    void ClearRoomTypeArray()
    {
        for (int i = 0; i < roomTypeArray.Length; i++)
            roomTypeArray[i] = null;
    }

    void Instantiate2DRoom(DungeonMapSO map)
    {
        room2DDictionary = new Dictionary<Vector2Int, InstantRoom2D>();

        Queue<string> roomIDs = new Queue<string>();
        roomIDs.Enqueue(map.roomCoordinateClass.GetRoomID(Vector2Int.zero));
        while (roomIDs.Count > 0)
        {
            DungeonRoomSO room = map.GetRoomNode(roomIDs.Dequeue());
            Vector2Int currentCoordinate = map.roomCoordinateClass.GetRoomCoordination(room.id);

            // Instantiate Room Object
            int typeIndex = RoomTypeToIndex(room.type);

            if (typeIndex < 0)
                continue;

            int roomIndex = Random.Range(0, roomTypeArray[typeIndex].Count);

            GameObject instantObject = Instantiate(roomTypeArray[typeIndex][roomIndex].roomPrefab,
                Vector3.zero, Quaternion.identity, transform);
            InstantRoom2D instanRoom = instantObject.GetComponentInChildren<InstantRoom2D>();

            byte _doorway = 0;

            if (!string.IsNullOrEmpty(room.parentID))
            {
                Orientation parentOrientation = map.roomCoordinateClass.GetDoorwayOrientation(room.id, room.parentID);
                _doorway = DoorwayToByte(_doorway, parentOrientation);
                instanRoom.DoorwayConnect(parentOrientation);

            }

            foreach (string childID in room.childrenID)
            {
                roomIDs.Enqueue(childID);
                Orientation childOrientation = map.roomCoordinateClass.GetDoorwayOrientation(room.id, childID);
                _doorway = DoorwayToByte(_doorway, childOrientation);
                instanRoom.DoorwayConnect(childOrientation);
            }

            minimap.AddMinimapDictionary(currentCoordinate, _doorway);

            RoomData2DSO roomData = roomTypeArray[typeIndex][roomIndex] as RoomData2DSO;
            instanRoom.Initialise(roomData.doorways, roomData.spawnPosition);
            instanRoom.DisableRoom();

            room2DDictionary.Add(currentCoordinate, instanRoom);
        }
    }

    void Instantiate3DRoom(DungeonMapSO map)
    {
        room3DDictionary = new Dictionary<Vector2Int, InstantRoom3D>();
        Queue<string> roomIDs = new Queue<string>();
        roomIDs.Enqueue(map.roomCoordinateClass.GetRoomID(Vector2Int.zero));
        while (roomIDs.Count > 0)
        {
            DungeonRoomSO room = map.GetRoomNode(roomIDs.Dequeue());
            Vector2Int currentCoordinate = map.roomCoordinateClass.GetRoomCoordination(room.id);

            // Instantiate Room Object
            int typeIndex = RoomTypeToIndex(room.type);

            if (typeIndex < 0)
                continue;

            int roomIndex = Random.Range(0, roomTypeArray[typeIndex].Count);

            GameObject instantObject = Instantiate(roomTypeArray[typeIndex][roomIndex].roomPrefab,
                Vector3.zero, Quaternion.identity, transform);
            InstantRoom3D instanRoom = instantObject.GetComponentInChildren<InstantRoom3D>();

            byte _doorway = 0;

            if (!string.IsNullOrEmpty(room.parentID))
            {
                Orientation parentOrientation = map.roomCoordinateClass.GetDoorwayOrientation(room.id, room.parentID);
                _doorway = DoorwayToByte(_doorway, parentOrientation);
                instanRoom.DoorwayConnect(parentOrientation);

            }

            foreach (string childID in room.childrenID)
            {
                roomIDs.Enqueue(childID);
                Orientation childOrientation = map.roomCoordinateClass.GetDoorwayOrientation(room.id, childID);
                _doorway = DoorwayToByte(_doorway, childOrientation);
                instanRoom.DoorwayConnect(childOrientation);
            }

            minimap.AddMinimapDictionary(currentCoordinate, _doorway);

            RoomData3DSO roomData = roomTypeArray[typeIndex][roomIndex] as RoomData3DSO;
            instanRoom.Initialise(roomData.doorways, roomData.spawnPosition);
            instanRoom.DisableRoom();;

            room3DDictionary.Add(currentCoordinate, instanRoom);
        }

    }

    public Vector3 ChangeRoom(Orientation orientation = Orientation.none)
    {
        DungeonEditorEventHandler.CallRoomEnterEvent();

        if (is2D)
            return Change2DRoom(orientation);
        else
            return Change3DRoom(orientation);
    }

    Vector3 Change2DRoom(Orientation orientation = Orientation.none)
    {
        room2DDictionary[currentRoom].DisableRoom();
        Vector2Int nextCoordinate = currentRoom + OrientationToVector2Int(orientation);
        Vector2Int nextPosition = room2DDictionary[nextCoordinate].GetSpawnPosition(orientation);

#if UNITY_EDITOR
        p2.MovePosition(new Vector3(nextPosition.x, nextPosition.y, 0f));
#endif
        room2DDictionary[nextCoordinate].EnableRoom();
        currentRoom = nextCoordinate;

        if (minimap.minimapDictionary[currentRoom].activeSelf)
            DungeonEditorEventHandler.CallRoomClearEvent();
        else
            minimap.minimapDictionary[currentRoom].SetActive(true);

        return new Vector3(nextPosition.x, nextPosition.y, 0f);
    }

    Vector3 Change3DRoom(Orientation orientation = Orientation.none)
    {
        Vector2Int nextCoordinate = currentRoom + OrientationToVector2Int(orientation);
        room3DDictionary[currentRoom].DisableRoom();
#if UNITY_EDITOR
        p3.MovePosition(room3DDictionary[nextCoordinate].GetSpawnPosition(orientation));
#endif
        room3DDictionary[nextCoordinate].EnableRoom();
        currentRoom = nextCoordinate;

        if (minimap.minimapDictionary[currentRoom].activeSelf)
            DungeonEditorEventHandler.CallRoomClearEvent();
        else
            minimap.minimapDictionary[currentRoom].SetActive(true);

        return room3DDictionary[nextCoordinate].GetSpawnPosition(orientation);
    }

    byte DoorwayToByte(byte doorwayByte, Orientation doorwayOrientation)
    {
        switch (doorwayOrientation)
        {
            case Orientation.north:
                doorwayByte |= 1 << 0;
                break;
            case Orientation.east:
                doorwayByte |= 1 << 1;
                break;
            case Orientation.south:
                doorwayByte |= 1 << 2;
                break;
            case Orientation.west:
                doorwayByte |= 1 << 3;
                break;
            case Orientation.none:
            default:
                break;
        }
        return doorwayByte;
    }

    public Vector2Int OrientationToVector2Int(Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.north: return Vector2Int.up;
            case Orientation.east: return Vector2Int.right;
            case Orientation.south: return -Vector2Int.up;
            case Orientation.west: return -Vector2Int.right;
            case Orientation.none:
            default:
                return Vector2Int.zero;
        }
    }

    public Vector3 OrientationToVector3(Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.north: return Vector3.forward;
            case Orientation.east: return Vector3.right;
            case Orientation.south: return -Vector3.forward;
            case Orientation.west: return -Vector3.right;
            case Orientation.none:
            default:
                return Vector3.zero;
        }
    }
}
