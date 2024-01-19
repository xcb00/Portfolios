using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="DungeonGraph", menuName ="Scriptable Objects/Dungeon Graph")]
public class DungeonGraphSO : ScriptableObject
{
    //public string name;
    public List<DungeonRoomSO> roomList = new List<DungeonRoomSO>();
    public Dictionary<string, DungeonRoomSO> roomDictionary = new Dictionary<string, DungeonRoomSO>();

    private void Awake()
    {
        SetRoomNodeDictionary();
    }

    void SetRoomNodeDictionary()
    {
        roomDictionary.Clear();

        foreach (DungeonRoomSO node in roomList)
            roomDictionary[node.id] = node;
    }

    public DungeonRoomSO GetRoomNode(string roomID) => roomDictionary.TryGetValue(roomID, out DungeonRoomSO room) ? room : null;

    //public bool RoomDictionaryContainKey(string key) => roomDictionary.ContainsKey(key);

    #region Editor Code
#if UNITY_EDITOR
    [HideInInspector] public DungeonRoomSO from = null;
    [HideInInspector] public Vector2 to = Vector2.zero;

    public void OnValidate()
    {
        SetRoomNodeDictionary();
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

    public void DisconnectNode(string deleteID, string disconnectID, bool disconnectIDisChild)
    {
        DungeonRoomSO disconnect = GetRoomNode(disconnectID);

        if (disconnect == null)
            return;

        if (disconnectIDisChild)
            disconnect.RemoveParentID(deleteID);
        else
            disconnect.RemoveChildID(deleteID);
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
#endif
#endregion
}
