** 포톤
1. 포톤 홈페이지에서 프로젝트 생성
2. 페키지 메니저에서 PUN2 다운
3. PUN Wizard에 AppID 입력
4. 

** Firebase 인증
1. 파이어베이스 콘솔에서 프로젝트 생성
2. json 파일을 유니티 프로젝트에 추가
3. 사용할 파이어베이스 SDK 패키지를 임포트
4. 파이어베이스 인증에서 사용할 인증 방법 활성화
5. 프로젝트 설정에 디지털 지문 추가
   > - 유니티의 경우 Keystore를 이용해 디지털 지문 생성 [참고](https://ugames.tistory.com/entry/%EA%B5%AC%EA%B8%80%ED%94%8C%EB%A0%88%EC%9D%B4-%EB%82%B4%EB%B6%80%ED%85%8C%EC%8A%A4%ED%8A%B8-%EB%B0%B0%ED%8F%AC-%EB%8B%A8%EA%B3%84%EB%B3%84-%EC%A0%95%EB%A6%AC)
   > 1. OpenJDK폴더의 bin파일을 `Shift + 우클릭`한 후 Power Shell로 열기
   > 2. '.\keytool -list -v -alias _AliasName_ -keystore _KeystoreDirectory\_KeystoreName.keystore' 입력
   > 3. keystore 비밀번호 입력
   > 4. SHA-1 또는 SHA-256 사용
7. 1

<details>
<summary>ClientState enum</summary>

```C#
public enum ClientState
{
    /// <summary>Peer is created but not used yet.</summary>
    PeerCreated,

    /// <summary>Transition state while connecting to a server. On the Photon Cloud this sends the AppId and AuthenticationValues (UserID).</summary>
    Authenticating,

    /// <summary>Not Used.</summary>
    Authenticated,

    /// <summary>The client sent an OpJoinLobby and if this was done on the Master Server, it will result in. Depending on the lobby, it gets room listings.</summary>
    JoiningLobby,

    /// <summary>The client is in a lobby, connected to the MasterServer. Depending on the lobby, it gets room listings.</summary>
    JoinedLobby,

    /// <summary>Transition from MasterServer to GameServer.</summary>
    DisconnectingFromMasterServer,
    [Obsolete("Renamed to DisconnectingFromMasterServer")]
    DisconnectingFromMasterserver = DisconnectingFromMasterServer,

    /// <summary>Transition to GameServer (client authenticates and joins/creates a room).</summary>
    ConnectingToGameServer,
    [Obsolete("Renamed to ConnectingToGameServer")]
    ConnectingToGameserver = ConnectingToGameServer,

    /// <summary>Connected to GameServer (going to auth and join game).</summary>
    ConnectedToGameServer,
    [Obsolete("Renamed to ConnectedToGameServer")]
    ConnectedToGameserver = ConnectedToGameServer,

    /// <summary>Transition state while joining or creating a room on GameServer.</summary>
    Joining,

    /// <summary>The client entered a room. The CurrentRoom and Players are known and you can now raise events.</summary>
    Joined,

    /// <summary>Transition state when leaving a room.</summary>
    Leaving,

    /// <summary>Transition from GameServer to MasterServer (after leaving a room/game).</summary>
    DisconnectingFromGameServer,
    [Obsolete("Renamed to DisconnectingFromGameServer")]
    DisconnectingFromGameserver = DisconnectingFromGameServer,

    /// <summary>Connecting to MasterServer (includes sending authentication values).</summary>
    ConnectingToMasterServer,
    [Obsolete("Renamed to ConnectingToMasterServer.")]
    ConnectingToMasterserver = ConnectingToMasterServer,

    /// <summary>The client disconnects (from any server). This leads to state Disconnected.</summary>
    Disconnecting,

    /// <summary>The client is no longer connected (to any server). Connect to MasterServer to go on.</summary>
    Disconnected,

    /// <summary>Connected to MasterServer. You might use matchmaking or join a lobby now.</summary>
    ConnectedToMasterServer,
    [Obsolete("Renamed to ConnectedToMasterServer.")]
    ConnectedToMasterserver = ConnectedToMasterServer,
    [Obsolete("Renamed to ConnectedToMasterServer.")]
    ConnectedToMaster = ConnectedToMasterServer,

    /// <summary>Client connects to the NameServer. This process includes low level connecting and setting up encryption. When done, state becomes ConnectedToNameServer.</summary>
    ConnectingToNameServer,

    /// <summary>Client is connected to the NameServer and established encryption already. You should call OpGetRegions or ConnectToRegionMaster.</summary>
    ConnectedToNameServer,

    /// <summary>Clients disconnects (specifically) from the NameServer (usually to connect to the MasterServer).</summary>
    DisconnectingFromNameServer,

    /// <summary>Client was unable to connect to Name Server and will attempt to connect with an alternative network protocol (TCP).</summary>
    ConnectWithFallbackProtocol,

    ConnectWithoutAuthOnceWss
}
```
</details>
