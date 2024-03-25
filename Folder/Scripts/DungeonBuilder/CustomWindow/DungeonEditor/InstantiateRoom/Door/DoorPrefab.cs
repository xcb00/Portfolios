using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPrefab : MonoBehaviour
{
    [HideInInspector] public Orientation orientation;

    public void SpawnDoorPrefab(Orientation doorOrientation)
    {
        orientation = doorOrientation;
    }

    protected virtual void OnEnable()
    {
        DungeonEditorEventHandler.RoomEnterEvent += EnterRoom;
        DungeonEditorEventHandler.RoomClearEvent += ClearRoom;
    }

    private void OnDisable()
    {
        DungeonEditorEventHandler.RoomEnterEvent -= EnterRoom;
        DungeonEditorEventHandler.RoomClearEvent -= ClearRoom;
    }


    protected virtual void EnterRoom() { }

    protected virtual void ClearRoom() { }

   
}
