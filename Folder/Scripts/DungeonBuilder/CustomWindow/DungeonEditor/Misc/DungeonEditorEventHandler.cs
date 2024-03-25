using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DungeonEditorEventHandler
{

    public static Action RoomEnterEvent;
    public static void CallRoomEnterEvent() { RoomEnterEvent?.Invoke(); }

    public static Action RoomClearEvent;
    public static void CallRoomClearEvent() { RoomClearEvent?.Invoke(); }
}
