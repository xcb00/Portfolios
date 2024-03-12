##### 2024. 03. 04.

### 목표
1. ~~조이스틱으로 플레이어를 움직일 수 있도록 함~~
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
1. ~~화면 드레그를 이용해 SpringArm 기능 구현~~
2. DataManager를 이용해 JSON으로 데이터를 저장하도록 함

### 완료
- 가상 조이스틱 구현 및 조이스틱으로 플레이어 이동
- 화면 드레그로 x, y축 회전 구현

##### 2024. 03. 07.

### 목표
1. ~~DataManager를 이용해 JSON으로 데이터를 저장하도록 함~~
2. ~~세팅창 구현 및 세팅창의 값을 JSON으로 저장~~
3. 조준점 및 공격 구현

### 완료
- Enum Caching : ToString()을 사용할 경우 박싱이 일어나기 때문에, Caching을 통해 자주 사용하는 enum값을 저장
  > enum을 문자열처럼 사용하는 이유 : 가독성과 유지보수를 용의하게 하기 위해서 이용하며, ToString 사용으로 인한 성능 저하를 Caching으로 보완함
- DataManager를 이용해 클래스/구조체를 JSON 형식으로 저장
  > <details>
  > <summary>Show Script</summary>
  > 
  > **DataClass.cs**
  > ```C#
  > // 클래스/구조체 리스트를 JSON 형식으로 저장하기 위한 구조체
  > [System.Serializable]
  > public struct DataList<T>
  > {
  >   public List<T> dataList;
  >   public DataList(List<T> dataList) =〉this.dataList = dataList;
  > }
  > ```
  > **DataManager.cs**
  > ```C#
  > StringBuilder strBuilder = new StringBuilder();
  > public string GetFilePath(JsonFileName name) // 'name.txt' 파일로 저장할 경로를 반환
  > 
  > // data 구조체/클래스를 Json형식으로 '../name.txt'파일로 저장
  > public void SaveDataToJson<T>(JsonFileName name, T data) =〉File.WriteAllText(GetFilePath(name), JsonUtility.ToJson(data)); 
  > 
  > // 구조체/클래스 리스트를 Json형식으로 '../name.txt'파일로 저장
  > public void SaveDataListToJson<T>(JsonFileName name, List<T> dataList) =〉File.WriteAllText(GetFilePath(name), DataListToJsonString(dataList));
  > 
  > // 구조체/클래스 리스트를 Json형식의 문자열로 변환
  > public string DataListToJsonString<T>(List<T> data) =〉JsonUtility.ToJson(new DataList<T>(data));
  > 
  > // Json 형식의 문자열을 구조체/클래스로 불러오기
  > public T LoadJsonToData<T>(JsonFileNae name, ref bool fileExists)
  > {
  >   string path = GetFilePath(name);
  >   if(!File.Exists(path)) // 데이터가 존재하는지 확인
  >   {
  >     fileExists = false;
  >     return default(T);
  >   }
  >   fileExists = true;
  >   return JsonUtility.FromJson<T>(File.ReadAllText(path));
  > }
  > 
  > // Json 형식의 문자열을 구조체/클래스 리스트로 불러오기
  > public T LoadJsonToData<T>(JsonFileNae name, ref bool fileExists)
  > {
  >   string path = GetFilePath(name);
  >   if(!File.Exists(path)) // 데이터가 존재하는지 확인
  >     return new List<T>();
  > 
  >   return JsonStringToDataList<T>(File.ReadAllText(path));
  > }
  > 
  > // Json 형식의 문자열을 구조체/클래스 리스트로 변환
  > public List<T> JsonStringToDataList<T>(string jsonString) =〉string.IsNullOrEmpty(jsonString) ? new List<T>() : JsonUtility.FromJson<DataList<T>>(jsonString).dataList;
  > 
  > ```
  > </details>
  > 

##### 2024. 03. 07.

### 목표
1. ~~조준점 및 공격 구현~~
2. ~~Setting 창에서 조준점 위치(Spring Arm의 위치)를 변경 및 저장하기~~








