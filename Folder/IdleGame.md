### 개발 기간
> 2024. 5. 7. ~

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

### Scriptable Object로 캐릭터의 스탯 조절
- PlayerCharacterStatus/MonsterStatus
  > - 캐릭터들의 status의 정보를 가지는 클래스
  > - 원본 클래스를 복사한 클래스를 생성할 수 있는 생성자를 가짐
- GetCharacterStatus
  > - 매개변수로 입력한 타입의 캐릭터 status를 배열에서 찾아 반환하는 메소드
  > - class의 경우 참조변수이기 때문에 반환받은 status의 값을 그대로 사용할 경우 SO의 status 값도 변경되기 때문에 생성자로 새로운 클래스를 생성해 반환
- Status 배열
  > - 캐릭터마다 StatusSO를 만들지 않고, 하나의 SO에 모든 캐릭터의 Status를 관리하기 위해 배열을 사용
- OnValidate
  > - 휴먼 에러를 줄이기 위해 인스펙터의 값이 변경될 경우 Character의 타입이 None 또는 중복된 값이 있는지 검증
  > - 게임이 실행될 때는 검증할 필요가 없기 때문에 전처리기를 이용해 유니티 에디터에서만 실행

### 캐릭터
- 상속
  > - 게임의 확장성을 고려해 `Property > Class > Details` 형태의 상속 사용
- FSM
  > - FSM을 이용해 캐릭터 상태에 따른 행동을 하도록 함


2. 재화 업그레이드에 따른 플레이어 캐릭터 스탯 조절
4. 출석보상
5. AdMob
---
6. 오프라인 보상
7. Stage 티켓
