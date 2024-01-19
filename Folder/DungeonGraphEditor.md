## Dungeon Graph Editor
### Description
- `The Binding of Isaac: Rebirth`와 같은 스타일의 로그라이크류 게임에서 사용될 수 있는 던전(스테이지)을 생성

### Functions
- Room Create
  > - `우클릭 > Create Room Node` 또는 `Ctrl + D`로 Room Node 생성
  > - 그래프의 Room Node가 없을 때 생성하면, 입구(Entrance)와 보스룸(Boss Room)이 자동으로 생성됨
  > - 이미 생성된 Room Node를 선택하고 `Ctrl + D`로 Room Node를 생성할 경우 선택된 Room Node의 타입과 동일한 Room Node가 생성
  > - 입구나 보스룸을 선택한 후 `Ctrl + D`를 할 경우 None 타입의 Room Node 생성
- Connect Room
  > - Room Node 2개를 마우스 오른쪽 버튼으로 드레그해 두 노드를 연결
  > - 노드를 연결할 때, 예외 검사를 해 노드가 잘못 연결되지 않도록 검증
  >   > <details>
  >   > <Summary>예외사항</Summary>
  >   > 
  >   > 1. 입구를 자식 노드로 추가할 경우(입구는 그래프의 Root로 설정)
  >   > 2. 보스룸에 자식 노드를 추가할 경우
  >   > 3. 부모 노드가 자식으로 추가하려는 노드를 이미 자식으로 가지고 있는 경우
  >   > 4. 부모 노드에 자식으로 추가하려는 노드가 이미 부모 노드를 가지고 있는 경우
  >   > 5. 부모 노드에 자식으로 추가하려는 노드의 Room Type이 설정되어있지 않은 경우
  >   > 6. 부모 노드에 자식으로 추가하려는 노드가 부모 노드에서 입구 노드까지 연결된 노드들 중 하나일 경우(순환형태 방지)
  >   > </details>
- Disconnect Room
  > - 연결을 끊을 노드들을 선택한 후 `우클릭 > Disconnect Selected Nodes` 또는 `Shift + E`로 연결 해제
  > - Room Node 위에서 `우클릭 > Disconnect`로 해당 노드의 부모와 자식 노드들의 연결 해제
- Room Delete
- Change Room Type
  > - Room Node 위에서 `우클릭 > Change Type > 바꿀 Room Type`으로 Room Node의 타입 변경
- Graph Generate
  > - Room의 최소/최대 갯수와, RoomType의 확률을 ㅇ닙력하면 Dugeon Graph를 생성

### Feature
- 커스텀 윈도우를 열 때, 해당 윈도우로 관리하는 스크립터블 오브젝트가 없을 경우 자동 생성
- 스크립터블 오브젝트를 자동 생성 시 해당 경로에 해당하는 폴더가 없는 경우 재귀함수를 이용해 폴더 생성(폴더의 경로는 string[]으로 설정)
- 스크립터블 오브젝트를 더블 클릭할 경우 커스텀 윈도우창 열기
- 이전에 작업중인 스크립터블 오브젝트가 있다면 커스텀 윈도우를 열 때, 해당 스크립터블 오브젝트를 불러옴
- 커스텀 윈도우가 활성화 시 스크립터블 오브젝트를 클릭하면 클릭한 스크립터블 오브젝트로 윈도우 갱신

### Logics
- 커스텀 윈도우에서 입력 처리
  > - 입력의 경우 빌더패턴과 Action을 사용해 구현
  > - 키 입력 시 실행할 메소드를 `Dictionary<Enum, Action>`을 이용해 저장
  > - 동적 생성과 쉬운 접근을 위해 Dictionary를 사용
  > - 마우스 입력의 경우 상황에 같은 입력이라도 상황에 따라 다른 이벤트가 발생될 수 있으므로 리스트를 사용
- 휴먼 에러를 줄이기 위해 `try catch`를 이용해 예외처리 및 에디터창에 예외사항 출력
- 유니티 에디터창에서만 실행되기 때문에 전처리기를 사용해 메모리 최적화
- 에디터창을 닫거나, 다른 DungeonGraph으로 변경할 때, 재귀함수를 이용해 보스룸(목적지)가 입구(출발지)와 연결되어있는지 검증

### Code
- [Eums](https://github.com/xcb00/Portfolios/tree/main/Folder/Scripts/Enums.cs)
- [EditorUtilities](https://github.com/xcb00/Portfolios/tree/main/Folder/Scripts/EditorUtilities.cs)
- [EditorProcessBuilder](https://github.com/xcb00/Portfolios/tree/main/Folder/Scripts/EditorProcessBuilder.cs)
- [DungeonGraphEditor](https://github.com/xcb00/Portfolios/tree/main/Folder/Scripts/DungeonGraphEditor.cs)
- [DungeonGraphSO](https://github.com/xcb00/Portfolios/tree/main/Folder/Scripts/DungeonGraphSO.cs)
- [DungeonRoomSO](https://github.com/xcb00/Portfolios/tree/main/Folder/Scripts/DungeonRoomSO.cs)
