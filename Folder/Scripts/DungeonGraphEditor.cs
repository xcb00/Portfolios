#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Text;
using NUnit.Framework.Internal.Commands;
using static Codice.CM.Common.CmCallContext;
using Codice.CM.WorkspaceServer.DataStore.WkTree;

// Ctrl + D : 선택한 노드들이
// Ctrl + V : graph.roomList를 검증 (OnDisable에서도 호출)
//            검증항목 : 1. 보스룸이 입구화 연결되어 있는지(에러), 생성한 룸 노드가 연결되어있는지(워닝)
// Ctrl + E : 선택한 노드들 삭제
// Shift + E : 선택한 노드들 간의 연결 삭제

// +a
// 그래프를 자동생성
// 입력값 : Vector2Int 방개수 범위; byte 최대뎁스, int 방 타입 별 생성 확률
public class DungeonGraphEditor : EditorWindow
{
    #region Vars
    // GUI Style
    GUIStyle defaultStyle;
    GUIStyle selectedStyle;

    Vector2 nodeSize = new Vector2(150f, 75f);

    // System
    readonly string[] soPaths = { "Assets", "ScriptableObjects", "DungeonGraphs" };
    StringBuilder sb;

    // Graph
    static DungeonGraphSO graph;
    const string ActiveGraph = "ActiveGraphID";
    DungeonRoomSO currentNode = null;
    bool draggingNode = false;

    Queue<RoomType> addRoomQueue = new Queue<RoomType>();

    // Grid
    Vector2 graphOffset;
    Vector2 graphDrag;

    // Validate
    bool connectBossRoom = false;
    int connectRoomCount = 0;

    // Graph Generator
    bool showGenerator = false;

    #endregion

    #region Open Window
    [MenuItem("Window/Custom Editor/Dungeon Graph Editor")]
    static void OpenWindow()
    {
        GetWindow<DungeonGraphEditor>("Dungeon Graph Editor");
    }

    [OnOpenAsset(0)]
    // 스크립터블 오브젝트를 더블클릭할 경우 에디터창을 열고 더블클릭한 스크립터블 오브젝트를 불러옴
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        DungeonGraphSO clickGraph = EditorUtility.InstanceIDToObject(instanceID) as DungeonGraphSO;

        if (clickGraph != null)
        {
            if (clickGraph != graph)
                SaveActiveGraph();
            OpenWindow();
            graph = clickGraph;

            return true;
        }
        return false;
    }

    void InspectorSelectionChanged()
    {
        DungeonGraphSO _graph = Selection.activeObject as DungeonGraphSO;
        if (_graph != null)
        {
            showGenerator = false;
            ValidateGraph();
            graph = _graph;
            graph.ClearLine();
            GUI.changed = true;
        }
    }

    void ActiveGraphChange()
    {
        string guid = EditorPrefs.GetString(ActiveGraph, "");

        if (!string.IsNullOrEmpty(guid))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            graph = AssetDatabase.LoadAssetAtPath<DungeonGraphSO>(path);
        }

        if (graph == null)
        {
            graph = ScriptableObject.CreateInstance<DungeonGraphSO>();

            CheckFolder();

            AssetDatabase.CreateAsset(graph, $"{sb}/DungeonGraph.asset");
            AssetDatabase.SaveAssets();
        }

        GUI.changed = true;
    }

    void CheckFolder(int index = 0)
    {
        if (index < soPaths.Length)
        {
            sb.Append(soPaths[index++]);

            if (index < soPaths.Length && !AssetDatabase.IsValidFolder($"{sb}/{soPaths[index]}"))
                AssetDatabase.CreateFolder(sb.ToString(), soPaths[index]);

            if (index < soPaths.Length)
                sb.Append("/");

            CheckFolder(index);
        }
    }

    static void SaveActiveGraph()
    {
        if (graph != null)
        {
            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(graph));
            EditorPrefs.SetString(ActiveGraph, guid);
        }
    }

    void DefineGUIStyle()
    {
        generatorStyle = EditorUtilities.DefineCustomGUIStyle(Vector2Int.one * 10, Vector2Int.one * 15, "Texture/CustomWindow/CustomTexture");

        //lineStyle = new GUIStyle(GUI.skin.box);
        lineStyle = new GUIStyle();
        Texture2D texture = new Texture2D(1, 1);
        Color color = Color.white;
        color.a = 0.5f;
        texture.SetPixel(0, 0, color);
        texture.Apply();
        lineStyle.normal.background = texture;

        defaultStyle = EditorUtilities.DefineUnityGUIStyle(new Vector2Int(25, 15), new Vector2Int(12, 12), NodeNumber.node1);
        selectedStyle = EditorUtilities.DefineUnityGUIStyle(new Vector2Int(25, 15), new Vector2Int(12, 12), NodeNumber.node1, NodeStyle.on);
    }
    #endregion

    #region Unity Event Function
    private void OnEnable()
    {
        if (sb == null)
            sb = new StringBuilder();
        else
            sb.Clear();

        Selection.selectionChanged += InspectorSelectionChanged;

        showGenerator = false;

        ActiveGraphChange();

        RegistInputEvent();

        graph.ClearLine();

        DefineGUIStyle();
    }

    private void OnDisable()
    {
        ValidateGraph();
        SaveActiveGraph();
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    private void OnGUI()
    {
        if (graph != null)
        {
            DrawBackgroundGrid(100f, 0.5f, Color.gray);
            DrawBackgroundGrid(25f, 0.2f, Color.gray);

            DrawDraggingLine();

            InputProcess(Event.current);

            DrawConnectionLine();

            DrawRoomNodes();

            if (showGenerator)
            {
                DrawGenerator();
            }
        }
        if (GUI.changed)
            Repaint();
    }
    #endregion

    #region Input Event Process
    EditorInputProcess inputEvent;
    void RegistInputEvent()
    {
        EditorProcessBuilder builder = new EditorProcessBuilder();
        inputEvent = builder.Build();

        builder.MouseEvent(ResetSelect, EditorMouseInput.LeftDown, (int)MouseDownEvent.NoneOver);
        builder.MouseEvent(NodeSelect, EditorMouseInput.LeftDown, (int)MouseDownEvent.NodeOver);

        builder.MouseEvent(EndDragGraph, EditorMouseInput.LeftUp, (int)MouseDownEvent.NoneOver);
        builder.MouseEvent(EndDragNode, EditorMouseInput.LeftUp, (int)MouseDownEvent.NodeOver);

        builder.MouseEvent(DragGraph, EditorMouseInput.LeftDrag, (int)MouseDownEvent.NoneOver);
        builder.MouseEvent(DragNode, EditorMouseInput.LeftDrag, (int)MouseDownEvent.NodeOver);

        builder.MouseEvent(ShowContextMenu, EditorMouseInput.RightDown, (int)MouseDownEvent.NoneOver);
        builder.MouseEvent(DrawConnectLine, EditorMouseInput.RightDown, (int)MouseDownEvent.NodeOver);

        builder.MouseEvent(DragConnectLine, EditorMouseInput.RightDrag, (int)MouseDownEvent.NoneOver);
        builder.MouseEvent(DragConnectLine, EditorMouseInput.RightDrag, (int)MouseDownEvent.NodeOver);

        builder.MouseEvent(ClearConnectLine, EditorMouseInput.RightUp, (int)MouseDownEvent.NoneOver);
        builder.MouseEvent(ConnectNodes, EditorMouseInput.RightUp, (int)MouseDownEvent.NodeOver);

        builder.KeyboardEvent(AllSelect, EditorKeyboardInput.Ctrl_A);
        builder.KeyboardEvent(ValidateGraph, EditorKeyboardInput.Ctrl_V);
        builder.KeyboardEvent(CreateRoomNode, EditorKeyboardInput.Ctrl_D);
        builder.KeyboardEvent(DeleteSelectedRoomNodes, EditorKeyboardInput.Ctrl_E);
        builder.KeyboardEvent(DisconnectSelectedNodes, EditorKeyboardInput.Shift_E);
        builder.KeyboardEvent(ResetSelect, EditorKeyboardInput.ESC);


    }

    EditorKeyboardInput GetKeyboardInput(Event _event)
    {
        EditorKeyboardInput input = EditorKeyboardInput.None;
        if (_event.modifiers == EventModifiers.Shift && _event.keyCode == KeyCode.E)
            input = EditorKeyboardInput.Shift_E;
        else if (_event.modifiers == EventModifiers.Control && _event.keyCode == KeyCode.E)
            input = EditorKeyboardInput.Ctrl_E;
        else if (_event.modifiers == EventModifiers.Control && _event.keyCode == KeyCode.A)
            input = EditorKeyboardInput.Ctrl_A;
        else if (_event.modifiers == EventModifiers.Control && _event.keyCode == KeyCode.S)
            input = EditorKeyboardInput.Ctrl_S;
        else if (_event.modifiers == EventModifiers.Control && _event.keyCode == KeyCode.V)
            input = EditorKeyboardInput.Ctrl_V;
        else if (_event.modifiers == EventModifiers.Control && _event.keyCode == KeyCode.D)
            input = EditorKeyboardInput.Ctrl_D;
        else if (_event.modifiers == EventModifiers.None && _event.keyCode == KeyCode.Escape)
            input = EditorKeyboardInput.ESC;

        return input;
    }

    EditorMouseInput GetMouseEvent(Event _event)
    {
        EditorMouseInput input = EditorMouseInput.None;


        if (_event.type == EventType.MouseDown && _event.button == 0)
            input = EditorMouseInput.LeftDown;
        else if (_event.type == EventType.MouseDrag && _event.button == 0)
            input = EditorMouseInput.LeftDrag;
        else if (_event.type == EventType.MouseUp && _event.button == 0)
            input = EditorMouseInput.LeftUp;
        else if (_event.type == EventType.MouseDown && _event.button == 1)
            input = EditorMouseInput.RightDown;
        else if (_event.type == EventType.MouseUp && _event.button == 1)
            input = EditorMouseInput.RightUp;
        else if (_event.type == EventType.MouseDrag && _event.button == 1)
            input = EditorMouseInput.RightDrag;
        /*if (_event.type == EventType.MouseDown && _event.button == 0)
            input = EditorMouseInput.LeftDown;
        else if (_event.type == EventType.ScrollWheel)
            input = EditorMouseInput.Wheel;
        else if (_event.type == EventType.MouseDown && _event.button == 1)
            input = EditorMouseInput.RightDown;
        else if (_event.type == EventType.ScrollWheel && _event.delta.y < 0)
            input = EditorMouseInput.WheelDown;*/

        return input;
    }

    void InputProcess(Event _event)
    {
        graphDrag = Vector2.zero;

        EditorMouseInput input = GetMouseEvent(_event);
        if (input == EditorMouseInput.None && _event.type==EventType.KeyUp)
            inputEvent.KeyInput(GetKeyboardInput(_event));
        else
        {
            if (!draggingNode)
                currentNode = IsMouseOverRoom(_event);
            inputEvent.MouseInput(_event, input,
                currentNode != null ? (int)MouseDownEvent.NodeOver : (int)MouseDownEvent.NoneOver);
        }
    }

    // 마우스가 노드 위에 있는지 체크하는 메소드
    DungeonRoomSO IsMouseOverRoom(Event _event)
    {
        // 현재 그래프의 룸 리스트를 반복하면서 마우스가 룸 위에 있는지 확인
        for (int i = graph.roomList.Count - 1; i >= 0; i--)
        {
            if (graph.roomList[i].rect.Contains(_event.mousePosition))
                return graph.roomList[i];
        }
        return null;
    }
    #endregion

    #region Input Event Callbacks
    void ShowContextMenu(Event _event)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("[Ctrl + D] Create Room Node"), false, CreateRoomNode, _event.mousePosition);

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("[Ctrl + V] Validate Graph or Update Depth"), false, ValidateGraph);

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("[Ctrl + A] Select All Room Nodes"), false, AllSelect);

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("[Ctrl + E] Delete Selected Nodes"), false, DeleteSelectedRoomNodes);
        menu.AddItem(new GUIContent("[Shift + E] Disconnect Selected Nodes"), false, DisconnectSelectedNodes);

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("Move to Entrance"), false, MoveToEntrance);
        //menu.AddItem(new GUIContent("Generate Graph"), false, AutoGraph);
        menu.AddItem(new GUIContent("Generate Graph"), false, ActiveGenerator, _event.mousePosition);

        menu.ShowAsContext();
    }

    void ShowNodeMenu(Event _event)
    {
        ResetSelect();
        NodeSelect(_event);

        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Delete Node"), false, DeleteSelectedRoomNodes);
        menu.AddItem(new GUIContent("Disconnect"), false, DisconnectSelectedNode);
        menu.AddItem(new GUIContent("Change Type/Small Room"), false, ChangeRoomType, 1);
        menu.AddItem(new GUIContent("Change Type/Medium Room"), false, ChangeRoomType, 2);
        menu.AddItem(new GUIContent("Change Type/Large Room"), false, ChangeRoomType, 3);
        menu.AddItem(new GUIContent("Change Type/Chest Room"), false, ChangeRoomType, 4);

        menu.ShowAsContext();
    }

    #region Drag Event
    void DragGraph(Event _event)
    {
        if (draggingNode)
            return;

        graphDrag = _event.delta;

        for (int i = 0; i < graph.roomList.Count; i++)
            graph.roomList[i].DragNode(_event.delta);

        GUI.changed = true;
    }

    void DragNode(Event _event)
    {
        if (!currentNode.isLeftClickDragging)
        {
            SelectOnlyCuurentNode();
        }

        if (!draggingNode)
            draggingNode = true;

        currentNode.DragNode(_event.delta);
    }

    void DragConnectLine(Event _event)
    {
        graph.to += _event.delta;
    }

    void DrawConnectLine(Event _event)
    {
        SelectOnlyCuurentNode();
        graph.DrawLine(currentNode, _event.mousePosition);
    }

    void EndDragNode(Event _event)
    {
        draggingNode = false;
        currentNode.LeftUp();
    }

    void EndDragGraph(Event _event)
    {
        for (int i = 0; i < graph.roomList.Count; i++)
            graph.roomList[i].LeftUp();
    }

    void ConnectNodes(Event _event)
    {
        if (graph.from != null)
        {
            DungeonRoomSO child = IsMouseOverRoom(_event);

            if (child != null)
            {
                if (child.id.Equals(graph.from.id))
                    ShowNodeMenu(_event);
                else if(graph.from.AddChildID(child.id))
                    child.AddParentID(graph.from.id);
            }

            graph.ClearLine();
            UpdateDepth();
        }
    }

    void ClearConnectLine(Event _event)
    {
        graph.ClearLine();
    }
    #endregion

    #region Select Event
    void NodeSelect(Event _event)
    {
        currentNode.isSelected = !currentNode.isSelected;
        GUI.changed = true;
    }

    void ResetSelect(Event _event)
    {
        ResetSelect();
    }

    void ResetSelect()
    {
        foreach (DungeonRoomSO room in graph.roomList)
        {
            if (room.isSelected)
                room.isSelected = false;
        }
        GUI.changed = true;
    }

    void AllSelect()
    {
        foreach (DungeonRoomSO room in graph.roomList)
        {
            if (!room.isSelected)
                room.isSelected = true;
        }
        GUI.changed = true;
    }

    void SelectOnlyCuurentNode()
    {
        foreach (DungeonRoomSO room in graph.roomList)
        {
            if (room == currentNode)
                room.isSelected = true;
            else
                room.isSelected = false;
        }
    }
    #endregion

    #region Validate Event
    // Ctrl + V or Disable 시 생성한 그래프 검증
    // 1. [에러] 입구부터 보스룸까지 연결되어있는지
    // 2. [워닝] 생성한 노드들 중 입구에 연결되어있지 않은 노드가 있는지
    void ValidateGraph()
    {
        if (graph == null || graph.roomList.Count < 1)
            return;

        connectBossRoom = false;
        connectRoomCount = 0;

        if (!UpdateDepth(true))
            return;

        if (!connectBossRoom)
            Debug.LogError($"{graph.name} Error : Entrance와 BossRoom이 연결되어있지 않습니다");
        else if (connectRoomCount != graph.roomList.Count)
            Debug.LogWarning($"{graph.name} Warring : 연결되지 않은 노드가 있습니다");
        else
            Debug.Log($"{graph.name} has no issues");
    }

    bool UpdateDepth(bool writeLog = false)
    {
        for (int i = 1; i < graph.roomList.Count; i++)
            graph.roomList[i].depth = 255;

        SearchDungeonGraph(graph.roomList[0]);

        return writeLog;
    }

    void SearchDungeonGraph(DungeonRoomSO room)
    {
        // Validate
        connectRoomCount++;
        if (room.roomType == RoomType.BossRoom)
            connectBossRoom = true;

        if (room.childrenID.Count > 0)
        {
            foreach (string childID in room.childrenID)
            {
                DungeonRoomSO child = graph.GetRoomNode(childID);
                child.depth = (byte)(room.depth + 1);
                SearchDungeonGraph(child);
            }
        }
    }
    #endregion

    #region Create Event
    void CreateRoomNode(object mousePosition)
    {
        CheckDefaultRoom((Vector2)mousePosition);

        CreateRoom((Vector2)mousePosition);
    }

    void CreateRoomNode()
    {
        addRoomQueue.Clear();

        foreach (DungeonRoomSO room in graph.roomList)
        {
            if (room.isSelected)
            {
                if ((int)room.roomType < (int)RoomType.Count)
                    addRoomQueue.Enqueue(room.roomType);
            }
        }

        if (addRoomQueue.Count < 1)
            CreateRoomNode(Event.current.mousePosition);
        else
        {
            int queueCnt = addRoomQueue.Count;
            while(addRoomQueue.Count > 0)
                CreateRoom(Event.current.mousePosition + Vector2.one * 10f * (queueCnt-addRoomQueue.Count), addRoomQueue.Dequeue());
        }
    }

    void CreateRoom(Vector2 position, RoomType roomType = RoomType.None)
    {
        DungeonRoomSO room = ScriptableObject.CreateInstance<DungeonRoomSO>();

        graph.roomList.Add(room);

        room.Initialise(new Rect(position, nodeSize), roomType, graph);

        AssetDatabase.AddObjectToAsset(room, graph);

        AssetDatabase.SaveAssets();

        graph.OnValidate();
    }

    void CheckDefaultRoom(Vector2 position)
    {
        if (graph.roomList.Count > 0)
            return;

        CreateRoom(position - Vector2.one * 100f, RoomType.Entrance);
        CreateRoom(position - Vector2.one * 50f, RoomType.BossRoom);

        GUI.changed = true;
    }
    #endregion

    #region Delete Event
    void DisconnectSelectedNode()
    {
        if (!string.IsNullOrEmpty(currentNode.parentID)) 
        {
            DungeonRoomSO parent = graph.GetRoomNode(currentNode.parentID);
            if(parent!=null)
                parent.isSelected = true;
        }

        foreach(string childID in currentNode.childrenID)
        {
            DungeonRoomSO child = graph.GetRoomNode(childID);
            if (child != null)
                child.isSelected = true;
        }

        DisconnectSelectedNodes();
    }
    void DisconnectSelectedNodes()
    {
        foreach(DungeonRoomSO room in graph.roomList)
        {
            if (room.isSelected && room.childrenID.Count>0)
            {
                for(int i = room.childrenID.Count - 1; i >= 0; i--)
                {
                    DungeonRoomSO child = graph.GetRoomNode(room.childrenID[i]);

                    if (child.isSelected)
                    {
                        child.RemoveParentID(room.id);
                        room.RemoveChildID(child.id);
                    }
                }
            }
        }

        UpdateDepth();
    }

    void DeleteSelectedRoomNodes()
    {
        // 리스트를 반복문을 이용해 탐색할 때, 반복문 안에서 리스트의 값을 생성/삭제 시 오류가 발생하기 때문에 큐에 삭제할 노드를 추가한 후 반복문 종료 후 일괄삭제
        Queue<DungeonRoomSO> delete = new Queue<DungeonRoomSO>();

        // roomList를 반복하면서 선택된 노드를 delete에 추가
        foreach(DungeonRoomSO room in graph.roomList)
        {
            if ((int)room.roomType > (int)RoomType.Count && room.isSelected)
                Debug.LogWarning($"{Enums.ToString(room.roomType)}을 삭제할 수 없습니다");
            else if (room.isSelected)
            {
                delete.Enqueue(room);

                // 현재 노드와 연결된 부모/자식 노드간의 연결 삭제
                if (!string.IsNullOrEmpty(room.parentID))
                    graph.DisconnectNode(room.id, room.parentID, false);

                foreach (string childID in room.childrenID)
                    graph.DisconnectNode(room.id, childID, true);
            }
        }

        while (delete.Count > 0)
        {
            DungeonRoomSO deleteRoom = delete.Dequeue();

            graph.roomDictionary.Remove(deleteRoom.id);
            graph.roomList.Remove(deleteRoom);

            DestroyImmediate(deleteRoom, true);

            AssetDatabase.SaveAssets();
        }

        UpdateDepth();
    }
    #endregion

    void ChangeRoomType(object roomType)
    {
        if ((int)currentNode.roomType > (int)RoomType.Count)
        {
            Debug.LogWarning($"{currentNode.roomType}을 다른 타입으로 변경할 수 없습니다");
            return;
        }

        int type = (int)roomType;
        currentNode.roomType = (RoomType)type;
    }

    void ActiveGenerator(object position)
    {
        showGenerator = true;
        generatorRect = new Rect(Vector2.zero, generatorSize);        
    }    

    void MoveToEntrance()
    {
        Vector2 gap = graph.roomList[0].rect.position;

        int i = 0;

        foreach(DungeonRoomSO room in graph.roomList)
        {
            if (!string.IsNullOrEmpty(room.parentID))
                room.DragNode(-gap);
            else
                room.rect.position = Vector2.zero + Vector2.one * 30f * i++;
        }
    }
    #endregion

    #region Draw GUI

    void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor)
    {
        int verticalLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
        int horizontalLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);

        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        graphOffset += graphDrag * 0.5f;

        Vector3 gridOffset = new Vector3(graphOffset.x % gridSize, graphOffset.y % gridSize, 0f);

        for (int i = 0; i < verticalLineCount; i++)
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0f) + gridOffset, new Vector3(gridSize * i, position.height + gridSize, 0f) + gridOffset);

        for (int j = 0; j < horizontalLineCount; j++)
            Handles.DrawLine(new Vector3(-gridSize, gridSize * j, 0f) + gridOffset, new Vector3(position.width + gridSize, gridSize * j, 0f) + gridOffset);

        Handles.color = Color.white;
    }

    void DrawRoomNodes()
    {
        foreach (DungeonRoomSO room in graph.roomList)
        {
            if (room.isSelected)
                room.Draw(selectedStyle);
            else
                room.Draw(defaultStyle);
        }

        GUI.changed = true;
    }

    void DrawDraggingLine()
    {
        if (graph.to != Vector2.zero)
            Handles.DrawBezier(graph.from.rect.center, graph.to, graph.from.rect.center, graph.to, Color.white, null, 3.0f);
    }

    void DrawConnectionLine()
    {
        foreach (DungeonRoomSO room in graph.roomList)
        {
            if (room.childrenID.Count > 0)
            {
                foreach (string childID in room.childrenID)
                {
                    if (graph.roomDictionary.ContainsKey(childID))
                    {
                        DrawConnectionLine(room.rect.center, graph.roomDictionary[childID].rect.center);
                        GUI.changed = true;
                    }
                }
            }
        }
    }

    void DrawConnectionLine(Vector2 from, Vector2 to)
    {
        float arrowSize = 5.0f;
        float arrowWidth = 3.0f;
        float arrowAngle = 60f;

        Vector2 mid = (from + to) * 0.5f;
        Vector2 dir = to - from;
        dir.Normalize();

        Vector2 arrowHeigt = new Vector2(-dir.y, dir.x) * arrowSize;
        Vector2 arrowBase = (dir * arrowSize) / Mathf.Tan(Mathf.Deg2Rad * arrowAngle * 0.5f);

        mid -= arrowBase * 0.5f;
        Vector2 tail1 = mid - arrowHeigt;
        Vector2 tail2 = mid + arrowHeigt;
        Vector2 head = mid + arrowBase;

        Handles.DrawBezier(head, tail1, head, tail1, Color.white, null, arrowWidth);
        Handles.DrawBezier(head, tail2, head, tail2, Color.white, null, arrowWidth);

        Handles.DrawBezier(from, to, from, to, Color.white, null, 3.0f);
        GUI.changed = true;
    }

    #endregion

    #region Graph Generator
    GUIStyle generatorStyle = null;
    GUIStyle lineStyle = null;
    Vector2 generatorSize = new Vector2(200f, 280f);
    Rect generatorRect = Rect.zero;
    int minRoom = 5;
    int maxRoom = 20;
    int nonePercentage = 20;
    int smallPercentage = 20;
    int mediumPercentage = 20;
    int largePercentage = 20;
    int chestPercentage = 20;
    int roomCount = 0;

    void DrawGenerator()
    {
        GUILayout.BeginArea(generatorRect, generatorStyle);

        EditorGUILayout.LabelField("Graph Generator", EditorStyles.boldLabel);
        GUILayout.Box("", lineStyle, GUILayout.ExpandWidth(true), GUILayout.Height(1));

        minRoom = Mathf.Clamp(EditorGUILayout.IntField("Minimum Room Count", minRoom), 3, maxRoom);
        maxRoom = Mathf.Clamp(EditorGUILayout.IntField("Maximum Room Count", maxRoom), minRoom, 255);
        nonePercentage = Mathf.Clamp(EditorGUILayout.IntField("None Percentage", nonePercentage), 0, 100);
        smallPercentage = Mathf.Clamp(EditorGUILayout.IntField("Small Room Percentage", smallPercentage), 0, 100);
        mediumPercentage = Mathf.Clamp(EditorGUILayout.IntField("Medium Room Percentage", mediumPercentage), 0, 100);
        largePercentage = Mathf.Clamp(EditorGUILayout.IntField("Large Room Percentage", largePercentage), 0, 100);
        chestPercentage = Mathf.Clamp(EditorGUILayout.IntField("Chest Room Percentage", chestPercentage), 0, 100);
        int total = nonePercentage + smallPercentage + mediumPercentage + largePercentage + chestPercentage;
        EditorGUILayout.LabelField($"Total Percentage : {total}");

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Generate"))
        {
            showGenerator = false;

            if (total == 100)
                GenerateGraph();
            else
                Debug.LogWarning("방이 생성될 확률의 합이 100이여야 합니다");
        }

        if(GUILayout.Button("Init Values"))
        {
            InitGenerateGraphValues();
        }

        if (GUILayout.Button("Close"))
            showGenerator = false;
        

        GUILayout.EndArea();
    }

    void InitGenerateGraphValues()
    {
        minRoom = 5;
        maxRoom = 20;

        nonePercentage = 20;
        smallPercentage = 20;
        mediumPercentage = 20;
        largePercentage = 20;
        chestPercentage = 20;
    }

    void GenerateGraph()
    {
        if (graph.roomList.Count < 1)
            CreateRoomNode();

        AllSelect();
        graph.roomList[0].isSelected = false;
        graph.roomList[1].isSelected = false;

        DeleteSelectedRoomNodes();
        ResetSelect();

        roomCount = Random.Range(minRoom, maxRoom + 1);
                
        AutoGenerate(graph.roomList[0]);
        ConnectBossRoom();
        RepositionRoom();
    }

    RoomType GetRandomRoomType()
    {
        int probability = Random.Range(0, 100);
        int result;
        int[] percentage = { nonePercentage, smallPercentage, mediumPercentage, largePercentage, chestPercentage };

        for(result = 0; result < percentage.Length; result++)
        {
            if (probability < percentage[result])
                break;

            probability -= percentage[result];
        }

        return (RoomType)result;
    }

    void AutoGenerate(DungeonRoomSO room)
    {
        if (graph.roomList.Count > roomCount)
            return;

        if (room.childrenID.Count < 1)
        {
            CreateChild(room);
            AutoGenerate(ShiftSibling(room));
        }
        else
            AutoGenerate(graph.GetRoomNode(room.childrenID[0]));
    }

    DungeonRoomSO ShiftSibling(DungeonRoomSO current)
    {
        if (current.roomType == RoomType.Entrance)
            return graph.GetRoomNode(current.childrenID[0]);

        DungeonRoomSO parent = graph.GetRoomNode(current.parentID);
        //Debug.Log("ShiftSibling's parent : " + parent.name);
        int idx = parent.childrenID.FindIndex(x => x == current.id);// + 1) % 3;
                                                                    //int idx = (parent.childrenID.FindIndex(x => x == current.id) + 1) % 3;

        //return graph.GetRoomNode(parent.childrenID[idx]);

        if (++idx < parent.childrenID.Count)
            return graph.GetRoomNode(parent.childrenID[idx]);
        else
            return graph.GetRoomNode(parent.childrenID[0]);
    }

    bool CreateChild(DungeonRoomSO room)
    {
        if (room.childrenID.Count > 0)
            return false;

        if (room.childrenID.Count < 1)
        {
            for (int i = 0; i < 3; i++)
            {
                RoomType type = GetRandomRoomType();

                if (type == RoomType.None)
                    break;

                CreateRoom(Vector2.one, type, room);
            }
        }

        if (room.childrenID.Count < 1)
            CreateRoom(Vector2.one, RoomType.SmallRoom, room);
        return true;
    }

    void CreateRoom(Vector2 position, RoomType roomType, DungeonRoomSO currentRoom)
    {

        DungeonRoomSO child = ScriptableObject.CreateInstance<DungeonRoomSO>();

        graph.roomList.Add(child);

        child.Initialise(new Rect(position, new Vector2(150f, 75f)), roomType, graph);

        graph.OnValidate();

        if (currentRoom.AddChildID(child.id))
            child.AddParentID(currentRoom.id);

        AssetDatabase.AddObjectToAsset(child, graph);

        AssetDatabase.SaveAssets();
    }

    void ConnectBossRoom()
    {
        DungeonRoomSO parent = graph.roomList[graph.roomList.Count - 1];

        if (parent.AddChildID(graph.roomList[1].id))
            graph.roomList[1].AddParentID(parent.id);
    }

    void RepositionRoom()
    {
        int length = 1;
        int row = 0;
        int col = 0;

        while (length * length < roomCount)
            length++;
                
        for(int i = 0; i < graph.roomList.Count; i++)
        {
            row = (int)(i / length);
            col = (int)(i % length);

            graph.roomList[i].rect.position = new Vector2(nodeSize.x * col + 50f, nodeSize.y * row + 50f);
        }
    }
    #endregion
}
#endif
