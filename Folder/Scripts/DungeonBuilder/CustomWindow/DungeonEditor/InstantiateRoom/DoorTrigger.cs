using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{

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
