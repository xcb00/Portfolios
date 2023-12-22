## 구글 플레이 스토어에 출시됨
> [다운로드 링크](https://play.google.com/store/apps/details?id=com.geon.farmstory)

## 데이터 관리

- 서버(구글 스프레드시트)로 데이터 관리
  > - `UnityEngine.Networking`의 `UnityWebRequest.Post`를 이용해 서버의 데이터를 받아오도록 제작함
  > - 서버의 데이터를 `JSON`형식을 이용해 서버의 데이터를 받아오도록 제작함
  > - 아이템의 정보와 같이 변동성이 큰 데이터들을 구글 스프레드시트를 이용해 데이터가 변경될 때마다 빌드하지 않도록 제작함
  > - 게임을 실행할 때마다 서버에서 데이터를 가져올 경우 로딩하는 시간이 길어 서버에서 변경된 데이터만 새롭게 받을 수 있도록 제작함

- NetworkManager
  > <details>
  > <summary>Show Code</summary>  
  > 
  > ```C#
  >   public enum DBData
  >   {
  >     CheckVersion, ServerTime, ItemData, CropData, ..., cnt
  >   }
  >   
  >   public class NetworkManager : MonoBehaviour
  >   {
  >     const string URL = "앱스스크립트 주소";
  >     const string versionKey = "Version";
  >     int connectTime = 60; 
  >     bool[] loadData;
  >     [SerializeField] LoadingUI loadingUI = null; // 데이터 로딩 화면
  >   
  >     private void Start()
  >     {
  >       StartCoroutine(LoadingDBData());
  >     }
  >       
  >     IEnumerator LoadingDBData()
  >     {
  >       loadingUI.StartLoading();
  >       GameDatas.pause = true; // 게임 내 시간을 멈춤
  >       float step = Mathf.Floor((1f/((int)DBData.cnt+1))*100f)/100f;
  >       
  >       for(int i=0; i < (int)DBData.cnt; i++)
  >       {
  >         loadingUI.MaxLoadingValue(step * (i+1));
  >         if (i == 0)
  >           yield return StartCoroutine(LoadingJsonData(DBata.CheckVersion, true)); 
  >         else if(i == 1)
  >           yield return StartCoroutine(LoadingNetworkTime()); // 게임 내 출석 보상을 위해 서버의 시간을 가져옴
  >         else
  >           yield return StartCoroutine(LoadingJsonData((DBData)i, loadData[i - 2]));
  >       }
  >       
  >       float t = 0.2f;
  >       loadingUI.EndLoading(t); // 로딩 바를 t 시간 동안 100%로 만듦
  >       while(t > 0.1f) { t -= Time.deltaTime; yield return null; } // t + 0.1f 동안 대기 후 게임 시작
  >       EventHandler.CallDBDataLoadEvent();
  >       SceneControlManager.Instance.LoadLobby();
  >     }
  >        
  >     // LoadingJsonData : name의 데이터를 게임에 불러오는 코루틴으로, loadData가 true일 경우 서버에서 데이터를 가져오고 false인 경우 로컬에 JSON 형식으로 저장된 데이터를 가져옴
  >     IEnumerator LoadingJsonData(DBData name, bool loadData) 
  >     {   string jsonData = string.Empty;  
  >       bool getData = false; // 로컬에 데이터가 있는지 확인  
  >       if(!loadData) // 서버에서 데이터를 가져오지 않아도 되는 경우 로컬에 있는 데이터를 가져옴  
  >         // JsonDataName - Json으로 저장된 텍스트 파일 이름
  >         // 로컬에서 데이터를 불러오지 못했다면 getData를 true로 변경
  >         jsonData = DataManager.Instance.DBJson((JsonDataName)((int)name + 11), ref getData);
  >       else  
  >         getData = true;  
  >         
  >       if(getData)  
  >       {  
  >         WWWForm form = new WWWForm();    
  >           
  >         // 서버에 보낼 데이터(메시지, 필드, 패킷) 생성
  >         form.AddField("dataType", EnumCaching.ToString(name));  
  >         using (UnityWebRequest www = UnityWebRequest.Post(URL, form))  
  >         {  
  >           www.timeout = connectTime;  
  >           yield return www.SendWebRequest();    
  >             
  >           if(www.result != UnityWebRequest.Result.Success)
  >             EventHandler.CallNetworkErrorEvent();    
  >           else  
  >             jsonData = www.downloadHandler.text;  
  >             
  >           www.Dispose();
  >         }
  >       }  
  >         
  >       switch(name)  
  >       {  
  >         DBData 타입에 따라 JSON을 파싱해 저장함    
  >       }  
  >     } 
  >      
  >     void ParseVersion(string json) 
  >     { 
  >       List〈JsonVersion> data = JsonUtility.FromJson<Data<JsonVersion>>(json).dataList;
  >       string[] saveVersion;
  >       loadData = new bool[data.Count];
  >       
  >       for(int i = 0; i 〈 loadData.Length; i++) loadData[i] = false;
  >       
  >       if(string.IsNullOrEmpty(PlayerPrefs.GetString(versionKey)))
  >       {
  >         saveVersion = new string[data.Count];
  >         for(int i = 0; i 〈 saveVersion.Length; i++) saveVersion[i] = "-1"; // 서버데이터 버전이 로컬에 저장되지 않았을 경우 서버에서 데이터를 다시 받아옴
  >       }
  >       else
  >         saveVersion = PlayerPrefs.GetString(versionKey).Split("_");
  >       
  >       // 예외처리 만약 saveVersion의 개수와 data의 개수가 다를 경우 서버에서 데이터를 다시 받아옴
  >       if(saveVersion.Length != data.Count)
  >       {
  >         saveVersion = new string[data.Count];
  >         for(int i = 0; i 〈 saveVersion.Length; i++) saveVersion[i] = "-1"; 
  >       }
  >       
  >       string _version = string.Empty;
  >       for(int i = 0; i 〈 loadData.Length; i++)
  >       {
  >         _version += (i == 0 ? data[i].virsion.ToString() : $"_{data[i].virsion.ToString()}");
  >         loadData[i] = int.Parse(saveVirsion[i]) != data[i].virsion;
  >       }
  >       PlayerPrefs.SetString(versionKey, _version);
  >     }
  >   }
  > ```  
  > </details>
- AppsScript
  > <details>
  > <summary>Show Code</summary>
  > 
  > ```JavaScript
  > var sheetId;
  > var p;
  > var dataList=[];
  > 
  > function response() // 데이터를 유니티에 전송하는 함수
  > {
  >   jsonData = JSON.stringify(dataList);
  >   // 생성한 JSON을 유니티의 JsonUtility로 파싱할 수 있는 형태로 변환
  >   jsonData = "{" + '"dataList"' + ":"+jsonData+"}"
  >   return ContentService.createTextOutput(jsonData);
  > }
  > 
  > function doPost(e)
  > {
  >   sheetID = SpreadsheetApp.openById("스프레드시트 URL");
  >   p = e.parameter;
  >   
  >   // 유니티에서 AddField을 이용해 dataType으로 보낸 값(Value)에 따라 JSON으로 만들어줌
  >   switch(p.dataType)
  >   {
  >     // 스프레드시트에서 데이터를 가져와 JSON으로 만드는 함수
  >   }
  >   return response(); // 만든 JSON을 유니티에 전송
  > }
  > 
  > function GetVersion()
  > {
  >   var sheet = sheetId.getSheets()[0];
  >   dataList = []; // JSON을 만들 데이터를 초기화시킴
  >   var cnt = sheet.getLastRow();
  >   for(let i = 2; i 〈= cnt; i++)
  >   {
  >     if(sheet.getRange(i, 2).getValue()=='') continue;
  >     var data = {};
  >     data.version = sheet.getRange(i, 2).getValue();
  >     dataList.push(data);
  > } 
  > ```
  > </details>
## 농작물의 성장 처리
- 농작물 성장 알고리즘
  > - 처음 개발할 때는 하루가 지나면 저장되어 있는 모든 농작물의 성장일을 1씩 증가하는 방향으로 개발함
  > - 코드 최적화를 위해 농장을 방문했을 때, 이전의 날짜와 현재 날짜의 차를 성장일에 더하는 방향으로 변경함
  > - 농작물 성장 처리 순서
  >   > 1. 새로운 씬을 로드할 때, MapTileManager.InitMap을 이용해 로컬에 저장된 CropTile의 정보를 가져옴
  >   > 2. MapTileManager.LoadTile을 통해 씬에 CropTile 정보가 존재하는 경우 날짜 증가 처리를 함
  >   > 3. CropManager.LoadCrop에서 로컬에 저장된 CropPrefab 정보 중 MapTileManager.LoadTile에서 파괴되지 않은 CropPrefab 생성
  >   > 4. CropPrefab.SpawnCropPrefab에서 CropPrefab이 생성될 때 CheckCropSprite를 이용해 CropTile의 growthDay에 따라 스프라이트를 변경시킴
- MapTileManager
  > <details>
  > <summary>Show Code</summary>
  > 
  > ```C#
  > [System.Serializable]
  > public class CropTileDetails // 좌표의 농장물에 관한 데이터를 관리하는 클래스
  > {
  >   public int seedCode, growthDay;
  >   public bool todayWater;
  >   public Vector2Int coordinate, remain; // remain.x : dugRemain, remain.y : waterRemain
  >   public Vector3Int lastDay;
  >   int gap =〉 Utility.DayGap(lastDay); // 플레이어의 마지막 방문 날짜와 현재 날짜의 차이
  >   public CropTileDetails(CropTileD)
  >   {
  >     // 플레이어가 땅을 팔 때 실행
  >     // 이후에는 List<CropTileDetails>를 JSON으로 저장하고, 불러와 사용하기 때문에 별도의 생성자가 필요하지 않음
  >   }
  >   
  >   public void SetLastDay() { lastDay = GameDatas.YearSeasonDay; } // 마지막 방문 날짜를 현재 날짜로 설정
  >   
  >   public Vector2Int CheckRemain()
  >   {
  >     todayWater = false;
  >     remain = new Vector2Int(remain.x - gap 〈 0? -1 : remain.x - gap, remain.y - gap 〈 0? -1 : remain.y - gap);
  >     growthDay = seedCode > 0 ? growthDay + gap : -1;
  >     return remain;
  >   }
  > }
  > 
  > public class MapTileManager : Singleton<MapTileManager>
  > {
  >   // 농장물을 수확하거나 시든 경우 GameDatas.cropTileList에서 해당 타일을 삭제해야 함
  >   // foreach 또는 for문을 사용해 리스트를 순환하는 중에 삭제할 경우 에러가 발생하기 때문에 삭제해야할 타일들을 저장할 리스트에 추가한 후 순환이 끝난 후 삭제하도록 함
  >   List<CropTileDetails> removeCropTiles;
  >   void InitMapTileList() // 씬 전환 시 실행하는 함수
  >   {
  >     DataManager.Instance.LoadCropTileData(GameDatas.currentscene); // 이전 씬의 cropTileList를 삭제하고, 현재 씬의 cropTileList를 로컬에서 가져옴(로컬에 파일이 없는 경우 빈 리스트 반환)
  >     for(int i = 0; i 〈 tilemaps.Length; i++)
  >       TilemapToList(tilemaps[i].type);
  >     LoadTilemap();
  >   }
  > 
  >   void LoadTilemap()
  >   {
  >     removeCropTiles.Clear(); // CropTileDetails를 삭제하기 위한 리스트
  >     GameDatas.removeCoordinateList.Clear(); // CropPrefab를 삭제하기 위한 리스트
  >     foreach(CropTileDetails tile in GameDatas.cropTileList)
  >     {
  >       Vector2Int result l= tile.CheckRemain();
  >       if(result.x 〈 0)
  >         removeCropTiles.Add(tile); // CropPrefab은 GameDatas.cropTileList에 있는 타일의 위치에만 생성됨(리스트에 없는 경우 Prefab이 생성되지 않음)
  >       else
  >       {
  >         GameDatas.mapTileList[(int)TilemapType.dugGround - 1].Add(new MapTileData(tile.coordinate, TilemapType.dugGround));
  >         if(result.y 〈 0)
  >         {
  >           GameDatas.removeCropCoordinateList.Add(tile.coordinate); // waterRemain이 0보다 작을 경우 농장물 시듦
  >           tile.Wither();
  >         }
  >         else if(result.y > 0) // waterRemain이 0보다 크면 waterTile을 생성
  >           GameDatas.mapTileList[(int)TilemapType.waterGround - 1].Add(new MapTileData(tile.coordinate, TilemapType.waterGround));
  >       }
  >     }
  >    
  >     foreach(CropTileDetails tile in removeCropTiles)
  >       if(tile != null) GameDatas.cropTileList.Remove(tile);
  >     foreach(MapTileData tile in GameDatas.mapTileList[(int)TilemapType.dugGround - 1 ])
  >       SetDugTile(tile.coordinate);
  >     foreach(MapTileData tile in GameDatas.mapTileList[(int)TilemapType.waterGround - 1 ])
  >       SetWaterTile(tile.coordinate);
  >     DataManager.Instance.SaveMapTileData();
  >   }    
  > }  
  > ```
  > </details>


- CropManager
  > <details>
  > <summary>Show Code</summary>
  > 
  > ```C#
  > void LoadCrop() // 이벤트 핸들러를 이용해 씬을 전환할 때 호출
  > {
  >   if(GameDatas.mapTileList[(int)TilemapType.diggable - 1].Count < 1)
  >     return; // 만약 현재 씬에 농장물을 심을 수 없는 경우 함수 종료
  >   GameDatas.cropPrefabList.Clear();
  >   List<CropPrefabJson> dataList = DataManager.Instance.LoadDataToJson<CropPrefabJson>(JsonDataName.CropPrefab, GAmeDatas.currentScene); // 로컬에 JSON 형식으로 저장된 현재 씬의 농장물 정보를 가져옴
  >   if(dataList.Count 〉0)
  >   {
  >     foreach(CropPrefabJson data in dataList)
  >     {
  >       if(GameDatas.removeCropCoordinateList.FindIndex(x =〉x == data.coordinate) 〈 0) 
  >         // 농장물이 생성될 수 없는 좌표가 아닐 경우 농장물 오브젝트 생성
  >         SpawnCrop(data.coordinate, data.seedCode, false);
  >     }
  >   }
  >   DataManager.Instance.SaveCropPrefabData();
  > }
  > 
  > public bool SpawnCrop(Vector2Int coordinate, int seedCode, bool useSeed = true)
  > {
  >   GameObject crop = PoolManager.Instance.DequeueObject(PoolPrefabName.crop);
  >   if(!crop.GetComponent<CropPrefab>().SpawnCropPrefab(GameDatas.cropDetailsList.Find(x =〉x.seedCode == seedCode), coordinate))
  >     // SpawnCropPrefab의 결과값이 false인 경우 false 반환 후 함수 종료
  >     return false;
  >   GameDatas.cropPrefabList.Add(crop.GetComponent<CropPrefab>());
  >   crop.SetActive(true);
  >   if(useSeed)
  >     // 만약 씨앗을 사용한 경우(플레이어가 씨앗을 심은 경우)
  >     EventHandler.CallUseSeedEvent();
  >   return true;
  > }
  > ```
  > </details>
  
- CropPrefab
  > <details>
  > <summary>Show Code</summary>
  > 
  > ```C#
  > CropDetails details;
  > CropTileDetails tileDetails;
  > public Vector2Int coordinate {get; private set;}
  > 
  > public bool SpawnCropPrefab(CropDetails crop, Vector2Int coordinate)
  > {
  >   this.coordinate = coordinate;
  >   details = crop;
  >   tileDetails = GameDatas.cropTileList.Find(x =〉x.coordinate == coordinate);
  >   if(tileDetails == null) return false; // 해당 좌표에 cropTile이 존재하지 않을 경우 false 반환
  >   
  >   tileDetails.seedCode = crop.seedCode;
  >   GetComponentInChildren<SpriteRenderer>().sprite = CheckCropSprite();
  >   transform.position = Utility.CoordinateToPosition(coordinate);
  >   return true;
  > }
  > 
  > Sprite CheckCropSprite()
  > {
  >   int stage = details.growthDay.Length;
  >   int currentStage = 0;
  >   int dayCounter = details.totalGrowthDay;
  >   for(int i = stage -1; i 〉= 0; i--)
  >   {
  >     if(tileDetails.growthDay 〉= dayCounter)
  >     {
  >       currentStage = i;
  >       break;
  >     }
  >     dayCounter -= details.growthDay[i];
  >   }
  >   return details.growthSprite[currentStage];
  > }
  > ```
  > </details>

