using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Content;
public class DungeonRoomSO : ScriptableObject
{
    //[HideInInspector] 
    public string id;// { get; private set; } // 외부에서 id값을 변경하지 못하도록 프로퍼티 사용
    //[HideInInspector] 
    public string parentID = string.Empty;
    //[HideInInspector] 
    public List<string> childrenID = new List<string>();
    [HideInInspector] public DungeonGraphSO graph;
    [HideInInspector] public RoomType roomType;
    [HideInInspector] public byte depth;

    #region Editor Code
#if UNITY_EDITOR
    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;
    public void Initialise(Rect rect, RoomType roomType, DungeonGraphSO graph)
    {
        id = Guid.NewGuid().ToString();
        this.graph = graph;
        this.roomType = roomType;
        this.rect = rect;
        this.name = "RoomNode";
        depth = (byte)(roomType == RoomType.Entrance ? 0 : 255);
    }

    public void Draw(GUIStyle style)
    {
        GUILayout.BeginArea(rect, style);

        if (string.IsNullOrEmpty(parentID))
            depth = (byte)(roomType == RoomType.Entrance ? 0 : 255);

        if (!string.IsNullOrEmpty(parentID) || (int)roomType >= (int)RoomType.Count)
        {
            EditorGUILayout.LabelField(Enums.ToString(roomType));
            if (depth < 255)
                EditorGUILayout.LabelField($"Depth : {depth}");
        }
        else
        {
            int selected = (int)roomType;

            int selection = EditorGUILayout.Popup("", selected, GetRoomType());

            roomType = (RoomType)selection;
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();

        GUI.changed = true;
    }

    // public
    string[] GetRoomType()
    {
        string[] typeArray = new string[(int)RoomType.Count];
        for (int i = 0; i < (int)RoomType.Count; i++)
            typeArray[i] = Enums.ToString((RoomType)i);
        return typeArray;
    }

    #region Input Event Callbacks
    public void DragNode(Vector2 delta)
    {
        isLeftClickDragging = true;
        rect.position += delta;
        EditorUtility.SetDirty(this);
        GUI.changed = true;
    }

    public void LeftUp()
    {
        if (isLeftClickDragging)
        {
            isLeftClickDragging = false;
            isSelected = false;
        }
        GUI.changed = true;
    }
    #endregion

    #region Connect Nodes
    public void AddParentID(string roomID)
    {
        parentID = roomID;
    }

    public bool AddChildID(string roomID)
    {
        if (CanAddChildID(roomID))
        {
            childrenID.Add(roomID);
            return true;
        }
        else
            return false;
    }

    bool CanAddChildID(string roomID)
    {
        DungeonRoomSO child = graph.GetRoomNode(roomID);

        if (child.roomType == RoomType.Entrance)
        {
            Debug.LogWarning("입구를 자식 노드로 추가할 수 없습니다");
            return false;
        }

        if (roomType == RoomType.BossRoom)
        {
            Debug.LogWarning("보스룸에 자식 노드를 추가할 수 없습니다");
            return false;
        }

        /*if (id.Equals(roomID))
        {
            Debug.LogWarning("자신을 자식으로 추가할 수 없습니다");
            return false;
        }*/

        if (childrenID.Contains(roomID))
        {
            Debug.LogWarning("이미 자식 노드로 추가했습니다");
            return false;
        }

        if (!string.IsNullOrEmpty(child.parentID))
        {
            Debug.LogWarning("이미 부모 노드를 가지고 있습니다");
            return false;
        }

        if (childrenID.Count >= 3)
        {
            Debug.LogWarning("자식 노드를 추가할 수 없습니다");
            return false;
        }

        if (child.roomType == RoomType.None)
        {
            Debug.LogWarning("자식 노드의 타입을 설정해야합니다");
            return false;
        }

        if (CheckParentID(roomID, id))
        {
            Debug.LogWarning("부모 노드를 자식 노드로 추가할 수 없습니다");
            return false;
        }

        if (depth < 255)
            child.depth = (byte)(depth + 1);

        return true;
    }

    // 자식으로 추가하려는 노드가 부모 노드에 있는지 확인
    bool CheckParentID(string roomID, string parentID)
    {
        bool result = false;

        DungeonRoomSO parent = graph.GetRoomNode(parentID);

        if (!string.IsNullOrEmpty(parent.parentID))
        {
            if (roomID.Equals(parent.parentID))
                result = true;
            else
                result = CheckParentID(roomID, parent.parentID);
        }

        return result;
    }

    #endregion

    #region Remove Nodes
    public bool RemoveChildID(string roomID)
    {
        if (childrenID.Contains(roomID))
        {
            childrenID.Remove(roomID);
            return true;
        }
        else
            return false;
    }

    public bool RemoveParentID(string roomID)
    {
        if (roomID.Equals(parentID))
        {
            parentID = string.Empty;
            return true;
        }
        else
            return false;
    }
    #endregion
#endif
    #endregion
}
