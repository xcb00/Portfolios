using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RoomCoordination
{
    public string id;
    public Vector2Int coordination;
    public RoomCoordination(string id, Vector2Int coordination)
    {
        this.id = id;
        this.coordination = coordination;
    }
}

[System.Serializable]
public class RoomCoordinateClass
{
    [SerializeField] string entranceID = string.Empty;
    [SerializeField] List<RoomCoordination> roomCoordinations;

    public Vector2Int GetRoomCoordination(string id)
    {
        if (roomCoordinations == null || roomCoordinations.Count < 1) return Vector2Int.zero;

        foreach (RoomCoordination room in roomCoordinations)
        {
            if (room.id.Equals(id))
                return room.coordination;
        }

        return Vector2Int.zero;
    }
    public string GetRoomID(Vector2Int coordination)
    {
        if (roomCoordinations.Count < 1)
            return string.Empty;

        foreach (RoomCoordination room in roomCoordinations)
        {
            if (room.coordination == coordination)
                return room.id;
        }

        return string.Empty;
    }
    public bool ContainID(string id)
    {
        foreach (RoomCoordination room in roomCoordinations)
        {
            if (room.id.Equals(id))
                return true;
        }
        return false;
    }
    public bool ContainCoordination(Vector2Int coordination)
    {
        foreach (RoomCoordination room in roomCoordinations)
        {
            if (room.coordination == coordination)
                return true;
        }
        return false;
    }

    Vector2Int GetDirection(int i)
    {
        switch (i % 4)
        {
            case 0: return Vector2Int.up;
            case 1: return Vector2Int.right;
            case 2: return Vector2Int.down;
            case 3: return Vector2Int.left;
            default:
                return Vector2Int.zero;
        }
    }

    public Orientation GetDoorwayOrientation(string currentRoomID, string nextRoomID)
    {
        Vector2Int direction = GetRoomCoordination(nextRoomID) - GetRoomCoordination(currentRoomID);
        if (direction == Vector2Int.up) return Orientation.north;
        else if (direction == Vector2Int.right) return Orientation.east;
        else if (direction == Vector2Int.down) return Orientation.south;
        else if (direction == Vector2Int.left) return Orientation.west;
        else return Orientation.none;
    }
    #region UNITY_EDITOR
#if UNITY_EDITOR
    public DungeonMapSO map;

    public RoomCoordinateClass(string entranceID, DungeonMapSO map)
    {
        this.entranceID = entranceID;
        if (this.map == null || map != this.map)
            this.map = map;
        Clear();
    }

    public void Clear()
    {
        if (roomCoordinations == null)
            roomCoordinations = new List<RoomCoordination>();
        else
            roomCoordinations.Clear();

        roomCoordinations.Add(new RoomCoordination(entranceID, Vector2Int.zero));
    }

    public void Remove(string id)
    {
        if (!map.roomDictionary.ContainsKey(id)) return;

        Queue<string> rooms = new Queue<string>();
        rooms.Enqueue(id);

        while (rooms.Count > 0)
        {
            DungeonRoomSO current = map.GetRoomNode(rooms.Dequeue());
            map.GetRoomNode(current.id).parentID = string.Empty;

            foreach (string childID in current.childrenID)
                rooms.Enqueue(childID);

            current.childrenID.Clear();

            int idx = -1;
            for(int i = 0; i < roomCoordinations.Count; i++)
            {
                if (roomCoordinations[i].id.Equals(current.id))
                {
                    idx = i;
                    break;
                }
            }

            if (idx > 0)
                roomCoordinations.RemoveAt(idx);
        }
    }

    public bool AddRoom(string currentID, string childID)
    {
        Vector2Int current = GetRoomCoordination(currentID);

        if(current==Vector2Int.zero&&!entranceID.Equals(currentID))
        {
            Debug.LogWarning("Current node is not conntected to the 'Entrance'.");
            return false;
        }

        int count = 0;
        int startDir = Random.Range(0, 4);
        Vector2Int childCoorination = Vector2Int.zero;

        for(count = 0;count< 4; count++)
        {
            childCoorination = current + GetDirection(startDir + count);
            if (!ContainCoordination(childCoorination))
                break;
        }

        if (count >= 4)
        {
            Debug.LogWarning("There are other nodes to the up, down, left, and right.");
            return false;
        }

        roomCoordinations.Add(new RoomCoordination(childID, childCoorination));
        return true;
    }

#endif
    #endregion
}

[System.Serializable]
public class Room2DDoorClass
{
    public Orientation orientation;
    public int width;
    public int height;
    public Vector2Int doorPosition;
    [Tooltip("현재 방이 다른 방과 연결되지 않을 경우 통로를 없애기 위해 타일을 복사할 위치로, 통로의 좌상단 위치")]
    public Vector2Int copyPosition;
    public GameObject doorPrefab;
}

[System.Serializable]
public class Room3DDoorClass
{
    public Orientation orientation;
    public Vector3 doorPosition;
    public GameObject doorPrefab;
    public GameObject WallPrefab;
}