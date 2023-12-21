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

## 농장물의 성장 처리
