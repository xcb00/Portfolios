### 게임 방법
> - Stage
>   > - 캐릭터 카드를 드래그 해 슬롯에 배치
>   > - 4장의 카드를 모두 배치할 경우 Start 버튼이 활성화되며, Start 버튼으로 스테이지 실행
>   > - 캐릭터 카드를 재배치할 경우 Reset 버튼으로 초기화
> - Upgrade
>   > - 스테이지와 오프라인 보상으로 획득한 골드를 이용해 캐릭터 업그레이드 가능
> - Offline
>   > - 게임을 종료한 시점부터 다시 접속한 시간 만큼 골드 획득 가능

### 사용자가 캐릭터의 배치를 설정할 수 있도록 함
- CharacterCard
  > - EventSystems을 사용해 사용자가 드래그 해 슬롯에 캐릭터를 배치할 수 있도록 함
  > - `OnBeginDrag` : 드레그 하고 있는 카드 UI가 ray에 충돌하지 않도록 CanvasGroup의 blocksRaycasts를 false로 변경
  > - `OnDrag` : 사용자가 드레그하는 위치로 카드 UI를 이동
  > - `OnEndDrag` : 카드 UI를 원래 위치로 이동시키고, 카드 UI를 다시 드레그 할 수 있도록 CanvasGroup의 blockRaycasts를 true로 변경
- CharacterCardSlot
  > - 사용자가 캐릭터를 재배치하기 위해 Reset을 할 경우 비활성화 되어있는 캐릭터카드를 활성화시킴
  > - EventHandler의 ResetCharacterSlotEvent에 ResetCard를 추가해 사용자가 Reset할 경우 ResetCard를 실행
- CharacterSlot
  > - EventSystems을 사용해 드래그 중인 카드의 정보를 가져오도록 함
  > - `OnDrop` : 현재 슬롯이 비어있을 경우 드레그 중인 카드의 캐릭터 이미지를 슬롯의 캐릭터 이미지로 변경하고, 카드를 비활성화
  > - EventHandler의 ResetCharacterSlotEvent에 ResetSlot을 추가해 사용자가 Reset할 경우 ResetSlot을 실행

### 고정 해상도
- CamRatio
  > - 카메라의 ViewportRect의 값을 조절해 어떠한 디바이스에서 플레이하더라도 9:16 비율의 해상도를 가지도록 함
  > - `dragAdjust` : 화면에 해상도에 맞춰 카메라의 비율을 조정하면서 드래그 포인터와 카드 이미지의 오차를 없애기 위해 화면을 줄이는 비율의 역수만큼 delta에 곱해서 사용
  > - Render Mode가 `Screen Space - Overlay`인 캔버스를 9:16 비율에 맞도록 Scaler의 Offset을 조절
  > - LetterBox : ViewportRect로 생긴 레터박스 위에 UI가 그려지면 지워지지 않고 남기 때문에 이를 방지하기 위해 이미지로 레터박스를 새로 생성

### Scriptable Object로 게임의 데이터 관리
- CharacterSO
  > - PlayerCharacterStatus/MonsterStatus
  >   > - 캐릭터들의 status의 정보를 가지는 클래스
  >   > - 원본 클래스를 복사한 클래스를 생성할 수 있는 생성자를 가짐
  > - GetCharacterStatus
  >   > - 매개변수로 입력한 타입의 캐릭터 status를 배열에서 찾아 반환하는 메소드
  >   > - class의 경우 참조변수이기 때문에 반환받은 status의 값을 그대로 사용할 경우 SO의 status 값도 변경되기 때문에 생성자로 새로운 클래스를 생성해 반환
  > - Status 배열
  >   > - 캐릭터마다 StatusSO를 만들지 않고, 하나의 SO에 모든 캐릭터의 Status를 관리하기 위해 배열을 사용
  > - OnValidate
  >   > - 휴먼 에러를 줄이기 위해 인스펙터의 값이 변경될 경우 Character의 타입이 None 또는 중복된 값이 있는지 검증
  >   > - 게임이 실행될 때는 검증할 필요가 없기 때문에 전처리기를 이용해 유니티 에디터에서만 실행
- StageSO
  > - StageInfo
  >   > - 스테이지의 정보를 가지는 클래스
  >   > - 원본 클래스를 복사한 클래스를 생성할 수 있는 생성자를 가짐
  > - GetStageInfo
  >   > - 매개변수로 받은 스테이지의 정보를 새로 생성해 반환하는 메소드
  > - OnValidate
  >   > - 휴먼 에러를 줄이기 위해 보스 몬스터와 스테이지 몬스터의 값 검증
  > - 확장성 고려
  >   > - 스테이지 생성되는 몬스터의 종류를 배열로 추가해, 몬스터의 종류가 증가할 때 쉽게 스테이지 데이터를 변경할 수 있도록 함
  >   > - 스테이지의 보스 몬스터로 생성될 몬스터의 종류를 따로 저장해, 몬스터의 종류가 증가할 때 쉽게 스테이지 데이터를 변경할 수 있도록 함

### 캐릭터
- 상속
  > - 게임의 확장성을 고려해 `Property > Class > Details` 형태의 상속 사용
  >   > - 플레이어 캐릭터와 몬스터의 공격 메소드만 재정의해 사용
  > - 애니메이터 오버라이드 컨드롤러를 이용
- FSM
  > - FSM을 이용해 캐릭터 상태에 따른 행동을 하도록 함
  >   > - 플레이어 캐릭터
  >   >   > - Idle : 현재 스테이지의 몬스터가 없는 경우 몬스터가 생성될 때 까지 대기
  >   >   > - Follow : 스테이지에 몬스터가 있는 경우 타겟 몬스터의 방향으로 이동
  >   >   > - Attack : 몬스터가 공격 범위 내에 있는 경우 공격
  >   > - 몬스터
  >   >   > - Idle : 플레이어 캐릭터가 감지 범위 밖에 있는 경우 대기
  >   >   > - Move : 플레이어 캐릭터가 감지 범위 밖에 있는 경우 랜덤한 방향으로 이동
  >   >   > - Follow : 플레이어 캐릭터가 감지 범위 내에 있는 경우 플레이어 캐릭터 방향으로 이동
  >   >   > - Attack : 플레이어 캐릭터가 공격 범위 내에 있는 경우 공격
  >   >   > - 플레이어 캐릭터가 감지 범위 내에 없는 경우 Idle과 Move를 반복해 패트롤 구현
- 애니메이션 이벤트
  > - 애니메이션 이벤트를 이용해 Attack/Die 메소드 실행

### 캐릭터 업그레이드
- 스테이지와 오프라인 보상으로 획득한 골드한 골드를 이용해 캐릭터 스탯 증가
- 업그레이드 레벨 * 10% 만큼 스탯 증가

### 캐릭터 정보 UI 구현
- 캐릭터의 HP와 레벨을 나타내는 UI 구현
- 캐릭터가 움직일 경우 캐릭터 정보 UI도 캐릭터를 따라 이동

### 스테이지 구현
- StageSO의 StageInfo의 개수 만큼 Stage 실행
- StageInfo의 TotalMoster만큼 몬스터를 사냥하면 보스 몬스터 생성
- 보스 몬스터를 사냥하면 다음 스테이지로 이동
- 스테이지를 모두 클리어 하거나 스테이지 포기 및 캐릭터가 모두 죽었을 경우 `사냥한 몬스터 수 * 100 + 스테이지 클리어 보상`만큼 골드 획득

### 스테이지 내 캐릭터 레벨 구현
- `PlayerCharacterStatus`의 exp만큼 몬스터를 사냥할 경우 레벨을 증가
- 레벨에 따라 exp를 증가
- `레벨 * 10%`만큼 캐릭터의 스탯 증가

### JSON 형식으로 게임 데이터 저장
- `class PlayerData`
  > - Gold : 현재 골드 량
  > - ExitTime : 게임을 종료한 시간으로, OnApplicationPause가 실행될 때의 시간을 저장
  > - Upgrades : 캐릭터 업그레이드 값으로, UpgradeValue Enum값을 List의 인덱스로 사용
- 플레이어 데이터 클래스를 JSON 형식으로 저장
- 로컬에 저장된 플레이어 데이터에 접근할 수 없도록 암호화해 저장

### 오프라인 보상
- 게임을 처음 실행할 때, 서버의 시간을 가져옴
- 서버의 시간과 PlayerData의 ExitTime의 차이를 구해 오프라인 보상 지급

### 무한 타일맵
- 타일맵이 카메라로부터 일정 범위 밖인 경우 타일맵을 재배치

### 게임 UI 관리
- UIManager로 게임의 UI들을 관리
- `UIDictionary`로 인스펙터 창에서 UIType과 CanvasGroup을 바인딩해 관리
- `EventHandler.CallActiveUIPanelEvent(UIType type, bool active)`로, UI관리

### Fade
- `async`를 이용해 Fade 이벤트 처리
- `EventHandler.CallFadeInEvent` 또는 `EventHandler.CallFadeOutEvent`로 Fade 이벤트 실행

### 향후 개발 방향
1. 플레이어 캐릭터에서 타겟을 지정하는 알고리즘 보완하기
   > - 캐릭터 피격 시 타겟 변경
   > - 타겟보다 다른 몬스터가 가까이 있는 경우 타겟 변경
2. 플레이어 캐릭터끼리 겹쳐지지 않도록 하기
3. Google Play Game Service를 이용해 플레이어 데이터를 로컬이 아닌 서버에 저장하기
4. Google AdMob을 이용해 게임에 광고 넣기
5. Stage 입장에 필요한 티켓 구현하기
6. 캐릭터 카드 뽑기를 이용해 캐릭터 강화 구현하기
7. 출석 보상 구현하기
