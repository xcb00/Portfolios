using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class DungeonRoomSO : ScriptableObject
{
    [HideInInspector] public string id = string.Empty;
    [HideInInspector] public string parentID = string.Empty;
    [HideInInspector] public List<string> childrenID = new List<string>();
    [HideInInspector] public DungeonMapSO map;
    [HideInInspector] public RoomType type;
    [HideInInspector] public byte depth;

    #region UNITY_EDITOR
#if UNITY_EDITOR
    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;

    public void Initialise(Rect rect, RoomType type, DungeonMapSO map)
    {
        id = Guid.NewGuid().ToString();
        this.map = map;
        this.type = type;
        this.rect = rect;
        name = "RoomNode";
        depth = (byte)(type == RoomType.Entrance ? 0 : 255);
    }

    public void Draw(GUIStyle style)
    {
        GUILayout.BeginArea(rect, style);

        if (string.IsNullOrEmpty(parentID))
            depth = (byte)(type == RoomType.Entrance ? 0 : 255);

        if (!string.IsNullOrEmpty(parentID) || (int)type >= (int)RoomType.Visible)
        {
            EditorGUILayout.LabelField(EnumCaching.ToString(type));
            if (depth < 255)
            {
                EditorGUILayout.LabelField($"Depth : {depth}");
                EditorGUILayout.LabelField($"Coordinate : {map.roomCoordinateClass.GetRoomCoordination(id)}");
            }
        }
        else
        {
            int selected = (int)type;

            int selection = EditorGUILayout.Popup("", selected, map.roomTypeArray);

            type = (RoomType)selection;
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();
        GUI.changed = true;
    }
    #region Node Method
    #region DragNode
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
            GUI.changed = true;
        }
    }
    #endregion

    #region ConnectNode
    public void AddParentID(string roomID) => parentID = roomID;
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
        DungeonRoomSO child = map.GetRoomNode(roomID);

        if (child == null)
        {
            Debug.LogError($"'{roomID}' is not registered in the '{map.name}'");
            return false;
        }
        if (child.type == RoomType.Entrance)
        {
            Debug.LogWarning("�Ա��� �ڽ� ���� �߰��� �� �����ϴ�");
            return false;
        }

        if (type == RoomType.BossRoom)
        {
            Debug.LogWarning("�����뿡 �ڽ� ��带 �߰��� �� �����ϴ�");
            return false;
        }

        if (type == RoomType.Entrance && child.type == RoomType.BossRoom)
        {
            Debug.LogWarning("�������� �Ա��� ������ �� �����ϴ�");
            return false;
        }

        if (childrenID.Contains(roomID))
        {
            Debug.LogWarning("�̹� �ڽ� ���� �߰��߽��ϴ�");
            return false;
        }

        if (!string.IsNullOrEmpty(child.parentID))
        {
            Debug.LogWarning("�̹� �θ� ��带 ������ �ֽ��ϴ�");
            return false;
        }

        if (childrenID.Count >= 3)
        {
            Debug.LogWarning("�ڽ� ��带 �߰��� �� �����ϴ�");
            return false;
        }

        if (child.type == RoomType.None)
        {
            Debug.LogWarning("�ڽ� ����� Ÿ���� �����ؾ��մϴ�");
            return false;
        }

        if (CheckParentID(roomID, id))
        {
            Debug.LogWarning("�θ� ��带 �ڽ� ���� �߰��� �� �����ϴ�");
            return false;
        }

        if (!map.roomCoordinateClass.AddRoom(id, roomID))
        {
            return false;
        }

        child.depth = (byte)(depth + 1);
        return true;
    }

    bool CheckParentID(string roomID, string parentID)
    {
        bool result = false;

        DungeonRoomSO parent = map.GetRoomNode(parentID);

        if (parent == null)
            return true; ;

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

    #region RemoveNode
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
        if (!string.IsNullOrEmpty(parentID) && roomID.Equals(parentID))
        {
            parentID = string.Empty;
            return true;
        }
        else return false;
    }
    #endregion
    #endregion
#endif
    #endregion
}
