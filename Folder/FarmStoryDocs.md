## 객체지향 프로그래밍
> - 상속을 이용해 코드를 재사용
>   > <details>
>   > <summary>객체구조</summary>
>   >  
>   > ![Pics](https://github.com/xcb00/Portfolios/blob/main/Folder/Resources/FarmstoyHierarchy.png)
>   > </details>
> - 델리게이트를 이용
>   > - EventHandler
>   > - UnityAction/Event

## Network
> - Google SpreadSheet를 DB로 이용
>   > - `UnityEngine.Networking`의 `UnityWebRequest.Post`를 이용해 서버의 데이터를 받아오도록 제작함
>   > - 서버의 데이터를 `JSON`형식을 이용해 서버의 데이터를 받아오도록 제작함
>   > - 아이템의 정보와 같이 변동성이 큰 데이터들을 구글 스프레드시트를 이용해 데이터가 변경될 때마다 빌드하지 않도록 제작함
>   > - 게임을 실행할 때마다 서버에서 데이터를 가져올 경우 로딩하는 시간이 길어 서버에서 변경된 데이터만 새롭게 받을 수 있도록 제작함
> - Network를 이용한 출석체크

## SDK 이용
> - GPGS
> - Googld Admob
> - IAP

## 기타
> - FSM을 이용해 AI 구현
> - AStar를 이용해 AI가 플레이어를 따라가도록 구현
> - 농작물 성장 알고리즘
>   > - 처음 개발할 때는 하루가 지나면 저장되어 있는 모든 농작물의 성장일을 1씩 증가하는 방향으로 개발함
>   > - 코드 최적화를 위해 농장을 방문했을 때, 이전의 날짜와 현재 날짜의 차를 성장일에 더하는 방향으로 변경함
>   > - 농작물 성장 처리 순서
>   >   > <details>
>   >   > <summary>Show Details</summary>
>   >   > 
>   >   > 1. 새로운 씬을 로드할 때, MapTileManager.InitMap을 이용해 로컬에 저장된 CropTile의 정보를 가져옴
>   >   > 2. MapTileManager.LoadTile을 통해 씬에 CropTile 정보가 존재하는 경우 날짜 증가 처리를 함
>   >   > 3. CropManager.LoadCrop에서 로컬에 저장된 CropPrefab 정보 중 MapTileManager.LoadTile에서 파괴되지 않은 CropPrefab 생성
>   >   > 4. CropPrefab.SpawnCropPrefab에서 CropPrefab이 생성될 때 CheckCropSprite를 이용해 CropTile의 growthDay에 따라 스프라이트를 변경시킴
>   >   > </details>
