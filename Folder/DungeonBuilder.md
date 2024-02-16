## DungeonBuilder
### Description
- `DugeonGraphEditor`에서 생성한 그래프의 모양 대로 게임에서 방을 생성해 스테이지(던전)을 생성
  > <details>
  > <summary>DungeonGraph and Minimap Image</summary>
  > 
  > ![GraphAndMinimap](https://github.com/xcb00/Portfolios/blob/main/Folder/Resources/DungeonBuilder1.png)
  > </details>
- 플레이어가 있는 방만 활성화하고, 다른 방으로 이동 시 현재 방은 비활성화 후 다음 방 활성화
- 방 생성 시 미니맵도 같이 생성
- 미니맵에서 플레이어가 방문한 방만 활성화시키고, 현재 플레이어가 있는 방 표시
- `RoomDataSO`에 `DungeonBuilder`로 생성할 방의 정보를 저장
- 방이 연결되지 못한 통로가 있는 경우 통로를 벽으로 바꾸기 위한 정보를 `RoomDoorClass`를 이용해 저장
- 레벨에 따라 `DungeonLevelSO`에 해당 레벨에 해당 레벨에 생성할 방의 종류(`List<RoomDataSO>`)와 그래프의 종류('List<DungeonGraphSO>')를 저장

### Scripts
<details>
<summary>RoomDataSO.cs</summary>

> <details>
> <summary>class RoomDoorClass</summary>
> 
> ```C#
> public Orientation orientation; // 방에서 통로가 있는 위치로, Orientation은 north, east, south, west가 있음
> public int width; // 통로의 너비
> public int height; // 통로의 높이
> public Vector2Int doorPosition; // doorPrefab을 생성할 위치
> public Vector2Int copyPosition; // 통로를 없애기 위해 타일을 복사할 위치로, 통로의 좌상단에 위치
> public GameObject doorPrefab
> ```
> </details>
> 
> <details>
> <summary>class RoomDataSO</summary>
> 
> ```C#
> public string guid;
> public GameObject roomPrefab; // DungeonBuilder에서 생성할 방의 게임 오브젝트
> public RoomType roomType; // 방의 종류로, DungeonGraph의 노드와 같을 경우 랜덤하게 생성됨
> public List<RoomDoorClass> doorways; // 방의 통로의 정보를 저장하는 리스트
> public List<Vector2Int> spawnPosition; // 플레이어를 제외한 오브젝트(적, 아이템 등)를 생성할 위치
> ```
> </details>
</details>

<details>
<summary>DungeonBuilder.cs</summary>

> <details>
> <summary>class DungeonBuilder</summary>
> 
> ```C#
> List<RoomDataSO>[] roomTypeArray = new List<RoomDataSO>[6]; // DungeonLevelSO에 저장된 RoomDataSO 중에 같은 타입이 있을 경우 랜덤하게 생성하게 생성될 수 있도록, RoomDataSO를 RoomType에 따라 저장
> Dictionary<Vector2Int, InstantRoom> roomObjectDictionary; // 방의 Coordination을 키로, 생성된 방의 정보를 저장하는 Dictionary
> [SerialiseField] Minimap minimap; // 방 생성 시 미니맵도 생성할 때 사용
> public bool canChangeRoom = false; // 방을 이동할 수 있는지 확인하는 값으로, 특정 조건을 만족할 때, true로 변경
> 
> public void GenerateDungeon(DungeonLevelSO dungeonLevel)
> {
>   ClearDungeon(); // 생성했던 방을 모두 삭제한 후, roomObjectDictionary 초기화
>   InitRoomTypeArray(dungeonLevel.roomList, dungeonLevel.name); // 새로운 dungeonLevel에 맞춰 roomTypeArray를 새롭게 갱신
>   InstantiateRoom(dungeonLevel.dungeonGraphList[Random.Range(0, dungeonLevel.dungeonGraphList.Count)]); // dungeonLvel이 가지고 있는 dungeonGraph 중 하나를 랜덤하게 생성
>   ClearRoomTypeArray(); // 던전 생성이 완료된 경우 메모리 최적화를 위해 roomTypeArray를 초기화해 GC가 메모리를 해제할 수 있도록 함
> }
>   
> void InitRoomTypeArray(List<RoomDataSO> roomList, string dungeonLevelName)
> {
>   for(int i = 0; i 〈 roomTypeArray.Length; i++)
>     roomTypeArray[i] = new List<RoomDataSO>();
>   foreach(RoomDataSO roomData in roomList)
>   {
>     int idx = RoomTypeToIndex(roomData.roomType); // RoomType의 값을 int값으로 변환 - RoomType의 값이 연속된 정수값이 아니기 때문에 변환해줌
>     if(idx 〈 0) continue; //roomData의 roomType이 잘못 설정됨 - none 또는 count로 설정되어있음
>     roomTypeArray[idx].Add(roomData);
>   }
> }
> 
> void InstantiateRoom(DungeonGraphSO dungeonGraph)
> {
>   roomObjectDictionary = new Dictionary<Vector2Int, InstantRoom>();
>   Queue<string> roomIDs = new Queue<string>();
>   roomIDs.Enqueue(dungeonGraph.roomCoordinateClass.GetRoomID(Vector2Int.zero));
>   
>   // dungeonGraph에 저장되어 있는 노드의 정보에 맞게 방을 생성
>   while(roomIDs.Count 〉0)
>   {
>     DungeonRoomSO room = dungeonGraph.GetRoomNode(roomIDs.Dequeue());
>     Vector2Int currentCoordinate = dungeonGraph.roomCoordinateClass.GetRoomCoordination(room.id);
>     
>     int typeIndex = RoomTypeToIndex(roomType);
>     if(typeIndex 〈 0) return;
>     
>     int roomIndex = Random.Range(0, roomTypeArray[typeIndex].Count);
>     GameObject instantObject = Instantiate(roomTypeArray[typeIndex][roomIndex].roomPrefab, Vector3.zero, Quaternion.identity, transform);
>     InstantRoom instantRoom = instantObject.GetComponentInChildren<InstantRoom>();
>     
>     // 연결된 노드가 있다면 정보를 Minimap과 InstantRoom에 전달
>     byte _doorway = 0;>     
>     if(!string.IsNullOrEmpty(room.parentID))
>     {
>       Orientation parentOrientation = dungeonGraph.roomCoordinateClass.GetDoorwayOrientation(room.id, room.parentID);
>       _doorway = DoorwayToByte(_doorway, parentOrientation); // 미니맵에 연결된 노드들의 방향을 byte를 이용해 전달
>       instantRoom.DoorwayConnect(parentOrientation); // 생성된 게임 오브젝트에 부모 노드와 연결된 방향을 전달
>     }
>     foreach(string childID in room.childrenID)
>     {
>       roomIDs.Enqueue(childID);
>       Oreintation childOrientation = dungeonGraph.roomCoordinateClass.GetDoorwayOrientation(room.id, childID);
>       _doorway = DoorwayToType(_doorway, childOrientation);
>       iinstantRoom.DoorwayConnect(childOrientation);
>     }
>     
>     minimap.AddMinimapDictionary(currentCoordinate, _doorway); // Minimap에 방의 위치와 연결된 통로의 정보를 전달
>     instantRoom.Initialise(roomTypeArray[typeIndex][roomIndex].doorways, roomTypeArray[typeIndex][roomIndex].spawnPosition);
>     instantRoom.DisableRoom();
>     roomObjectDictionary.Add(currentCoordinate, instantRoom);
>   }
> }
> public Vector3 ChangeRoom(Vector2Int currentCoordinate, Vector2Int nextCoordinate, Orientation orientation = Orientation.none)
> {
>   roomObjectDictionary[currentCoordinate].DisableRoom();
>   roomObjectDictionary[nextCoordinate].EnableRoom();
>   canChangeRoom = minimap.MoveMinimapPlayer(nextCoordinate); // 미니맵의 플레이어 표시를 다음 방으로 이동하고, 미니맵이 이미 활성화되어 있는 경우(이전에 방문한 적이 있는 경우) true 반환
>   Vector2Int nextPosition = roomObjecDictionary[nextCoordinate].GetSpawnPosition(orientation); // 플레이어의 위치를 현재 방과 연결된 통로의 문 앞 1칸으로 이동
>   return new Vector3(nextPosition.x, nextPosition.y, 0f); // ChangeRoom은 Player의 OnTriggerEnter에서 호출하고, 플레이어의 위치를 반환받은 Vector3값으로 이동
> }
> ```
> </details>
</details>

<details>
<summary>InstantRoom.cs</summary>

> <details>
> <summary>class InstantRoom</summary>
> 
> ```C#
> public void Initialise(List<RoomDoorClass> doorwayList, List<Vector2Int> spawnPosition)
> {
>   this.doorway = doorwayList;
>   this.spawnPosition = spawnPosition;
>   BindingTilemap(); // 연결되지 않은 통로를 벽으로 만들기 위해 Room Prefab의 Tilemap을 변수로 저장
> }
> 
> void BlockOffUnusedDoorways(); // EnableDoor()에서 실행
> {
>   for(int i = 0; i 〈 doorwayOrientations.Length; i++)
>   {
>     RoomDoorClass doorData = doorwayList.Find(x =〉x.orientation == (Orientation)i);
>     if(doorData == null) return;
>     if(doorwayOrientations[i]) // (Orientation)i 방향의 통로가 연결되어 있는 경우 문 생성
>     {
>       SpawnDoorPrefab();
>       continue;
>     }
>     
>     if(_Tileamp != null) BleckDoorwayOnTileampLayer(_Tilemap, doorData) // Room Prefab의 Tileamp 개수만큼 반복
>   }
>   alreadyDoorwayBlock = true; // 방이 다시 활성화될 때, BlockOffUnusedDoorways를 다시 실행하지 않도록 true로 변경
> }
> 
> void BlockDoorwayOnTilemapLayer(Tilemap tilemap, RoomDoorClass doorData)
> {
>   switch(doorData.orientation)
>   {
>     case Orientation.north:
>     case Orientation.south:
>       BlockDoorwayHorizontally(tilemap, doorData);
>       break;
>     case Orientation.east:
>     case Orientation.west:
>       BlockDoorwayVertically(tilemap, doorData);
>       break;
>     default:
>     case Orientation.south:
>       break;
>   }
> }
>  
> // BlockDoorwayVertically로 BlockDoorwayHorizontally와 유사
> void BlockDoorwayHorizontally(Tilemap tilemap, RoomDoorClass doorClass)
> {
>   Vector2Int startPosition = doorData.copyPosition;
>   for(int x = 0 〈 doorData.width; x++)
>   {
>     for(int y = 0; y 〈 doorData.height; y++)
>     {
>       Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + x, startPosition.y - y, 0));
>       tilemap.SetTile(new Vector3Int(startPosition.x + 1 + x, startPosition.y - y, 0), // 복사한 타일을 놓을 위치
>           tilemap.GetTile(new Vector3Int(startPosition.x + x, startPosition.y - y, 0))); // 가져올 타일
>       tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + 1 + x, startPosition.y - y, 0), transformMatrix);
>     }
>   }
> }
> 
> public Vector2Int GetSpawnPosition(Orientation orientation)
> {
>   if(orientation == Orientation.none) return spawnPosition[Random.Range[0, spawnPosition.Count)];
>   
>   orientation = (Orientation)(((int)orientation + 2) % 4); // orienation을 전달받은 방향의 반대로 변경(현재 방에서 남쪽으로 이동했다면 다음 방의 북쪽에 플레이어가 위치하게 됨)
>   Vector2Int adjust = GameManager.Inst.OrientationToVctor2Int(orientation); // 문의 위치에서 한 칸 앞으로 이동하기 위한 조정값
>   return doorwayList.Find(x =〉x.orientation == orientation).doorPosition - adjust;
> }
> ```
> </details>
</details>

