# 기획
## 특징
- Custom Window를 이용해 레벨에 따라 던전(맵)을 무작위로 생성함
- TPS로, 방의 몬스터를 모두 죽이면 다음 방으로 이동 가능
- 던전은 직선의 형태가 아닌 트리의 형태로, 플레이어가 보스룸을 찾아야 함
## 밴치마킹
### 궁수의 전설
> - 던전을 제외한 UI/UX 부분을 밴치마킹해 제작
### 아이작
> - 던전 관련 부분을 밴치마킹해 제작
### 모바일 배그
> - 캐릭터 조작 방법을 밴치마킹해 제작
---
# 제작
## 스크립트
<details>
<summary>Show Scripts</summary>

> - Camera
>   > - SpringArm : 카메라의 Rotation/Zoom을 구현하는 스크립트
> - Manager
>   > - Settings : 게임의 설정참에서 값을 조절할 수 있는 변수들을 관리하는 스크립트
>   > - DataManager : 게임의 데이터를 관리하는 스크립트로, JSON을 이용해 데이터를 관리
>   > - GameManager : 전반적인 게임을 관리하는 스크립트로, 유일하게 Update문을 가지고 있음
> - Misc
>   > - EventHandler : 옵저버 패턴과 같이 특정 이벤트가 발생될 때 실행할 함수(메소드)를 관리하는 스크립트
>   > - Singleton : 싱글톤 패턴을 사용하기 위한 스크립트
> - 1
> - 2
> - 3
> - 
</details>


## 스크립트
<details>
<summary>Dev Log</summary>

> - [1일차](https://github.com/xcb00/Portfolios/blob/main/Folder/DevLog/ExitDungeon_01.md)
</details>






# 메모
- 카메라 시점 이동 : 화면을 드레그해 이동(플레이어는 드레그하는 대로 y축으로 회전) // 배그에서도 케릭터를 회전시킴
- JSON을 이용해 데이터 저장 


