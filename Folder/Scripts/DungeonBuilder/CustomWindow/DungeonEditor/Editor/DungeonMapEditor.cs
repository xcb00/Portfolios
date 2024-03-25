#if UNITY_EDITOR
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using UnityEditor.Graphs;


public class DungeonMapEditor : EditorWindow
{
    #region Variable
    #region Editor variable
    // GUI Style
    GUIStyle defaultStyle;
    GUIStyle selectedStyle;

    // Grid
    Vector2 graphOffset;
    Vector2 graphDrag;

    // Graph Paths
    readonly string[] soPath = { "Assets", "ScriptableObjects", "DungeonMaps" };

    StringBuilder sb;

    Vector2 nodeSize = new Vector2(169f, 100f);

    // 마지막 DungeonMapSO를 저장하기 위한 키
    const string CurrentMap = "CurrentMap";

    static DungeonMapSO currentMap;
    DungeonRoomSO currentNode = null;

    bool draggingNode = false;
    bool draggingGraph = false;

    Queue<RoomType> addRoomQueue = new Queue<RoomType>();

    bool connectBossRoom = false;
    int connectRoomCount = 0;

    bool showGenerator = false;

    static Vector2 windowSize = Vector2.zero;
    #endregion

    #region MapGenerator Variable
    GUIStyle generatorStyle;
    GUIStyle lineStyle;
    Vector2 generatorSize = new Vector2(200f, 280f);
    Rect generatorRect = Rect.zero;
    Rect nodeRect = Rect.zero;

    int minRoom = 5;
    int maxRoom = 10;
    int nonePercentage = 20;
    int smallPercentage = 20;
    int mediumPercentage = 20;
    int largePercentage = 20;
    int chestPercentage = 20;
    int roomCount = 0;
    int maxAttempt = 10;
    #endregion

    #region Grid Variable
    Vector2Int largeLineCount = Vector2Int.zero;
    Vector2Int smallLineCount = Vector2Int.zero;
    Color largeLineColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    Color smallLineColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);
    Vector3 largeGridOffset = Vector3.zero;
    Vector3 smallGridOffset = Vector3.zero;
    #endregion

    EditorInputProcess inputProcess;
    #endregion

    #region Open Window
    [MenuItem("Custom Editor/Dungeon Map Editor")]
    static void OpenWindow()
    {
        EditorWindow window = GetWindow<DungeonMapEditor>("Dungeon Map Editor");
        windowSize = window.position.size;
    }

    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        DungeonMapSO clickMap = EditorUtility.InstanceIDToObject(instanceID) as DungeonMapSO;

        if (clickMap != null)
        {
            if (clickMap != currentMap)
                SaveActiveGraph();

            OpenWindow();

            currentMap = clickMap;

            //currentMap.SetRoomTypeArray();

            return true;
        }
        return false;
    }

    void InspectorSelectionChanged()
    {
        DungeonMapSO map = Selection.activeObject as DungeonMapSO;
        if (map != null)
        {
            showGenerator = false;
            draggingGraph = false;
            draggingNode = false;
            ValidateGraph();
            currentMap = map;
            currentMap.ClearLine();
            //currentMap.SetRoomTypeArray();
            AssetDatabase.SaveAssets();
            GUI.changed = true;
        }
    }

    void ActiveGraphChange()
    {
        string guid = EditorPrefs.GetString(CurrentMap, "");

        if (!string.IsNullOrEmpty(guid))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            currentMap = AssetDatabase.LoadAssetAtPath<DungeonMapSO>(path);
        }

        if (currentMap == null)
        {
            currentMap = ScriptableObject.CreateInstance<DungeonMapSO>();
            CheckFolder();

            AssetDatabase.CreateAsset(currentMap, $"{sb}/DungeonMap.asset");
            currentMap.SetRoomTypeArray();
            AssetDatabase.SaveAssets();
        }

        GUI.changed = true;
    }

    void CheckFolder(int index = 0)
    {
        if (index < soPath.Length)
        {
            sb.Append(soPath[index++]);

            if (index < soPath.Length && !AssetDatabase.IsValidFolder($"{sb}/{soPath[index]}"))
                AssetDatabase.CreateFolder(sb.ToString(), soPath[index]);

            if (index < soPath.Length)
                sb.Append('/');

            CheckFolder(index);
        }
    }

    static void SaveActiveGraph()
    {
        if (currentMap != null)
            EditorPrefs.SetString(CurrentMap, AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(currentMap)));
    }

    void DefineGUIStyle()
    {
        generatorStyle = CustomEditorUtility.DefineRectangleGUIStyle(Color.black, 0.2f);
        lineStyle = CustomEditorUtility.DefineRectangleGUIStyle(Color.white, 0.1f);
        defaultStyle = CustomEditorUtility.DefineUnityGUIStyle(new Vector2Int(15, 15), new Vector2Int(12, 10), NodeNumber.node1, NodeStyle.none);
        selectedStyle = CustomEditorUtility.DefineUnityGUIStyle(new Vector2Int(15, 15), new Vector2Int(12, 10), NodeNumber.node1, NodeStyle.on);
    }
    #endregion

    #region Unity Event Method
    private void OnEnable()
    {
        if (sb == null)
            sb = new StringBuilder();
        else
            sb.Clear();

        Selection.selectionChanged += InspectorSelectionChanged;

        showGenerator = false;
        draggingGraph = false;
        draggingNode = false;

        ActiveGraphChange();

        RegistInputEvent();

        currentMap.ClearLine();

        DefineGUIStyle();
    }

    private void OnDisable()
    {
        ValidateGraph();
        SaveActiveGraph();
        Selection.selectionChanged -= InspectorSelectionChanged;
    }
    #endregion

    #region OnGUI
    private void OnGUI()
    {
        // 유니티에서 윈도우창의 크기가 변경될 때, 호출하는 이벤트를 제공하지 않기 때문에, OnGUI에서 매 프레임마다 윈도우창의 변화를 검사해야함
        // 윈도우 창의 크기가 변화하지 않을 때도 매 프레임마다 윈도우 창의 크기를 비교하는 것을 최적화하기 위해 Repaint 이벤트가 발생할 때마다 검사하는 것으로 변경
        // GUI를 새로 그릴 때마다 윈도우 창의 크기를 검사하지만, 매 프레임마다 비교하는 것보단 적게 검사함
        if (Event.current.type == EventType.Repaint)
        {
            if (windowSize != position.size)
                windowSize = position.size;

            CalculateGrid(100f, 25f);
        }

        if (currentMap != null)
        {
            DrawGrid(100f, 25f);

            DrawDraggingLine();

            InputProcess(Event.current);

            DrawConnectionLine();

            DrawNodes();

            if (showGenerator)
            {
                DrawGenerator();
            }
        }

        // 만약 변경된 GUI가 있는 경우 GUI를 다시 그림
        if (GUI.changed)
            Repaint();
    }
    #endregion

    #region Input Event Process
    void RegistInputEvent()
    {
        EditorProcessBuilder builder = new EditorProcessBuilder();
        inputProcess = builder.Build();

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
        builder.KeyboardEvent(RepositionRoom, EditorKeyboardInput.Ctrl_R);
        builder.KeyboardEvent(ActiveGenerator, EditorKeyboardInput.Ctrl_G);
        builder.KeyboardEvent(ResetSelect, EditorKeyboardInput.ESC);


    }

    #region Get Input Event Enum
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
        else if (_event.modifiers == EventModifiers.Control && _event.keyCode == KeyCode.R)
            input = EditorKeyboardInput.Ctrl_R;
        else if (_event.modifiers == EventModifiers.Control && _event.keyCode == KeyCode.G)
            input = EditorKeyboardInput.Ctrl_G;
        else if (_event.modifiers == EventModifiers.None && _event.keyCode == KeyCode.Escape)
            input = EditorKeyboardInput.ESC;

        return input;
    }

    EditorMouseInput GetMouseInput(Event _event)
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
    #endregion

    void InputProcess(Event _event)
    {
        graphDrag = Vector2.zero;
        EditorMouseInput input = GetMouseInput(_event);

        if (input == EditorMouseInput.None && _event.type == EventType.KeyUp)
            inputProcess.KeyInput(GetKeyboardInput(_event));
        else
        {
            if (!draggingNode)
                currentNode = IsMouseOverNode(_event);

            inputProcess.MouseInput(_event, input, currentNode != null ? (int)MouseDownEvent.NodeOver : (int)MouseDownEvent.NoneOver);
        }
    }

    DungeonRoomSO IsMouseOverNode(Event _event)
    {
        for (int i = currentMap.roomList.Count - 1; i >= 0; i--)
        {
            if (currentMap.roomList[i].rect.Contains(_event.mousePosition))
                return currentMap.roomList[i];
        }
        return null;
    }
    #endregion

    #region Input Event Callback
    #region ContextMenu
    void ShowContextMenu(Event _event)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("[Ctrl + D] Create Room Node"), false, CreateRoomNode, _event.mousePosition);

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("[Ctrl + V] Validate Graph or Update Depth"), false, ValidateGraph);

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("[Ctrl + A] Select All Room Nodes"), false, AllSelect);
        menu.AddItem(new GUIContent("Select Entrance Node"), false, SelectOnlyEntrance);
        menu.AddItem(new GUIContent("Select Boss Room Node"), false, SelectOnlyBossRoom);

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("[Ctrl + E] Delete Selected Nodes"), false, DeleteSelectedRoomNodes);
        menu.AddItem(new GUIContent("[Shift + E] Disconnect Selected Nodes"), false, DisconnectSelectedNodes);

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("[Ctrl + R] Reposition Node"), false, RepositionRoom);
        menu.AddItem(new GUIContent("[Ctrl + G] Generate Graph"), false, ActiveGenerator, _event.mousePosition);

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
    #endregion

    #region Drag Callback
    void DragGraph(Event _event)
    {
        if (draggingNode)
            return;

        draggingGraph = true;

        graphDrag = _event.delta;

        for (int i = 0; i < currentMap.roomList.Count; i++)
            currentMap.roomList[i].DragNode(_event.delta);

        GUI.changed = true;
    }

    void DragNode(Event _event)
    {
        if (draggingGraph)
            return;

        if (!currentNode.isLeftClickDragging)
        {
            SelectOnlyCurrentNode();
        }

        if (!draggingNode)
            draggingNode = true;

        currentNode.DragNode(_event.delta);
    }

    void DragConnectLine(Event _event)
    {
        currentMap.to += _event.delta;
    }

    void DrawConnectLine(Event _event)
    {
        SelectOnlyCurrentNode();
        currentMap.DrawLine(currentNode, _event.mousePosition);
    }

    void EndDragNode(Event _event)
    {
        draggingNode = false;
        currentNode.LeftUp();
    }

    void EndDragGraph(Event _event)
    {
        draggingGraph = false;
        for (int i = 0; i < currentMap.roomList.Count; i++)
            currentMap.roomList[i].LeftUp();
    }

    void ConnectNodes(Event _event)
    {
        if (currentMap.from != null)
        {
            DungeonRoomSO child = IsMouseOverNode(_event);

            if (child != null)
            {
                if (child.id.Equals(currentMap.from.id))
                    ShowNodeMenu(_event);
                else if (currentMap.from.AddChildID(child.id))
                    child.AddParentID(currentMap.from.id);
            }

            currentMap.ClearLine();
            UpdateDepth();
        }
        AssetDatabase.SaveAssets();
    }

    void ClearConnectLine(Event _event)
    {
        currentMap.ClearLine();
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
        foreach (DungeonRoomSO room in currentMap.roomList)
        {
            if (room.isSelected)
                room.isSelected = false;
        }
        GUI.changed = true;
    }

    void AllSelect()
    {
        foreach (DungeonRoomSO room in currentMap.roomList)
        {
            if (!room.isSelected)
                room.isSelected = true;
        }
        GUI.changed = true;
    }

    void SelectOnlyCurrentNode()
    {
        foreach (DungeonRoomSO room in currentMap.roomList)
        {
            if (room == currentNode)
                room.isSelected = true;
            else
                room.isSelected = false;
        }
        GUI.changed = true;
    }

    void SelectOnlyEntrance()
    {
        foreach (DungeonRoomSO room in currentMap.roomList)
        {
            if (room.type==RoomType.Entrance)
                room.isSelected = true;
            else
                room.isSelected = false;
        }
        GUI.changed = true;
    }

    void SelectOnlyBossRoom()
    {
        foreach (DungeonRoomSO room in currentMap.roomList)
        {
            if (room.type == RoomType.BossRoom)
                room.isSelected = true;
            else
                room.isSelected = false;
        }
        GUI.changed = true;
    }
    #endregion

    #region Validate Event
    // Ctrl + V or Disable 시 생성한 그래프 검증
    // 1. [에러] 입구부터 보스룸까지 연결되어있는지
    // 2. [워닝] 생성한 노드들 중 입구에 연결되어있지 않은 노드가 있는지
    void ValidateGraph()
    {
        if (currentMap == null || currentMap.roomList.Count < 1)
            return;

        connectRoomCount = 0;

        if (!UpdateDepth(true))
            return;

        if (!connectBossRoom)
            Debug.LogError($"{currentMap.name} Error : Entrance와 BossRoom이 연결되어있지 않습니다");
        else if (connectRoomCount != currentMap.roomList.Count)
            Debug.LogWarning($"{currentMap.name} Warring : 연결되지 않은 노드가 있습니다");
        else
            RoomReposition();

    }

    bool UpdateDepth(bool writeLog = false)
    {
        connectBossRoom = false;
        for (int i = 1; i < currentMap.roomList.Count; i++)
            currentMap.roomList[i].depth = 255;

        SearchDungeonGraph(currentMap.roomList[0]);

        return writeLog;
    }

    void SearchDungeonGraph(DungeonRoomSO room)
    {
        // Validate
        connectRoomCount++;
        if (room.type == RoomType.BossRoom)
            connectBossRoom = true;

        foreach (string childID in room.childrenID)
        {
            DungeonRoomSO child = currentMap.GetRoomNode(childID);
            child.depth = (byte)(room.depth + 1);
            SearchDungeonGraph(child);
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

        foreach (DungeonRoomSO room in currentMap.roomList)
        {
            if (room.isSelected)
            {
                if ((int)room.type < (int)RoomType.Visible)
                    addRoomQueue.Enqueue(room.type);
            }
        }

        if (addRoomQueue.Count < 1)
            CreateRoomNode(Event.current.mousePosition);
        else
        {
            int queueCnt = addRoomQueue.Count;
            while (addRoomQueue.Count > 0)
                CreateRoom(Event.current.mousePosition + Vector2.one * 10f * (queueCnt - addRoomQueue.Count), addRoomQueue.Dequeue());
        }
    }

    void CreateRoom(Vector2 position, RoomType roomType = RoomType.None)
    {
        DungeonRoomSO room = ScriptableObject.CreateInstance<DungeonRoomSO>();

        currentMap.roomList.Add(room);

        room.Initialise(new Rect(position, nodeSize), roomType, currentMap);

        currentMap.roomPositioned = false;

        AssetDatabase.AddObjectToAsset(room, currentMap);

        AssetDatabase.SaveAssets();

        currentMap.OnValidate();
    }

    void CheckDefaultRoom(Vector2 position)
    {
        if (currentMap.roomList.Count > 0)
            return;

        CreateRoom(position - Vector2.one * 100f, RoomType.Entrance);
        CreateRoom(position - Vector2.one * 50f, RoomType.BossRoom);

        currentMap.SetRoomCoordinate();

        GUI.changed = true;
    }
    #endregion

    #region Delete Event
    void DisconnectSelectedNode()
    {
        if (!string.IsNullOrEmpty(currentNode.parentID))
        {
            DungeonRoomSO parent = currentMap.GetRoomNode(currentNode.parentID);
            if (parent != null)
                parent.isSelected = true;
        }

        foreach (string childID in currentNode.childrenID)
        {
            DungeonRoomSO child = currentMap.GetRoomNode(childID);
            if (child != null)
                child.isSelected = true;
        }

        DisconnectSelectedNodes();
    }
    void DisconnectSelectedNodes()
    {
        foreach (DungeonRoomSO room in currentMap.roomList)
        {
            if (room.isSelected && room.childrenID.Count > 0)
            {
                for (int i = room.childrenID.Count - 1; i >= 0; i--)
                {
                    DungeonRoomSO child = currentMap.GetRoomNode(room.childrenID[i]);

                    if (child.isSelected)
                    {
                        currentMap.DisconnectNode(child.id, room.id, false);
                        currentMap.DisconnectNode(room.id, child.id, true);
                    }
                }
            }
        }

        currentMap.roomPositioned = false;

        UpdateDepth();
    }

    void DeleteSelectedRoomNodes()
    {
        // 리스트를 반복문을 이용해 탐색할 때, 반복문 안에서 리스트의 값을 생성/삭제 시 오류가 발생하기 때문에 큐에 삭제할 노드를 추가한 후 반복문 종료 후 일괄삭제
        Queue<DungeonRoomSO> delete = new Queue<DungeonRoomSO>();

        // roomList를 반복하면서 선택된 노드를 delete에 추가
        foreach (DungeonRoomSO room in currentMap.roomList)
        {
            if (room.isSelected)
            {
                // 현재 노드와 연결된 부모/자식 노드간의 연결 삭제
                if (!string.IsNullOrEmpty(room.parentID))
                    currentMap.DisconnectNode(room.id, room.parentID, false);

                foreach (string childID in room.childrenID)
                    currentMap.DisconnectNode(room.id, childID, true);
                delete.Enqueue(room);

            }
        }

        while (delete.Count > 0)
        {
            DungeonRoomSO deleteRoom = delete.Dequeue();

            if (deleteRoom.type == RoomType.BossRoom)
            {
                deleteRoom.parentID = string.Empty;
                continue;
            }
            else if (deleteRoom.type == RoomType.Entrance)
            {
                deleteRoom.childrenID.Clear();
                continue;
            }

            currentMap.roomDictionary.Remove(deleteRoom.id);
            currentMap.roomList.Remove(deleteRoom);

            DestroyImmediate(deleteRoom, true);

            AssetDatabase.SaveAssets();
        }

        currentMap.roomPositioned = false;

        UpdateDepth();
    }
    #endregion

    void ChangeRoomType(object roomType)
    {
        if ((int)currentNode.type > (int)RoomType.Visible)
        {
            Debug.LogWarning($"{EnumCaching.ToString(currentNode.type)}을 다른 타입으로 변경할 수 없습니다");
            return;
        }

        int type = (int)roomType;
        currentNode.type = (RoomType)type;
    }

    void ActiveGenerator(object position)
    {
        ActiveGenerator();
    }

    void ActiveGenerator()
    {
        showGenerator = true;
        generatorRect = new Rect(Vector2.zero, generatorSize);
    }
    #endregion

    #region Draw GUI
    #region Grid
    void CalculateGrid(float largeGridSize, float smallGridSize)
    {
        largeLineCount = new Vector2Int(Mathf.CeilToInt((position.width + largeGridSize) / largeGridSize), Mathf.CeilToInt((position.height + largeGridSize) / largeGridSize));
        smallLineCount = new Vector2Int(Mathf.CeilToInt((position.width + smallGridSize) / smallGridSize), Mathf.CeilToInt((position.height + smallGridSize) / smallGridSize));
/*
        graphOffset += graphDrag;

        largeGridOffset = new Vector3(graphOffset.x % largeGridSize, graphOffset.y % largeGridSize, 0f);
        smallGridOffset = new Vector3(graphOffset.x % smallGridSize, graphOffset.y % smallGridSize, 0f);*/
    }

    void DrawGrid(float largeGridSize, float smallGridSize)
    {
        if (draggingGraph)
        {
            graphOffset += graphDrag;

            largeGridOffset = new Vector3(graphOffset.x % largeGridSize, graphOffset.y % largeGridSize, 0f);
            smallGridOffset = new Vector3(graphOffset.x % smallGridSize, graphOffset.y % smallGridSize, 0f);
        }

        Handles.color = largeLineColor;
        for (int i = 0; i < largeLineCount.x; i++)
            Handles.DrawLine(new Vector3(largeGridSize * i, -largeGridSize, 0f) + largeGridOffset, new Vector3(largeGridSize * i, position.height + largeGridSize, 0f) + largeGridOffset);

        for (int j = 0; j < largeLineCount.y; j++)
            Handles.DrawLine(new Vector3(-largeGridSize, largeGridSize * j, 0f) + largeGridOffset, new Vector3(position.width + largeGridSize, largeGridSize * j, 0f) + largeGridOffset);


        Handles.color = smallLineColor;
        for (int i = 0; i < smallLineCount.x; i++)
            Handles.DrawLine(new Vector3(smallGridSize * i, -smallGridSize, 0f) + smallGridOffset, new Vector3(smallGridSize * i, position.height + smallGridSize, 0f) + smallGridOffset);

        for (int j = 0; j < smallLineCount.y; j++)
            Handles.DrawLine(new Vector3(-smallGridSize, smallGridSize * j, 0f) + smallGridOffset, new Vector3(position.width + smallGridSize, smallGridSize * j, 0f) + smallGridOffset);

        Handles.color = Color.white;
    }
    #endregion

    #region Node
    void DrawNodes()
    {
        foreach(DungeonRoomSO room in currentMap.roomList)
        {
            if (room.isSelected)
                room.Draw(selectedStyle);
            else
                room.Draw(defaultStyle);
        }
    }
    #endregion

    #region Line
    void DrawDraggingLine()
    {
        if (currentMap.from != null)
            Handles.DrawBezier(currentMap.from.rect.center, currentMap.to, currentMap.from.rect.center, currentMap.to, Color.white, null, 3.0f);
    }

    void DrawConnectionLine()
    {
        foreach(DungeonRoomSO room in currentMap.roomList)
        {
            if (room.childrenID.Count > 0)
            {
                foreach(string child in room.childrenID)
                {
                    if (currentMap.roomDictionary.ContainsKey(child))
                    {
                        DrawConnectionLine(room.rect.center, currentMap.roomDictionary[child].rect.center);
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
    }
    #endregion
    #endregion

    #region Graph Generator

    void DrawGenerator()
    {
        nodeRect = new Rect(Vector2.zero, nodeSize);
        GUILayout.BeginArea(generatorRect, generatorStyle);

        EditorGUILayout.LabelField("Graph Generator", EditorStyles.boldLabel);
        GUILayout.Box("", lineStyle, GUILayout.ExpandWidth(true), GUILayout.Height(1));

        maxAttempt = Mathf.Clamp(EditorGUILayout.IntField("Maximum Attempt", maxAttempt), 3, 255);
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

        if (GUILayout.Button("Init Values"))
        {
            InitGenerateGraphValues();
        }

        if (GUILayout.Button("Close"))
            showGenerator = false;


        GUILayout.EndArea();
    }

    void InitGenerateGraphValues()
    {
        maxAttempt = 10;
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
        if (currentMap.roomList.Count < 1)
            CreateRoomNode();

        AllSelect();
        currentMap.roomList[0].isSelected = false;
        currentMap.roomList[1].isSelected = false;

        DeleteSelectedRoomNodes();
        ResetSelect();

        roomCount = Random.Range(minRoom, maxRoom + 1);

        Queue<DungeonRoomSO> rooms = new Queue<DungeonRoomSO>();
        rooms.Enqueue(currentMap.roomList[0]);
        AutoGenerate(rooms);
        RoomReposition();

        int max = maxAttempt;
        rooms.Clear();
        int depth = currentMap.roomList[currentMap.roomList.Count - 1].depth;

        // RoomReposition에서 삭제된 노드만큼 추가
        while (currentMap.roomList.Count < roomCount)
        {
            if (--max < 0)
            {
                Debug.LogWarning("Over max attempt");
                break;
            }
            for (int i = currentMap.roomList.Count - 1; i > 0; i--)
            {
                if (depth - currentMap.roomList[i].depth > 4)
                    break;
                rooms.Enqueue(currentMap.roomList[i]);
            }
            AutoGenerate(rooms);
            RoomReposition();
        }

        ConnectBossRoom();

        AssetDatabase.SaveAssets();
    }

    void DeleteDisconnectRoom()
    {
        ResetSelect();
        foreach (DungeonRoomSO room in currentMap.roomList)
        {
            if (string.IsNullOrEmpty(room.parentID) && room.childrenID.Count < 1)
                room.isSelected = true;
        }
        DeleteSelectedRoomNodes();
    }

    RoomType GetRandomRoomType()
    {
        int probability = Random.Range(0, 100);
        int result;
        int[] percentage = { nonePercentage, smallPercentage, mediumPercentage, largePercentage, chestPercentage };

        for (result = 0; result < percentage.Length; result++)
        {
            if (probability < percentage[result])
                break;

            probability -= percentage[result];
        }

        return (RoomType)result;
    }

    void AutoGenerate(Queue<DungeonRoomSO> rooms)
    {
        currentMap.roomPositioned = false;
        while (rooms.Count > 0)
        {
            if (roomCount <= currentMap.roomList.Count)
            {
                break;
            }

            DungeonRoomSO current = rooms.Dequeue();

            if (CreateChild(current))
            {
                foreach (string childID in current.childrenID)
                    rooms.Enqueue(currentMap.GetRoomNode(childID));
            }
        }
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

                CreateRoom(type, room);
            }
        }

        if (room.childrenID.Count < 1)
            CreateRoom(RoomType.SmallRoom, room);
        return true;
    }

    void CreateRoom(RoomType roomType, DungeonRoomSO currentRoom)
    {

        DungeonRoomSO child = ScriptableObject.CreateInstance<DungeonRoomSO>();

        currentMap.roomList.Add(child);

        child.Initialise(nodeRect, roomType, currentMap);

        currentMap.AddLastRoomOfListToDictionary();

        if (currentRoom.AddChildID(child.id))
            child.AddParentID(currentRoom.id);

        AssetDatabase.AddObjectToAsset(child, currentMap);

        AssetDatabase.SaveAssets();
    }

    void ConnectBossRoom()
    {
        DeleteDisconnectRoom();
        for (int i = currentMap.roomList.Count - 1; i >= 0; i--)
        {
            if (currentMap.roomList[i].childrenID.Count >= 3)
                continue;
            DungeonRoomSO room = currentMap.roomList[i];

            if (room.AddChildID(currentMap.roomList[1].id))
            {
                currentMap.roomList[1].AddParentID(room.id);
                break;
            }
        }

        Vector2Int coordinate = currentMap.roomCoordinateClass.GetRoomCoordination(currentMap.roomList[1].id);
        currentMap.roomList[1].rect.position = new Vector2(nodeSize.x * coordinate.x + 50f, nodeSize.y * -coordinate.y + 50f) + center;
        GUI.changed = true;
        currentMap.roomPositioned = true;
    }
    #endregion

    #region Room Coordinate
    Vector2 center = Vector2.zero;
    void RepositionRoom()
    {
        if (currentMap.roomList.Count < 1)
            return;

        UpdateDepth();

        RoomReposition();

        if (!connectBossRoom)
            currentMap.roomList[1].rect.position = currentMap.roomList[0].rect.position + Vector2.one * 50f;

        AssetDatabase.SaveAssets();
    }

    void RoomReposition()
    {
        if (currentMap.roomPositioned)
        {
            MoveRoomNode();
            return;
        }

        ResetSelect();
        Queue<DungeonRoomSO> rooms = new Queue<DungeonRoomSO>();

        currentMap.roomCoordinateClass.Clear();
        rooms.Enqueue(currentMap.roomList[0]);

        while (rooms.Count > 0)
        {
            DungeonRoomSO current = rooms.Dequeue();
            foreach (string childID in current.childrenID)
            {
                if (currentMap.roomCoordinateClass.AddRoom(current.id, childID))
                    rooms.Enqueue(currentMap.GetRoomNode(childID));
                else
                    SelectChildren(childID);
            }
        }

        DeleteSelectedRoomNodes();

        DeleteDisconnectRoom();

        MoveRoomNode();

        AssetDatabase.SaveAssets();
    }

    void MoveRoomNode()
    {
        center = windowSize * 0.5f + Vector2.one * -50f;
        foreach (DungeonRoomSO room in currentMap.roomList)
        {
            Vector2Int coordinate = currentMap.roomCoordinateClass.GetRoomCoordination(room.id);
            room.rect.position = new Vector2(nodeSize.x * coordinate.x + 50f, nodeSize.y * -coordinate.y + 50f) + center;
        }
        currentMap.roomPositioned = true;
    }

    void SelectChildren(string room)
    {
        Queue<DungeonRoomSO> rooms = new Queue<DungeonRoomSO>();
        rooms.Enqueue(currentMap.GetRoomNode(room));
        while (rooms.Count > 0)
        {
            DungeonRoomSO current = rooms.Dequeue();
            currentMap.roomDictionary.Remove(current.id);
            current.isSelected = true;
            foreach (string childID in current.childrenID)
                rooms.Enqueue(currentMap.GetRoomNode(childID));
        }
    }
    #endregion
}
#endif