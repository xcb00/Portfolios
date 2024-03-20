## DungeonBuilder
### Summary
게임에서 '아이작'과 같은 맵을 쉽게 생성하도록 도와주는 툴

### How to use
1. `Create > Scriptable Objects > Dungeon Map`으로 `DungeonMap` Scriptable Object 생성
2. `Custom Editor > Dungeon Map Editor`또는 생성한 `Dungeon Map Scriptable Object`를 더블 클릭으로 `Dungeon Map Editor`창 열기 [Dungeon Map Editor Menual]()
3. `Create > Scriptable Objects > RoomData > Dungeon Room Data 2D/3D`로 `RoomData2D/3D` Scripatble Object를 생성
   > 게임에서 던전을 생성할 때 생성할 방의 정보를 가지는 Scriptable Object
   > <details>
   > <summary>Dungeon Room 2D</summary>
   > 
   > - Hello World
   > </details>
   > 
   > <details>
   > <summary>Dungeon Room 3D</summary>
   > 
   > - Room Prefab : 게임에서 생성할 Room을 Prefab으로 만든 GameObject
   > - Room Type : Room Prefab의 타입
   > - Doorways : 다른 방으로 이동할 문의 정보를 가지는 `Doorways Class`의 배열
   >   > - Orientation : 동서남북 중 문의 위치
   >   > - Door Position : 문이 생성될 위치
   >   > - Door Prefab : 다른 방으로 이동하기 위한 문 오브젝트
   >   > - Wall Prefab : 다른 방과 연결되지 않은 방향에 문 대신 생성할 벽 오브젝트
   > - Spawn Position : 몬스터, 상자 등 오브젝트들이 생성될 위치
   > </details>
4. `Create > Scriptable Objects > Dungeon Level`로 `DugeonLevel` Scriptable Object 생성
   > <details>
   > <summary>Dungeon Level</summary>
   > 
   > - Room List : `RoomData`의 List로, 게임에서 방을 생성할 때, Room List에 있는 RoomData를 랜덤으로 선택해 생성
   >   > **Room List의 첫 번째 요소에 의해 2D/3D가 결정되며, 첫 번째 요소와 다른 차원의 RoomData를 넣을 경우 추가되지 않음**
   > - Dugeon Map List : `Dungeon Map`의 List로, 게임에서 던전을 생성할 때 랜덤으로 선택해 생성
   > </details>
5. `Hierarchy`에 빈 게임 오브젝트를 생성한 후 `DungeonBuilder.cs` 추가
   > <details>
   > <summary>Dungeon Builder</summary>
   > 
   > - P2/P3 : 생성된 게임을 임시로 테스트하기 위한 플레이어 오브젝트로, 실제 게임을 제작할 때는 `Player` 스크립트를 생성해 사용하는 것을 지향함
   > - PlayerCollider : DoorTrigger가 반응할 Layer의 종류로, Player의 `Collider` 오브젝트의 Layer와 동일하게 설정
   > </details>
6. 1
7. 


1. LayerMask 설정(DungeonEditorEnum의 DungeonBuilderLayerMask 참조)
    > - User Layer 30 : PlayerCollider
    > - User Layer 31 : Minimap
2. Object의 Layer 설정
    > - Player의 Layer를 PlayerCollider로 변경(Player의 하위 항목 중 Collider가 있는 오브젝트의 Layer를 PlayerCollider로 변경)
    > - Minimap의 MinimapPlayer, MinimapRoom, MinimapDoorway의 Layer를 Minimap으로 변경
3. Minimap Camera의 Culling Mask를 Minimap으로 변경
4. DoorPrefab의 Tag 설정
    > - 2D 또는 3D Door Prefab에서 Collider 오브젝트의 Tag를 DoorCollider로 변경
    > - 2D 또는 3D Door Prefab에서 Trigger 오브젝트의 Tag를 DoorTrigger로 변경
5. Minimap에 MinimapPrefab 바인딩
6. 1

### 에디터 키
- 숫자 1 : 문 열기(다음 방으로 이동할 수 있도록 함)
- 숫자 2 : 문 닫기
- R : 던전을 새로 만듦
