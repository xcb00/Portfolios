using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName ="DungeonMap", menuName = "Scriptable Objects/Dungeon Map")]
public class DungeonMapSO : ScriptableObject
{
    [HideInInspector] public List<DungeonRoomSO> roomList = new List<DungeonRoomSO>();
    [HideInInspector] public Dictionary<string, DungeonRoomSO> roomDictionary = new Dictionary<string, DungeonRoomSO>();
    //[HideInInspector] 
    public RoomCoordinateClass roomCoordinateClass;
    [HideInInspector] public bool roomPositioned = false;

    private void Awake()
    {
        SetRoomNodeDictionary();
    }

    void SetRoomNodeDictionary()
    {
        roomDictionary.Clear();
        foreach (DungeonRoomSO room in roomList)
            roomDictionary[room.id] = room;
    }
    public void AddLastRoomOfListToDictionary()
    {
        if (roomList.Count - roomDictionary.Count == 1)
        {
            if (!roomDictionary.ContainsKey(roomList[roomList.Count - 1].id))
                roomDictionary[roomList[roomList.Count - 1].id] = roomList[roomList.Count - 1];
        }
        else
            SetRoomNodeDictionary();
    }
    
    public DungeonRoomSO GetRoomNode(string roomID) => roomDictionary.TryGetValue(roomID, out DungeonRoomSO room) ? room : null;

    #region UNITY_EDITOR
#if UNITY_EDITOR
    [HideInInspector] public DungeonRoomSO from = null;
    [HideInInspector] public Vector2 to = Vector2.zero;
    [HideInInspector] public string[] roomTypeArray = null;

    public void SetRoomTypeArray()
    {
        if (roomTypeArray == null || roomTypeArray.Length<1)
        {
            roomTypeArray = new string[(int)RoomType.Visible];
            for (int i = 0; i < (int)RoomType.Visible; i++)
                roomTypeArray[i] = EnumCaching.ToString((RoomType)i);
        }
    }

    public void OnValidate()
    {
        SetRoomNodeDictionary();
        SetRoomTypeArray();
    }

    public void DrawLine(DungeonRoomSO from, Vector2 to)
    {
        this.from = from;
        this.to = to;
    }

    public void ClearLine()
    {
        from = null;
        to = Vector2.zero;
        GUI.changed = true;
    }
    public void DisconnectNode(string currentID, string disconnectID, bool disconnectIDisChild)
    {
        DungeonRoomSO disconnect = GetRoomNode(disconnectID);

        if (disconnect == null)
            return;

        if (disconnectIDisChild)
        {
            disconnect.RemoveParentID(currentID);
            roomCoordinateClass.Remove(disconnectID);
        }
        else
        {
            disconnect.RemoveChildID(currentID);
            roomCoordinateClass.Remove(currentID);
        }
        AssetDatabase.SaveAssets();
    }

    // 부모 노드의 자식ID를 삭제하는 경우
    // roomID : 삭제하려는 노드
    // parentID : 부모 노드
    public void DisconnectChild(string roomID, string parentID)
    {
        DungeonRoomSO parent = GetRoomNode(parentID);

        if (parent != null)
            parent.RemoveChildID(roomID);
    }

    // 자식 노드의 부모ID를 삭제하는 경우
    // roomID : 삭제하려는 노드
    // childID : 자식 노드
    public void DisconnectParent(string roomID, string childID)
    {
        DungeonRoomSO child = GetRoomNode(childID);

        if (child != null)
            child.RemoveParentID(roomID);
    }

    public void SetRoomCoordinate()
    {
        if (roomList.Count < 1)
            return;

        roomCoordinateClass = new RoomCoordinateClass(roomList[0].id, this);
    }


#endif
    #endregion
}
