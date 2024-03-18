## DungeonBuilder

### To Do
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
