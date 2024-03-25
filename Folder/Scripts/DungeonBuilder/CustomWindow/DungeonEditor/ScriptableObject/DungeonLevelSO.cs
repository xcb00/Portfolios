using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    [HideInInspector]public bool is2D = false;
    public List<RoomDataSO> roomList;
    public List<DungeonMapSO> dungeonGraphList;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (roomList == null || roomList.Count == 0)
            Debug.LogWarning($"{this.name}'s roomList is null or empty");
        if(dungeonGraphList==null || roomList.Count == 0)
            Debug.LogWarning($"{this.name}'s dungeonGraphList is null or empty");

        EditorApplication.delayCall += DelayCall;
    }

    void DelayCall()
    {

        if (roomList.Count > 0)
        {
            is2D = roomList[0] is RoomData2DSO;
            Queue<RoomDataSO> deleteRoom = new Queue<RoomDataSO>();
            if (is2D)
            {
                for (int i = 0; i < roomList.Count; i++)
                {
                    if (!(roomList[i] is RoomData2DSO))
                    {
                        deleteRoom.Enqueue(roomList[i]);
                        continue;
                    }
                }
            }
            else
            {
                for (int i = 0; i < roomList.Count; i++)
                {
                    if (!(roomList[i] is RoomData3DSO))
                    {
                        deleteRoom.Enqueue(roomList[i]);
                        continue;
                    }
                }
            }

            while (deleteRoom.Count > 0)
                roomList.Remove(deleteRoom.Dequeue());
        }

        AssetDatabase.SaveAssets();
        EditorApplication.delayCall -= DelayCall;
    }
#endif
}


/*[HideInInspector] public bool is2D = false;
public List<RoomDataSO> roomList;
public List<DungeonMapSO> dungeonGraphList;
[HideInInspector]
public List<RoomData2DSO> room2DList;
[HideInInspector]
public List<RoomData3DSO> room3DList;

#if UNITY_EDITOR
private void OnValidate()
{
    if (roomList == null || roomList.Count == 0)
    {
        Debug.LogWarning($"{this.name}'s roomList is null or empty");

        if (room2DList != null)
            room2DList = null;
        if (room3DList != null)
            room3DList = null;
    }
    if (dungeonGraphList == null || roomList.Count == 0)
        Debug.LogWarning($"{this.name}'s dungeonGraphList is null or empty");

    EditorApplication.delayCall += DelayCall;
}

void DelayCall()
{

    if (roomList.Count > 0)
    {
        is2D = roomList[0] is RoomData2DSO;
        if (is2D)
        {
            if (room2DList == null)
                room2DList = new List<RoomData2DSO>();
            else
                room2DList.Clear();
            room3DList = null;
            for (int i = 0; i < roomList.Count; i++)
            {
                if (!(roomList[i] is RoomData2DSO))
                {
                    Debug.LogError($"{this.name}'s {roomList[i].name}(roomList[{i}]) is not RoomData2DSO.");
                    continue;
                }
                else if (!room2DList.Contains(roomList[i] as RoomData2DSO))
                    room2DList.Add(roomList[i] as RoomData2DSO);
                else
                {
                    Debug.LogWarning($"The duplicated value '{roomList[i].name}' exists in {this.name}'s roomList");
                    continue;
                }
            }
        }
        else
        {
            room2DList = null;
            if (room3DList == null)
                room3DList = new List<RoomData3DSO>();
            else
                room3DList.Clear();

            for (int i = 0; i < roomList.Count; i++)
            {
                if (!(roomList[i] is RoomData3DSO))
                {
                    Debug.LogError($"{this.name}'s {roomList[i].name}(roomList[{i}]) is not RoomData3DSO.");
                    continue;
                }
                else if (!room3DList.Contains(roomList[i] as RoomData3DSO))
                    room3DList.Add(roomList[i] as RoomData3DSO);
                else
                {
                    Debug.LogWarning($"The duplicated value '{roomList[i].name}' exists in {this.name}'s roomList");
                    continue;
                }
            }
        }
    }

    AssetDatabase.SaveAssets();
    EditorApplication.delayCall -= DelayCall;
}
#endif*/