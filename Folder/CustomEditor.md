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


### Dungeon Graph Editor를 이용해 던전 생성
[던전 생성]("www.naver.com")


