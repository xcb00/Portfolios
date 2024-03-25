#region UNITY_EDITOR
#if UNITY_EDITOR
#region EditorInputProcess
public enum EditorKeyboardInput
{
    None,
    Ctrl_S,
    Ctrl_A,
    Ctrl_V,
    Ctrl_D,
    Ctrl_R,
    Ctrl_G,
    Ctrl_E,
    Shift_E,
    ESC
}

public enum EditorMouseInput
{
    None,
    LeftDown, LeftUp, LeftDrag,
    RightDown, RightUp, RightDrag,
}

public enum MouseDownEvent
{
    NoneOver,
    NodeOver
}
#endregion

#region GUIStyle
public enum NodeNumber
{
    node1, node2, node3, node4, node5, node6
}

public enum NodeStyle
{
    none, on
}

#endregion
#endif
#endregion