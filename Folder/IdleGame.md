## 필수 구현 항목

## 추가 구현 항목
### 사용자가 캐릭터의 배치를 설정할 수 있또록 함
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

- JSON으로 플레이어 데이터 저장
- 캐릭터 뽑기
- 환생
- 광고
- 광고버프
- 보스 클리어 시 진동
- BGM / 사운드 이펙트
- 출석보상
- 슬롯에 따른 캐릭터 버프

- 사용자가 캐릭터의 배치를 선택할 수 있도록 함
