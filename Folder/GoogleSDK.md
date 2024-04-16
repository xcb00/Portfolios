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
6. 1
