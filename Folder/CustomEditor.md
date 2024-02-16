## Dungeon Graph Editor
### Description
- `The Binding of Isaac: Rebirth`와 같은 스타일의 로그라이크류 게임에서 사용될 수 있는 던전(스테이지)을 제작할 수 있도록 도와주는 툴이다
- 개발자가 생성한 그래프대로 던전을 생성하기 때문에 다양한 난이도의 던전을 쉽게 제작할 수 있다
- 그래프의 자동 생성도 가능하며, 기획에 따라 게임 플레이 시 던전이 자동 생성되도록 할 수도 있다
- 던전 생성 시 던전의 방은 프리펩화된 게임 오브젝트를 이용하기 때문에 방은 랜덤하게 생성할 수 없지만, 던전 자체는 랜덤하게 생성 가능하다

### Dungeon Graph Menu
- 사용법
  > <details>
  > <summary>Details</summary>
  > 
  > - 빈 공간을 우클릭해 Graph Menu 활성화
  > - 노드를 우클릭해 Node Menu 활성화
  > - 선택되지 않은 노드를 클릭해 노드 선택
  > - 선택된 노드를 클릭해 선택 해제
  > - ESC키를 사용해 선택된 모든 노드 선택 해제
  > - 빈 공간을 드래그해 그래프를 이동
  > - 노드를 드래그해 노드 이동
  > - 노드를 마우스 오른쪽 버튼으로 드래그해 다른 노드와 연결
  > </details>
- Graph Menu
  > <details>
  > <summary>Graph Menu Image</summary>
  > 
  > ![Graph Menu Image](https://raw.githubusercontent.com/xcb00/Portfolios/main/Folder/Resources/DungeonGraphMenuImg1.png)
  > </details>
  > 
  > - Create Room Node
  >   > - Room Node를 생성
  >   > - 노드를 생성하고 [Ctrl + D]로 노드를 생성할 경우 선택한 노드와 동일한 타입의 노드가 마우스 커서 위치에 생성됨
  >   > - 여러 노드를 선택한 후 [Ctrl + D]로 노드를 생성할 경우 선택한 노드의 개수만큼 새로운 노드가 생성됨
  > - Validate Graph or Update Depth
  >   > - 입구와 보스룸이 연결되어있는지 검증
  >   > - 연결되지 않은 노드가 있는지 검증
  >   > - 검증에 오류가 없는 경우 입구가 윈도우의 중심에 오도록 이동
  >   > - 노드의 Depth값을 업데이트
  > - Select All Room Nodes
  >   > - 윈도우에 있는 모든 노드를 선택함
  > - Delete Selected Nodes
  >   > - 선택한 노드들을 삭제함
  >   > - 선택한 노드를 삭제할 때, 다른 노드들과의 연결을 해제함
  > - Disconnect Selected Nodes
  >   > - 선택한 노드들 사이의 연결을 해제함
  >   > - 선택한 노드들이 서로 연결되지 않았을 경우 아무런 일도 일어나지 않음
  > - Reposition Node
  >   > - 입구가 윈도우의 중심에 오도록 이동하며, 입구를 원점으로 노드들의 좌표에 해당하는 위치로 노드들을 이동
  > - Generate Graph  
  >   > <details>
  >   > <summary>Graph Generator Image</summary>
  >   > 
  >   > ![Graph Generator Image](https://raw.githubusercontent.com/xcb00/Portfolios/main/Folder/Resources/DungeonGraphMenuImg3.png)
  >   > </details>
  >   > 
  >   > - 설정한 노드의 개수만큼 노드를 생성하며, 방의 타입은 설정한 확률을 따름
  >   > - 설정한 확률의 합이 100이 아닐 경우 그래프를 생성하지 않고 콘솔창에 경고를 출력
- Node Menu
  > <details>
  > <summary>Node Menu Image</summary>
  > 
  > ![Node Menu Image](https://raw.githubusercontent.com/xcb00/Portfolios/main/Folder/Resources/DungeonGraphMenuImg2.png)
  > </details>
  > 
  > - Delete Node
  >   > 다른 노드들과의 연결을 해제한 후 삭제
  > - Disconnect
  >   > 다른 노드들과의 연결을 해제
  > - Change Type
  >   > 해당 노드의 타입을 다른 타입으로 변경

### Feature

<details>
<summary>휴먼 에러를 줄이기 위한 예외처리</summary>
  
  > - 입구 또는 보스룸을 생성하지 않는 것을 방지하기 위해 첫 번째 노드 생성 시 입구와 보스룸을 자동으로 생성됨
  > - 입구 또는 보스룸을 삭제하는 것을 방지하기 위해 입구와 보스룸이 생성되면 삭제되지 않음
  > - 입구와 보스룸이 연결되지 않는 것을 방지하기 위해 입구와 보스룸이 연결되지 않은 채 에디터창을 닫으면 에러가 출력됨
  > - 생성된 방 중 입구와 연결되지 못한 방이 있다면 경고가 출력됨
  > - 노드간 연결 시 제약조건을 주어 조건에 맞지 않으면 연결되지 않도록 함
  >   > <details>
  >   > <summary>제약조건</summary>
  >   > 
  >   > - 입구는 부모 노드를 가질 수 없음
  >   > - 보스룸은 자식 노드를 가질 수 없음
  >   > - 자식 노드는 중복될 수 없음(이미 자식 노드로 추가한 노드를 자식 노드로 추가할 수 없음)
  >   > - 부모 노드로 한 개의 노드만 가질 수 있음(이미 부모 노드가 있는 노드에 부모 노드를 추가할 수 없음)
  >   > - 자식 노드는 최대 3개 까지만 가능(다른 노드와 상하좌우로 연결될 수 있는 경우 4가지 중, 부모 노드와 연결이 때문에)
  >   > - 연결하려는 노드의 타입이 설정되지 않았을 경우
  >   > - 입구와 연결되지 않은 노드를 부모 노드로 연결할 경우
  >   > - 부모노드의 상하좌우에 다른 노드들이 있는 경우
  >   > - 노드의 깊이가 255를 넘을 경우
  >   > </details>
  > 
  > - 에디터 윈도우를 열 때, 에디터 창의 데이터를 저장하는 스크립터블 오브젝트가 없을 경우 자동 생성
  > - 스크립터블 오브젝트를 자동 생성 시 해당 경로에 해당하는 폴더가 없는 경우 재귀함수를 이용해 폴더 생성(폴더의 경로는 string[]으로 설정)

</details>  

### Script
<details>
<summary>EditorProcessBuilder.cs</summary>

> - Custom Window의 입력을 처리하기 위한 스크립트
> - 빌더 패턴을 사용해 Custom Window에서 사용할 입력을 등록
> - `Dictionary<InputEnum, Action>`을 이용해 키 입력 시 발생할 메소드 관리
>
> <details>
> <summary>How to use</summary>
> 
> ```C#
> EditorInputProcess inputEvent;
> void RegistInputEvent()
> {
>   EditorProcessBuilder builder = new EditorProcesesBuilder();
>   inputEvent = builder.Build();
>   builder.KeyboardEvent(MethodName, EditorKeyboardInput.value);
> }
> void InputProcess(Event _event)
> {
>   EditorKeyboardInput _input = GetKeyboardInput(_event); // Event의 input값에 따라 맞는 EditorKeyboardInput을 반환
>   inputEvent.KeyInput(_input);
> }
> void OnGUI()
> { InputProcess(Event.current); }
> ```
> </details>
> <details>
> <summary>class EditorInputProcess</summary>
> 
> ```C#
> // 휴먼 에러를 줄이기 위해 enum을 Dictionary의 키로 사용
> // 마우스 입력의 경우 상황에 따라 다른 메소드를 호출할 수도 있기 때문에 List로 같은 입력에 여러 메소드를 등록할 수 있도록 함
> public Dictionary<EditorMouseInput, List<Action<Event>>> MouseInputDic;
> public Dictionary<EditorKeyboardInput, Action> KeyboardInputDic;
> public EditorInputProcess()
> {
>    MouseInputDic = new Dictionary<EditorMouseInput, List<Action<Event>>>();
>    KeyboardInputDic = new Dictionary<EditorKeyboardInput, Action>();
> }
> 
> public void KeyInput(EditorKeyboardInput input)
> {
>   try{
>     if(input == EditorKeyboardInput.None) return;
>     // KeyboardInputDic에 input을 키로 가지는 값이 없다면 에러 메시지 출력
>     else if(!KeyboardInputDic.ContainsKey(input)) throw new Exception("KeyNotFoundException");
>     else KeyboardInputDic[input]?.Invoke();
>   } catch(Exception e) { Debug.LogError(e.Message); }
> }
> 
> public void MouseInput(Event _event, EditorMouseInput input, int index) ...
> ```
</details>
</details>

<details>
<summary>RoomCoordinateClass.cs</summary>

> - 에디터창에서 노드를 연결할 때, 노드의 ID와 좌표(coordination)를 저장하는 클래스
> - 게임 내에서 던전을 생성할 때, 좌표를 기반으로 던전이 생성 및 이동
>   > 입구를 원점(0, 0)으로, 플레이어가 위치한 방만 활성화 시킴
> - 노드 ID와 좌표를 한 쌍으로 가지지만, ID로 좌표를 찾거나 좌표로 ID를 찾는 경우 모두 발생하기 때문에 `Dictionary`를 사용하지 않고 `List<struct>`를 사용
>
> <details>
> <summary>struct RoomCoordination</summary>
> 
> ```C#
> public string id;
> public Vector2Int coordination;
> public RoomCoordination(string id, Vector2Int coordination)
> {
>   this.id = id;
>   this.coordination = coordination;
> }
> ```
> </details>
> 
> <details>
> <summary>class RoomCoordinateClass</summary>
> 
> ```C#
> // 노드를 연결할 때, 부모 노드를 가지고 있지 않은 경우 연결할 수 없도록 함
> // 입구는 Root 노드로, 부모 노드를 가질 수 없기 때문에 예외처리를 위해 입구 노드의 ID를 저장
> string entranceID = string.Empty;
> List<RoomCoordination> roomCoordinations;
> public RoomCoordinateClass
> public RoomCoordination(string entranceID)
> {
>   this.entranceID = entranceID;
>   Clear();
> }
> public void Clear()
> {
>   if(roomCoordinations == null) 
>     roomCoordinations = new List<RoomCoordination>();
>   else 
>     roomCoordinations.Clear();
>   roomCoordinations.Add(new RoomCoordination(entranceID, Vector2Int.zero));
> }
> public Vector2Int GetRoomCoordination(string id
> {
>   if(roomCoordinations.Count < 1)
>     return Vector2Int.zero;
>   foreach(RoomCoordination room in roomCoordinations)
>   {
>     if(room.id.Equals(id))
>       return room.coordinatinon;
>   }
>   return Vector2Int.zero;
> }
> public string GetRoomID(Vector2Int coordination)
> {
>   if(roomCoordinations.Count < 1)
>     return string.Empty;
>   foreach(RoomCoordination room in roomCoordinations)
>   {
>     if(room.coordination == coordination)
>       return room.id;
>   }
>   return string.Empty;
> }
> public bool ContainCoordination(Vector2Int coordination)
> {
>   foreach(RoomCoordination room in roomCoordinations)
>   {
>     if(room.coordination == coordination)
>       return true;
>   }
>   return false;
> }
> public bool ContainID(string id)
> {
>   foreach(RoomCoordination room in roomCoordinations)
>   {
>     if(room.id.Equals(id))
>       return true;
>   }
>   return false;
> }
> 
> public void Remove(string id)
> {
>   if(!graph.roomDictionary.ContainsKey(id)) return; // 이미 지웠다면 종료
>   Queue<string>rooms = new Queue<string>();
>   rooms.Enqueue(id);
>   
>   while(rooms.Count 〉0)
>   {
>     DungeonRoomSO current = graph.GetRoomNode(rooms.Dequeue());
>     
>     // 현재 노드와 부모 노드와의 연결 해제
>     graph.GetRoomNode(current.id).parentID = string.Empty;
> 
>     // 현재 노드가 자식 노드가 있다면 현재 노드와 연결된 모든 노드들의 연결 해제하기 위해 rooms에 추가
>     foreach(string childID in current.childrenID)
>       rooms.Enqueue(childID);
>     current.childrenID.Clear();
> 
>     // roomCoordinations에 current 삭제
>     int idx = -1;
>     for(int i = 0; i 〈 roomCoordinations.Count; i++)
>     {
>       if(roomCoordinations[i].id.Equals(current.id))
>       {
>         idx = i;
>         break;
>       }
>     }
>     if(idx 〉0)
>       roomCoordinations.RemoveAt(idx);
>   }
> }
> 
> public bool AddRoom(string currentID, string childID)
> {
>   Vector2Int current = GetRoomCoordination(currentID);
>   if(current == Vector2.zero && !entranceID.Equals(currentID))
>     return false; // 입구와 연결되지 않은 노드를 부모 노드로 가질 수 없음
>   int count = 0;
>   int startDir = Random.Range(0, 4); // 자식 노드의 좌표를 부여할 때, 규칙성을 없애기 위해 Random 사용
>   for(count = 0; count〈 4; count++)
>   {
>     if(!ContainCoordination(current + GetDirection(startDir + count)))
>       break;
>   }
>   if(count 〉3)
>     return false; // 현재 노드의 상하좌우에 이미 다른 노드가 있어 자식 노드를 추가할 수 없음
>   roomCoordinations.Add(new RoomCoordination(childID, current + GetDirection(startDir + count)));
>   return true;
> }
> Vector2Int GetDirection(int i) 
> {
>   switch (i % 4)
>   {
>     case 0: return Vector2Int.up;
>     case 1: return Vector2Int.right;
>     case 2: return Vector2Int.down;
>     case 3: return Vector2Int.left;
>     default: return Vector2Int.zero;
>   }
> }
> ```
> </details>
</details>

<details>
<summary>DungeonGraphEditor.cs</summary>

> - 커스텀 에디터 창에 그래프를 그리고 저장하는 스크립트
> - `OnGUI`에서 `GUILayout`을 그리고, `Event.current`를 이용해 입력을 처리
>
> 
> <details>
> <summary>Delete Node</summary>
> 
> ```C#
> void DeleteSelectedRoomNode()
> {
>   // 리스트를 반복문을 이용해 탐색할 때, 반복문 안에서 리스트의 값을 생성/삭제 시 오류가 발생할 수 있기 때문에 큐에 삭제할 노드를 추가한 후 반복문 종료 후 일괄삭제
>   Queue<DungeonRoomSO> delete = new Queue<DungeonRoomSO>();
>   foreach(DungeonRoomSO room in graph.roomList)
>   {
>     if (room.isSelected)
>     {
>       // 노드를 먼저 삭제하면 다른 노드들과의 연결을 처리하는 것이 어렵기 때문에 먼저 현재 노드와 연결된 노드들의 연결을 해제
>       if(!string.IsNullOrEmpty(room.parentID))
>         graph.DisconnectNode(room.id, room.parentID, false); // graph.DisconnectNode에서 RoomCoordinateClass의 Remove 실행
>       foreach(string childID in room.childrenID)
>         graph.DisconnectNode(room.id, childID, true); 
>       delete.Emqueue(room);
>     }
>   }
> 
>   while(delete.Count 〉0)
>   {
>     DungeonRoomSO deleteRoom = delete.Dequeue();
>     if(deleteRoom.roomType == RoomType.BossRoom) // 삭제하려는 노드가 보스룸일 경우 부모 노드의 ID를 삭제
>     {
>       deleteRoom.parentID = string.Empty;
>       continue;
>     }
>     else if(deleteRoom.roomType == RoomType.Entrance) // 삭제하려는 노드가 입구일 경우 자식 노드들을 삭제
>     {
>       deleteRoom.childrenID.Clear();
>       continue;
>     }
>     
>     graph.roomDictionary.Remove(deleteRoom.id);
>     graph.roomList.Remove(deleteRoom);
>     DestroyImmediate(deleteRoom, true);
>     AssetDatabase.SaveAssets();
>   }
>   graph.roomPositioned = false;
>   UpdateDepth();
> ```
> </details>
> 
> <details>
> <summary>Graph Generate</summary>
> 
> ```C#
> void GenerateGraph()
> {
>   if(graph.roomList.Count 〈 1) // 생성된 노드가 없다면 노드 생성(입구와 보스룸 노드 생성)
>     CreateRoomNode();
> 
>   // 입구와 보스룸 노드를 제외한 모든 노드를 삭제
>   AllSelect(); // graph.roomList에 있는 모든 DungeonRoomSO의 isSelected를 true로 변경
>   DeleteSelectedRoomNodes(); // 선택한 모든 노드 삭제(입구와 보스룸은 삭제되지 않음)
> 
> 
>   roomCount = Random.Range(minRoom, maxRoom + 1); // 생성할 방의 개수를 설정
>   Queue<DungeonRoomSO> rooms = new Queue<DungeonRoomSO>()
>   rooms.Enqueue(graph.roomList[0]);
>   AutoGenerate(rooms);
>   RoomReposition(); 
>   
>   // RoomReposition에서 상하좌우에 이미 노드가 있어 삭제된 노드의 개수만큼 추가
>   int max = maxAttempt; // Graph Generator에서 while문을 반복할 최대 횟수로, while문에서 무한반복 방지
>   rooms.Clear(); // while문 안에서 rooms를 클리어할 경우 새로 생성될 방들이 roomList의 뒷쪽에 있는 방들의 자식들로만 생성됨기 때문에 특정 노드의 깊이만 증가됨
>   int depth = graph.roomList[graph.roomList.Count - 1].depth;
>   while(graph.roomList.Count 〈 roomCount)
>   {
>     if(--max 〈 0) break; // 최대 반복 횟구에 도달하면 while문 종료
>     for(int i = graph.roomList.Count - 1; i 〉0; i--)
>     {
>       if(depth - graph.roomList[i].depth 〉4) break; // depth가 작을수록 상하좌우에 다른 노드들이 있을 경우가 크기 때문에 조건을 주어 성능을 최적화시킴
>       rooms.Enqueue(graph.roomList[i]);
>     }
>     AutoGenerate(rooms);
>     RoomReposition();
>   }
>   ConnectBossRoom();
> }
> 
> void AutoGenerate(Queue<DungeonRoomSO> rooms)
> {
>   graph.roomPositioned = false;
>   while(rooms.Count 〉0)
>   {
>     if(roomCount 〈= graph.roomList.Count) // 노드의 개수가 roomCount와 크거나 같다면 노드 생성을 종료
>       break;
>     DungeonRoomSO current = rooms.Dequeue();
>     if(CreateChild(current)) // 현재 노드에 자식 노드를 추가한 경우 true
>     {
>       foreach(string childID in current.childrenID)
>         rooms.Enqueue(graph.GetRoomNode(childID)); 
>     }
>   }
> }
> 
> bool CreateChild(DungeonRoomSO room)
> {
>   if(room.childrenID.Count > 0)
>     return false;
>   for(int i = 0; i 〈 3; i++)
>   {
>     RoomType type = GetRandomRoomType(); // GraphGenerator에서 설정한 RoomType 확률에 따라 랜덤하게 RoomType 반환
>     if(type == RoomType.None) break;
>     CreateRoom(type, room);
>   }
> 
>   if(room.childrenID.Count 〈 1) // GetRandomRoomType이 모두 RoomType.None을 반환해 자식을 추가하지 못한 경우 SmallRoom 타입을 자식으로 추가
>     CreateRoom(RoomType.SmallRoom, room);
>   return true;
> }
> 
> void CreateRoom(RoomType roomType, DungeonRoomSO currentRoom)
> {
>   DungeonRoomSO child = ScriptableObject.CreateInstance<DungeonRoomSO>(); // 스크립터블 오브젝트 생성
>   graph.roomList.Add(child);
>   // nodeRect : 에디터 창에 그릴 노드의 Rect 정보로, new Rect(Vector2.zero, nodeSize)
>   // graph : 에디터 창에 활성화되어있는 DungeonGraphSO
>   child.Initialise(nodeRect, roomType, graph);
>   
>   // graph.roomList에 추가한 DungeonRoomSO를 graph.roomDictionary에도 추가
>   // nodeID를 이용해 DugeonRoomSO에 쉽게 접근할 수 있도록 Dictionary<string, DungeonRoomSO>를 사용
>   graph.AddLastRoomOfListToDictionary(); 
>   
>   if(currentRoom.AddChildID(child.id))
>     child.AddParentID(currentRoom.id);
>   AssetDatabase.AddObjectToAsset(child, graph;
>   AssetDatabase.SaveAssets();
> }
> 
> void ConnectBossRoom()
> {
>   for(int i = graph.roomList.Count - 1; i 〉=0; i--)
>   {
>     if(graph.roomList[i].childrenID.Count 〉2) continue;
>     
>     DungeonRoomSO room = graph.roomList[i];
>     if(room.AddChildID(graph.roomList[1].id))
>     {  
>       graph.roomList[1].AddParentID(room.id);
>       break;
>     }
>   }
> 
>   Vector2Int coordinate = graph.roomCoordinateClass.GetRoomCoordinate(graph.roomList[1].id);
>   // center : 커스텀 윈도우의 중심
>   graph.roomList[1].rect.position = new Vector2(nodeSize.x * coordinate.x + 50f, nodeSize.y * -coordinate.y + 50f) + center; 
>   GUI.changed = true;
> }
> ```
> </details>
> 
> <details>
> <summary>Node Coordinate</summary>
> 
> ```C#
> void RepositionRoom()
> {
>   if(graph.roomList.Count 〈 1)
>     return;
>   UpdateDepth(); // Node의 Depth값을 갱신
>   RoomReposition();
>   
>   if(!connectBossRoom) // 보스룸이 입구와 연결되지 않았을 경우 보스룸을 입구의 위치로 이동
>     graph.roomList[1].rect.position = graph.roomList[0].rect.position + Vector2.one * 50f;
> }
> 
> void RoomReposition()
> {
>   // graph에 변화가 없는 경우 node들을 coordination에 맞춰 이동
>   if(graph.roomPositioned)
>   {
>     MoveRoomNode();
>     return;
>   }
> 
>   ResetSelect(); // 선택되어 있는 노드들을 선택 해제함
>   Queue<DungeonRoomSO> rooms = new Queue<DunteonRoomSO>();
>   graph.roomCoordinateClass.Clear();
>   
>   while(rooms.Count 〉0)
>   {
>     DungeonRoomSO current = rooms.Dequeue();
>     foreach(string childID in current.childrenID)
>     {
>       if(graph.roomCoordinateClass.AddRoom(current.id, childID)) // current 노드에 자식 노드를 추가할 수 있디면 추가
>         rooms.Enqueue(graph.GetRoomNode(childID));
>       else 
>         SelectChildren(childID); // current 노드에 자식 노드를 추가할 수 없다면 현재 노드와 연결된 모든 자식 노드를 삭제를 위해 isSelected를 true로 변경
>     }
>   }
>   DeleteSelectedRoomNodes();
>   DeleteDisconnectRoom(); // 연결되지 않은 모든 노드들을 삭제함
>   MoveRoomNode(); // 에디터 윈도우의 원점을 (0, 0)으로 node의 coordination에 따라 node를 이동
> }
> ```  
> </details>
</details>


### Dungeon Graph Editor를 이용해 던전 생성
[던전 생성](https://github.com/xcb00/Portfolios/blob/main/Folder/DungeonBuilder.md))


