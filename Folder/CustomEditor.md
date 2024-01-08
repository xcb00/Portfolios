## Custom Editor
- 입력의 경우 빌더패턴과 Action을 사용해 구현
- 키 입력 시 실행할 메소드를 `Dictionary<Enum, Action>`을 이용해 저장
  > - 동적 생성과 쉬운 접근을 위해 Dictionary를 사용
  > - 마우스 입력의 경우 상황에 같은 입력이라도 상황에 따라 다른 이벤트가 발생될 수 있으므로 리스트를 사용
- 휴먼 에러를 줄이기 위해 `try catch`를 이용해 예외처리 및 에디터창에 예외사항 출력
- 유니티 에디터창에서만 실행되기 때문에 전처리기를 사용해 메모리 최적화

## Code

<details>
<summary>EditorProcessBuilder.cs</summary>

```C#
using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR 

#region Editor Keyboard Input Enum
public enum EditorKeyboardInput
{
    Ctrl_S,
    Ctrl_A,
    Ctrl_Shift_A
}

public enum EditorMouseInput
{
    None,
    LeftDown, LeftUp, LeftDrag,
    RightDown, RightUp, RightDrag,
    WheelDown, WheelUp
}
#endregion

public class EditorInputProcess
{
    public Dictionary<EditorMouseInput, List<Action<Event>>> MouseInputDic;
    public Dictionary<EditorKeyboardInput, Action> KeyboardInputDic;
    public int MouseEventCount(EditorMouseInput input) => MouseInputDic[input].Count;

    public EditorInputProcess()
    {
        MouseInputDic = new Dictionary<EditorMouseInput, List<Action<Event>>>();
        KeyboardInputDic = new Dictionary<EditorKeyboardInput, Action>();
    }

    public void InputProcess(Event _event, int index = -1)
    {
        if (_event.type == EventType.KeyDown)
            KeyInput(_event);
        else
            MouseInput(_event, index);
    }

    void KeyInput(Event _event)
    {
        if (_event.modifiers == (EventModifiers.Shift | EventModifiers.Control) && _event.keyCode == KeyCode.A)
            KeyboardInputDic[EditorKeyboardInput.Ctrl_Shift_A]?.Invoke();
        else if (_event.modifiers == EventModifiers.Control && _event.keyCode == KeyCode.A)
            KeyboardInputDic[EditorKeyboardInput.Ctrl_A]?.Invoke();
        else if (_event.modifiers == EventModifiers.Control && _event.keyCode == KeyCode.S)
            KeyboardInputDic[EditorKeyboardInput.Ctrl_S]?.Invoke();
    }

    void MouseInput(Event _event, int index)
    {
        EditorMouseInput input = EditorMouseInput.None;

        if (_event.type == EventType.MouseDown && _event.button == 0)
            input = EditorMouseInput.LeftDown;
        else if (_event.type == EventType.MouseDown && _event.button == 1)
            input = EditorMouseInput.RightDown;

        try
        {
            if (input == EditorMouseInput.None) // 마우스가 에디터창에서 움직일 경우
                return;
            else if (MouseInputDic[input].Count <= index)
                throw new Exception(ErrorCode.IndexOutOfRangeException.ToString());
            else
                MouseInputDic[input][index]?.Invoke(_event);
        }
        catch(Exception e)
        {
            EditorUtilities.PrintErrorMessage(e.Message, $"입력한 인덱스[{index}]가 {input}의 크기[{MouseInputDic[input].Count}]보다 크거나 같습니다", 3);
        }
    }
}

public class EditorProcessBuilder
{
    EditorInputProcess process;
    public EditorProcessBuilder()
    {
        process = new EditorInputProcess();
    }

    public EditorProcessBuilder MouseEvent(Action<Event> func, EditorMouseInput input, int index)
    {
        try
        {
            if (!process.MouseInputDic.ContainsKey(input))
                process.MouseInputDic[input] = new List<Action<Event>>();

            if (process.MouseInputDic[input].Count < index)
                throw new Exception(ErrorCode.IndexOutOfRangeException.ToString());
            else if (process.MouseInputDic[input].Count == index)
                process.MouseInputDic[input].Add(func);
            else
                process.MouseInputDic[input][index] += func;
        }
        catch(Exception e)
        {
            EditorUtilities.PrintErrorMessage(e.Message, $"입력한 인덱스[{index}]가 {input} 리스트의 크기[{process.MouseEventCount(input)}]보다 같거나 작아야합니다", 2);
        }
        return this;
    }

    public EditorProcessBuilder KeyboardEvent(Action func, EditorKeyboardInput input)
    {
        try
        {
            if (process.KeyboardInputDic.ContainsKey(input))
                throw new Exception(ErrorCode.IndexOutOfRangeException.ToString());
            else
                process.KeyboardInputDic[input] = func;
        }
        catch (Exception e)
        {
            EditorUtilities.PrintErrorMessage(e.Message, $"{input}에 이미 등록된 함수가 있습니다", 2);
        }
        return this;
    }   

    public EditorInputProcess Build()
    {
        return process;
    }
}
#endif
```  
</details>

<details>
<summary>EditorUtilities.cs</summary>

```C#
using UnityEngine;
using UnityEditor;

# if UNITY_EDITOR

#region Debbug Enum
public enum ErrorCode
{
    NullReferenceException,         // 오브젝트나 클래스의 인스턴스가 아직 생성되지 않았는데 참조할 경우
    IndexOutOfRangeException,       // 배열이나 리스트 등의 컬렉션에서 범위를 초과하는 인덱스에 접근할 경우
    //ArgumentException,              // 잘못된 인수를 메소드에 전달할 경우
    //InvalidOperationException,      // 메소드 호출이 객체의 현재 상태에 대해 유효하지 않을 경우(열거형이 수정된 상태에서 열거형을 계속 사용할 경우 >> foreach문으로 list를 반복하는 중에 list를 삭제/추가할 경우)
    //MissingReferenceException,      // Destory된 오브젝트를 참조할 경우
    //UnassingedReferenceException    // 인스펙터에서 참조를 할당하지 않은 상태에서 참조할 경우(public을 사용하지 않은 변수를 참조할 경우)
}
#endregion

#region NodeTexture Enum
public enum NodeNumber
{
    node1, node2, node3, node4, node5, node6
}

public enum NodeStyle
{
    none, on
}
#endregion

public static class EditorUtilities
{
#if UNITY_EDITOR
    #region PrintErrorMessage
    public static void PrintErrorMessage(string errorCode, string errorMessage, int stackIndex = 1)
    {
        System.Diagnostics.StackTrace stack = new System.Diagnostics.StackTrace(true);

        /*bool check = false;
        if (check)
        {
            for (int i = stack.FrameCount - 1; i >= 0; i--)
            {
                System.Diagnostics.StackFrame f = stack.GetFrame(i);
                Debug.Log($"[{i} : {f.GetFileName()}");
            }
        }*/

        System.Diagnostics.StackFrame frame = stack.GetFrame(stackIndex);
        Debug.LogError($"{errorCode} : {System.IO.Path.GetFileName(frame.GetFileName())} : {frame.GetFileLineNumber()}" +
                $"\n{errorMessage}");
    }
    #endregion

    #region Define GUI Style
    // 커스텀 윈도우에 생성될 노드의 스타일을 정의하는 메소드
    public static GUIStyle DefineGUIStyle(Vector2Int padding, Vector2Int border, NodeNumber number, NodeStyle style = NodeStyle.none)
    {
        GUIStyle result = new GUIStyle();
        string name = string.Concat(Enums.ToString(number), style == NodeStyle.none ? string.Empty : $" {Enums.ToString(style)}");
        result.normal.background = EditorGUIUtility.Load(name) as Texture2D;
        result.normal.textColor = Color.white;
        result.padding = new RectOffset(padding.x, padding.x, padding.y, padding.y);
        result.border = new RectOffset(border.x, border.x, border.y, border.y);
        return result;
    }
    #endregion

    #region DrawBackgroundGrid
    // 커스텀 윈도우에서 격자(Grid)를 그리는 메소드
    public static void DrawBackgroundGrid(Vector2 size, Vector2 grapOffset, float gridSize, float gridOpacity, Color gridColor)
    {
        int verticalLineCount = Mathf.CeilToInt((size.x + gridSize) / gridSize);
        int horizontalLineCount = Mathf.CeilToInt((size.y + gridSize) / gridSize);

        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        Vector3 offset = new Vector3((grapOffset.x * 0.5f) % gridSize, (grapOffset.y * 0.5f) % gridSize, 0f);

        for (int i = 0; i < verticalLineCount; i++)
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0f) + offset, new Vector3(gridSize * i, size.y + gridSize, 0f) + offset);
        for (int i = 0; i < horizontalLineCount; i++)
            Handles.DrawLine(new Vector3(-gridSize, gridSize * i, 0f) + offset, new Vector3(size.x + gridSize, gridSize * i, 0f) + offset);
        Handles.color = Color.white;
    }
    #endregion
#endif
}
#endif    
```
</details>

<details>
<summary>DungeonGraphEditor.cs</summary>

```C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

#if UNITY_EDITOR
public class DungeonGraphEditor : EditorWindow
{
    #region Vars
    // GUI Style
    GUIStyle defaultStyle;
    GUIStyle selectedStyle;
    #endregion

    [MenuItem("Window/Custom Editor/Dungeon Graph Editor")]
    static void OpenWindow()
    {
        GetWindow<DungeonGraphEditor>("Dungeon Graph Editor");
    }

    private void OnEnable()
    {
        RegistInputEvent();
        defaultStyle = EditorUtilities.DefineGUIStyle(new Vector2Int(25, 25), new Vector2Int(12, 12), NodeNumber.node1);
        selectedStyle = EditorUtilities.DefineGUIStyle(new Vector2Int(25, 25), new Vector2Int(12, 12), NodeNumber.node1, NodeStyle.on);

    }

    private void OnGUI()
    {
        inputEvent.InputProcess(Event.current, 0);
    }

    #region Input Event Process
    EditorInputProcess inputEvent;
    void RegistInputEvent()
    {
        EditorProcessBuilder builder = new EditorProcessBuilder();
        inputEvent = builder.Build();
        builder.MouseEvent((e) => { Debug.Log(e.mousePosition); }, EditorMouseInput.LeftDown, 0);

    }
    #endregion
}
#endif
```
</details>





