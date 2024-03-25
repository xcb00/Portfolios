## DungeonBuilder
### Summary
게임에서 '아이작'과 같은 맵을 쉽게 생성하도록 도와주는 툴

### How to use
1. DungeonBuilder를 3D에서 사용할 경우 `CustomWindow > Misc`에서 `CustomCoordinateBrush.cs`를 삭제
   > `CustomCoordinateBrush`는 Tilemap에서 브러쉬 위치의 좌표를 출력해주는 브러쉬로, `RoomData2D`에 좌표값을 넣을 때 사용
2. `Create > Scriptable Objects > Dungeon Map`으로 `DungeonMap` Scriptable Object 생성
3. `Custom Editor > Dungeon Map Editor` 또는 생성한 `Dungeon Map Scriptable Object`를 더블 클릭으로 `Dungeon Map Editor`창 열기
4. `Dungeon Map Editor`에서 맵 생성하기 [Dungeon Map Editor Menual]()
5. `Create > Scriptable Objects > RoomData > Dungeon Room Data 2D/3D`로 `RoomData2D/3D` Scripatble Object를 생성
   > 게임에서 던전을 생성할 때 생성할 방의 정보를 가지는 Scriptable Object
   > <details>
   > <summary>Dungeon Room 2D</summary>
   > 
   > - Room Prefab : 게임에서 생성할 Room을 Prefab으로 만든 GameObject
   > - Room Type : Room Prefab의 타입
   > - Doorways : 다른 방으로 이동할 문의 정보를 가지는 `Room2DDoorClass`의 배열
   >   > - Orientation : 동서남북 중 문의 위치
   >   > - Width : 통로의 너비로, 다른 방과 연결되지 않은 통로를 벽으로 만들기 위해 사용
   >   > - Height : 통로의 높이로, 다른 방과 연결되지 않은 통로를 벽으로 만들기 위해 사용
   >   > - Door Position : 문이 생성될 위치
   >   > - Copy Position : 통로를 벽으로 만들기 위해 타일을 복사를 시작할 위치
   >   > - Door Prefab : 다른 방으로 이동하기 위한 문 오브젝트
   >   >   > 1. Door Prefab을 만들 때, `Door Collider`와 `Door Trigger`를 자식으로 생성한 후 `DoorCollider2D.cs`와 `DoorTrigger2D.cs`를 각각 추가
   >   >   > 2. Door Prefab에서 Collider 오브젝트의 Tag를 DoorCollider로 변경
   >   >   > 3. Door Prefab에서 Trigger 오브젝트의 Tag를 DoorTrigger로 변경
   >   >   > - DoorCollider : 문이 닫혀있을 때, 플레이어가 문을 지나가지 못하도록 하는 오브젝트
   >   >   > - DoorTrigger : 문이 열렸을 때, 플레이어가 다음 방으로 이동하는 메소드를 실행
   > - Spawn Position : 몬스터, 상자 등 오브젝트들이 생성될 위치
   > </details>
   > 
   > <details>
   > <summary>Dungeon Room 3D</summary>
   > 
   > - Room Prefab : 게임에서 생성할 Room을 Prefab으로 만든 GameObject
   > - Room Type : Room Prefab의 타입
   > - Doorways : 다른 방으로 이동할 문의 정보를 가지는 `Room3DDoorClass`의 배열
   >   > - Orientation : 동서남북 중 문의 위치
   >   > - Door Position : 문이 생성될 위치
   >   > - Door Prefab : 다른 방으로 이동하기 위한 문 오브젝트
   >   >   > 1. Door Prefab을 만들 때, `Door Collider`와 `Door Trigger`를 자식으로 생성한 후 `DoorCollider3D.cs`와 `DoorTrigger3D.cs`를 각각 추가
   >   >   > 2. Door Prefab에서 Collider 오브젝트의 Tag를 DoorCollider로 변경
   >   >   > 3. Door Prefab에서 Trigger 오브젝트의 Tag를 DoorTrigger로 변경
   >   >   > - DoorCollider : 문이 닫혀있을 때, 플레이어가 문을 지나가지 못하도록 하는 오브젝트
   >   >   > - DoorTrigger : 문이 열렸을 때, 플레이어가 다음 방으로 이동하는 메소드를 실행
   >   > - Wall Prefab : 다른 방과 연결되지 않은 방향에 문 대신 생성할 벽 오브젝트
   > - Spawn Position : 몬스터, 상자 등 오브젝트들이 생성될 위치
   > </details>
6. `Create > Scriptable Objects > Dungeon Level`로 `DugeonLevel` Scriptable Object 생성
   > <details>
   > <summary>Dungeon Level</summary>
   > 
   > - Room List : `RoomData`의 List로, 게임에서 방을 생성할 때, Room List에 있는 RoomData를 랜덤으로 선택해 생성
   >   > **Room List의 첫 번째 요소에 의해 2D/3D가 결정되며, 첫 번째 요소와 다른 차원의 RoomData를 넣을 경우 추가되지 않음**
   > - Dugeon Map List : `Dungeon Map`의 List로, 게임에서 던전을 생성할 때 랜덤으로 선택해 생성
   > </details>
7. `Hierarchy`에 빈 게임 오브젝트를 생성한 후 `DungeonBuilder.cs` 추가
   > <details>
   > <summary>Dungeon Builder</summary>
   > 
   > - `DungeonBuilder.Inst.GenerateDungeon(_dungeonLevel);`로 던전을 생성하며, DungeonBuilder에서 `GenerateDungeon`을 호출해 생성하기보다 `GameManager`와 같은 외부 스크립트를 이용해 생성하는 것을 지향함
   > - P2/P3 : 생성된 게임을 임시로 테스트하기 위한 플레이어 오브젝트로, 실제 게임을 제작할 때는 `Player` 스크립트를 생성해 사용하는 것을 지향함
   > - Tmp Level : DungeonBuilder에서 `GenerateDungeon`을 테스트하기 위한 임시 레벨
   > - PlayerCollider : DoorTrigger가 반응할 Layer의 종류로, Player의 `Collider` 오브젝트의 Layer와 동일하게 설정
   > - Minimap : `Hierarchy`에서 `Minimap.cs`를 가지고 있는 오브젝트로, Minimap을 관리하는 오브젝트
   > - Dungeons : 게임에서 생성할 `DungeonLevel` 리스트
   > </details>
8. `Hierarchy`에 빈 게임 오브젝트를 생성한 후 `Minimap.cs` 추가
      > <details>
      > <summary>Minimap</summary>
      > 
      > - `Minimap.cs`의 `ActiveMinimap`로 Minimap을 활성화하고, `InactiveMinimap`로 비활성화
      > - Minimap Room : Minimap에서 방를 나타낼 게임 오브젝트로, Layer를 `Minimap`으로 설정
      > - Minimap Doorway : Minimap에서 방과 방의 연결을 나타낼 게임 오브젝트로, Layer를 `Minimap`으로 설정
      > - Player Obj : Minimap에서 플레이어의 위치를 나타낼 게임 오브젝트로, Layer를 `Minimap`으로 설정
      > - Drag Speed : `MinimapUI`에서 드레그를 할 때, `MinimapCamera`를 움직이는 속도
      > - Pinch Speed : `MinimapUI`에서 Pinch를 할 때, `MinimapCamera`의 Size 증감 속도
      > - Pinch Range : `MinimapUI`에서 Pinch를 할 때, `MinimapCamera`의 Size의 범위
      > </details>
9. `Hierarchy`의 Minimap에 자식으로 MinimapCamera를 추가
      > <details>
      > <summary>Minimap Camera Setting</summary>
      > 
      > 1. `Clear Flags`를 `Solid Color`로 변경
      > 2. `Culling Mask`를 `Minimap`으로 변경
      > 3. `Projection`을 `Orthographic`으로 변경
      > 4. `Target Texture`를 `Minimap Texture`로 설정
      > </details>
10. `Hierarchy`에 MinimamCamera로 찍은 화면을 출력할 `MinimapUI` 추가
      > <details>
      > <summary>Minimap UI</summary>
      > 
      > 1. `Canvas`에 `RawImage`추가한 후 `Texture`에 `MinimapTexture` 설정
      > 2. `RawImage`에 `MinimapUI.cs` 추가
      >    > - `Drag Event`에 `Minimap` 오브젝트의 `Minimap.DragEvent` 추가
      >    > - `Pinch Event`에 `Minimap` 오브젝트의 `Minimap.PinchEvent` 추가
      > </details>
11. `DungeonBuilder`를 에디터에서 테스트할 경우 입력 키
      > - 숫자 1 : 문 열기(다음 방으로 이동할 수 있도록 함)
      > - 숫자 2 : 문 닫기
      > - R : 던전을 새로 만듦
12. 외부 스크립트에서 `DungeonBuilder`를 참조해 사용할 경우, `DungeonBuilder.cs`의 `Update`문을 삭제
