##### 2024. 03. 04.

### 목표
1. 조이스틱으로 플레이어를 움직일 수 있도록 함
2. 화면 드레그를 이용해 SpringArm 기능 구현
3.  DataManager를 이용해 JSON으로 데이터를 저장하도록 함

### 완료
- GameManager에서 FSM을 이용해 게임의 흐름을 제어하도록 함
- `Input.GetAxis`를 이용해 입력이 있는 경우 캐릭터가 움직이도록 함
- 다양한 모바일 해상도에 대응하기 위해 고정 해상도 사용
  > - 디바이스의 해상도가 설정한 해상도와 맞지 않는 경우 검은색 레터박스 생성
  > - 카메라 해상도에 맞춰 캔버스의 크기 조절
  > <details>
  > <summary>Show Deatils</summary>  
  > 
  > > - 빨간색 : CanvasRatio를 사용한 캔버스
  > > - 노란색 : CanvasRatio를 사용하지 않은 캔버스
  > >   <details>
  > >   <summary>Camera/Canvas Ratio 적용 전</summary>
  > >   
  > >   ![Camera/Canvas Ratio 적용 전](https://github.com/xcb00/Portfolios/blob/main/Folder/Resources/Ratio1.png)
  > >   </details>
  > > 
  > >   <details>
  > >   <summary>설정한 해상도보다 가로가 길 경우</summary>
  > >   
  > >   ![설정한 해상도보다 가로가 길 경우](https://github.com/xcb00/Portfolios/blob/main/Folder/Resources/Ratio2.png)
  > >   </details>
  > > 
  > >   <details>
  > >   <summary>설정한 해상도보다 세로가 길 경우</summary>
  > >   
  > >   ![설정한 해상도보다 세로가 길 경우](https://github.com/xcb00/Portfolios/blob/main/Folder/Resources/Ratio3.png)
  > >   </details>
  > > 
  > </details>

##### 2024. 03. 05.

### 목표
1. 조이스틱으로 플레이어를 움직일 수 있도록 함
2. 화면 드레그를 이용해 SpringArm 기능 구현
3.  DataManager를 이용해 JSON으로 데이터를 저장하도록 함

### 완료
- 가상 조이스틱 구현 및 조이스틱으로 플레이어 이동
- 화면 드레그로 x, y축 회전 구현


