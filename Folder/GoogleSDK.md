## GPGS
[GPGS Docs](https://github.com/playgameservices/play-games-plugin-for-unity) / [gorani_TV GPGS](https://www.youtube.com/watch?v=aCG5nixgyVw)
1. Player Settings
   > - Switch Platform
   > - API Version
   > - Java 환경변수 설정
   >   > <details>
   >   > <summary>240416</summary>
   >   > 
   >   > ### Additional instructions on building for Android on Windows
   >   > If you are using Windows, you must make sure that your Java SDK installation can be accessed by Unity. To do this:   
   >   > 1. Set the JAVA_HOME environment variable to your Java SDK installation path (for example, `C:\Program Files\Java\jdk1.7.0_45`)   
   >   > 2. Add the Java SDK's `bin` folder to your `PATH` environment variable (for example, `C:\Program Files\Java\jdk1.7.0_45\bin`)   
   >   > 3. Reboot.   
   >   > [참고](https://code-algo.tistory.com/28)
   >   > </details>
2. Import GPGS Package
   > <details>
   > <summary>GooglePlayGamesPlugin-0.11.01 failed error</summary>
   > 
   > 1. `Assets\GooglePlayGames\com.google.play.games\Editor\GooglePlayGamesPluginDependencies.xml` 열기   
   > 2. repository 내부를 `Assets/GooglePlayGames/com.google.play.games/Editor/m2repository`로 변경   
   > 3. `Assets > External Dependency Manager > Android Resolver > Force Resolve`로 Plug-in 설치
   > </details>
3. Google Play Console에 앱 생성 및 등록
   > <details>
   > <summary>Build 시 Gradle Error</summary>
   > 
   > 1. `Player Settings > Publishing Settings > Build > Custom Gradle Properties Template` 체크
   > 2. `Assets\Plugins\Android`의 `graldeTemplate.properties` 열기
   > 3. `android.enableD8=true` 붙여넣기
   > 4. `android.enableR8=false` 붙여넣기
   > </details>
4. Play 게임즈 서비스 설정
   > <details>
   > <summary>Google Play Games Setting</summary>
   > 
   > 1. `Play 게임즈 서비스 > 설정 및 관리 > 설정 > 새 Play 게임즈 서비스 프로젝트 만들기`
   > 2. `Google Cloud Console`에서 **앱 이름과 동일하게** 새 프로젝트 생성
   > 3. `Google Play Console`에서 `Google Cloud Console`에서 생성한 프로젝트를 `클라우드 프로젝트`로 선택
   > 4. `Google Cloud Platform에서 OAuth 동의 화면 만들기` 클릭 후 `Google Cloud Platform` 클릭
   > 5. User Type을 외부로 선택 후 만들기
   > 6. 앱 이름에 프로젝트 이름과 동일하게 입력하고, 사용자 지원 이메일, 개발자 연락처 정보에 이메일 입력 후 저장 후 계속
   > 7. 사용자 인증서보 추가 > 게임 서버 > OAuth 클라이언트 만들기 > `OAuth 클라이언트 ID 만들기` 클릭
   > 8. 애플리케이션 유형 - 웹 애플리케이션 선택, 이름에 앱 이름 입력 후 만들기 클릭
   > 9. `Google Play Console`에서 OAuth 클라이언트를 생성한 클라이언트로 선택 후 변경사항 저장
   > 10. 사용자 인증서보 추가 > Android > OAuth 클라이언트 만들기 > `OAuth 클라이언트 ID 만들기` 클릭
   > 11. `Google Play Console`의 `OAuth 클라이언트를 만드는 방법`에 나와있는 내용을 `Google Cloud Platform`의 `OAuth 클라이언트 ID 만들기에 입력 후 만들기
   > 12. `Google Play Console`에서 OAuth 클라이언트를 생성한 클라이언트로 선택 후 변경사항 저장
   > 13. 플레이 스토어에 출시 전 테스트를 위해 `앱 무결성 > 앱 서명 > 업로드 키 인증서`의 SHA-1 인증서 지문을 가지는 사용자 인증 정보를 추가 생성 (10~12 반복)
   > 14. `Google Cloud Console > OAuth 동의화면`에서 앱 게시
   > 15. `Google Play Console > Play 게임즈 서비스 > 설정 및 관리 > 설정 > 사용자 인증 정보`에서 리소스 보기 후 `xml` 복사
   > 16. 'Unity > Window > Google Play Games > Setup > Android Setup`으로 `Google Play Games - Android Configuration` 열기
   > 17. `Google Play Games - Android Configuration`의 Resources Definition에 xml 붙여넣기
   > 18. `Google Cloud Platform > 사용자 인증 정보 > 웹 어플리케이션`의 클라이언트 ID를 `Google Play Games - Android Configuration > Client ID`에 입력 후 Setup
   > </details>
5. GooglePlayGames에서 사용할 API 작성
   > - Log in
   >   > <details>
   >   > <summary>Show Code</summary>
   >   > 
   >   > ```C#
   >   > using GooglePlayGames;
   >   > using GooglePlayGames.BasicApi;
   >   > using UnityEngine.SocialPlatforms;
   >   > using UnityEngine.UI;
   >   > public class GoogleLogin : MonoBehaviour
   >   > {
   >   >    public Text login;
   >   >    public void Login()
   >   >    {
   >   >       // Unity에서 제공하는 Social을 사용하기 위해 사용
   >   >       // Unity의 Social을 사용하지 않을 경우 PlayGamesPlatform.Instance.Authenticate만 사용할 수 있음
   >   >       PlayGamesPlatform.Activate(); 
   >   >       
   >   >       // SignInStatus.Success : The operation was successful
   >   >       // SignInStatus.InternalError : An internal error occurred
   >   >       // SignInStatus.Canceled : The sign in was canceled
   >   >       PlayGamesPlatform.Instance.Authenticate(
   >   >          (SignInStatus status) =〉login.text = status == SignInStatus.Success ? $"Success : {Social.localUser}" : $"Failed : {status}";);
   >   >    } 
   >   > }
   >   > ```
   >   > </details>   
   > - Save/Load
   >   > 1. `Google Play Console > Play 게임즈 서비스 > 설정 및 관리 > 설정 > 속성`에서 속성 수정을 클릭 후 저장된 게임을 사용으로 변경
   >   > 2. `Google Cloud Console > 메뉴 > 제품 및 솔루션 > 모든 제품 > Google Workspace > API > Google Workspace Marketplace SDK` 사용
   >   > 3. 'Google Play Console > 대시보드 > 앱 설정 > 앱 엑세스 권한`에서 엑세스 권한 허용
   >   > <details>
   >   > <summary>Show Code</summary>
   >   > 
   >   > ```C#
   >   > using GooglePlayGames;
   >   > using GooglePlayGames.BasicApi;
   >   > using UnityEngine.SocialPlatforms;
   >   > public class GoogleLogin : MonoBehaviour
   >   > {
   >   >    
   >   > }
   >   > ```
   >   > </details>
   > - Friends
   >   > <details>
   >   > <summary>Show Code</summary>
   >   > 
   >   > ```C#
   >   > using GooglePlayGames;
   >   > using GooglePlayGames.BasicApi;
   >   > using UnityEngine.SocialPlatforms;
   >   > using UnityEngine.UI;
   >   > public class GoogleLogin : MonoBehaviour
   >   > {
   >   >    public Text askFriend;
   >   >    public Text loadFriend;
   >   >    public void LoadFriend()
   >   >    {
   >   >       // LoadFriendsStatus.Completed : All the friends have been loaded
   >   >       // LoadFriendsStatus.LoadMore : There are more friends to load
   >   >       // LoadFriendsStatus.ResolutionRequired : The game doesn't have permission to access the player's friends list
   >   >       
   >   >       // GetLastLoadFriendsStatus : 마지막으로 친구 목록을 로드한 상태를 반환
   >   >       // 만약 게임에 친구 목록을 불러올 권한이 없을 경우 AskForLoadFriendsResolution로 플레이어에게 권한 요청
   >   >       if(PlayGamesPlatform.Inst.GetLastLoadFriendsStatus() == LoadFriendsStatus.ResolutionRequired)
   >   >          PlayGamesPlatform.Instance.AskForLoadFriendsResolution(
   >   >             (result) = 〉askFriend.text = result == UISatus.Valid ? "Player Agree" : "Player Disagree");
   >   >       
   >   >       // 플레이어의 친구 목록을 불러옴
   >   >       // LoadFriends : Google Play Games의 친구 목록을 불러오는 메소드로
   >   >       // pageSize(불러올 친구 개수), forceReload(캐시 사용 여부), callback을 변수로 가짐
   >   >       // forceReload가 true일 경우 캐시에 상관 없이 서버에서 친구 목록을 새로 불러오고, false일 경우 캐시가 있을 경우 캐시 사용
   >   >       PlayGamesPlatform.Inst.LoadFriends(3, false, (status)=〉{
   >   >          if(status == LoadFriendsStatus.Completed) // 친구 불러오기가 완료되었을 경우 친구의 userName을 출력
   >   >          {
   >   >             foreach(IUserProfile _friend in Social.localUser.friends)
   >   >                loadFriend.text += _friend.userName + "/";
   >   >          }
   >   >          else
   >   >             loadFriend.text = $"Load Friend Failed : {status}";
   >   >    }
   >   > }
   >   > ```
   >   > </details>
   > - Achievements / Leaderboard / Event
   >   > 1. `Google Play Console > Play 게임즈 서비스 > 업적/리더보드/이벤트`에서 업적/리더보드/이벤트 생성
   >   > 2. 리소스 보기 후 복사(업적/리더보드/이벤트의 리소스가 모두 동일)
   >   > 3. 'Unity > Window > Google Play Games > Setup > Android Setup`으로 `Google Play Games - Android Configuration - Resources Definition`에 xml 붙여넣기
   >   > 4. Setup
   >   > <details>
   >   > <summary>Achievements Code</summary>
   >   > 
   >   > ```C#
   >   > using GooglePlayGames;
   >   > using GooglePlayGames.BasicApi;
   >   > using UnityEngine.SocialPlatforms;
   >   > public class GoogleLogin : MonoBehaviour
   >   > {
   >   >    public void Login()
   >   >    {
   >   >       PlayGame
   >   > }
   >   > ```
   >   > </details>
   >   > <details>
   >   > <summary>Leaderboard Code</summary>
   >   > 
   >   > ```C#
   >   > using GooglePlayGames;
   >   > using GooglePlayGames.BasicApi;
   >   > using UnityEngine.SocialPlatforms;
   >   > public class GoogleLogin : MonoBehaviour
   >   > {
   >   >    public void Login()
   >   >    {
   >   >       PlayGame
   >   > }
   >   > ```
   >   > </details>
   >   > <details>
   >   > <summary>Event Code</summary>
   >   > 
   >   > ```C#
   >   > using GooglePlayGames;
   >   > using GooglePlayGames.BasicApi;
   >   > using UnityEngine.SocialPlatforms;
   >   > public class GoogleLogin : MonoBehaviour
   >   > {
   >   >    public void Login()
   >   >    {
   >   >       PlayGame
   >   > }
   >   > ```
   >   > </details>
7. 1
