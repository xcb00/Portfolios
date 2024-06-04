using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks // Pun의 이벤트를 감지할 수 있는 MonoBehaviour
{
    GameObject roomBtn;
    [SerializeField] Transform roomList;
    [SerializeField] CanvasGroup multiButtons;

    List<string> rooms = null;
    List<RoomInfo> roomInfos = null;

    private readonly string version = "1.0";
    readonly string roomNameFormat = "Room {0}";
        
    public string RoomName(int ran) => System.String.Format(roomNameFormat, PhotonNetwork.CountOfRooms + 1 + ran);
    
    private void Start()
    {
        //SceneChange.CheckManagerScene();
        if (!PhotonNetwork.IsConnected)
            ConnectServer();
        else
        {
            if (PhotonNetwork.CurrentRoom != null)
                PhotonNetwork.LeaveRoom();

            LoadRoomInfos();
            EventHandler.CallFadeDelayEvent(false);
        }
        //rooms = null;
        roomBtn = Resources.Load("Prefab/UI/RoomButton") as GameObject;
        GameManager.Inst.ShowBanner();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        GameManager.Inst.DestoryBanner();
    }

    void ConnectServer()
    {
        PhotonNetwork.GameVersion = version;
        PhotonNetwork.ConnectUsingSettings(); // 서버에 접속을 시도하는 함수
    }

    #region Photon Event Method
    #region Photon Join Room Event Method
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        multiButtons.interactable = false;
        multiButtons.alpha = 0.6f;
        EventHandler.CallFadeDelayEvent(false);
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        multiButtons.interactable = true;
        multiButtons.alpha = 1.0f;
        EventHandler.CallFadeDelayEvent(false);
    }

    public override void OnJoinedRoom()
    {
        SceneChange.ChangeScene(SceneName.Lobby, SceneName.Multi, null, null, null, () => EventHandler.CallCheckPlayerEvent());
        return;
    }
    #endregion

    #region Photon Load Room List Event Method
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        roomInfos = roomList;
        LoadRoomInfos();
    }
    #endregion

    #region Photon Failed Event Method
    public override void OnJoinRandomFailed(short returnCode, string message) => CreateRoom();
    public override void OnCreateRoomFailed(short returnCode, string message) => CreateRoom(UnityEngine.Random.Range(PhotonNetwork.CountOfRooms + 5, PhotonNetwork.CountOfRooms + 15));
    #endregion
    #endregion

    #region Button Event Method
    public void CreateRoom()
    {
        if (CheckGold())
            CreateRoom(0);
    }

    void CreateRoom(int i) => PhotonNetwork.CreateRoom(RoomName(i), new RoomOptions { MaxPlayers = 2 });

    bool CheckGold()
    {

        if (GameManager.Inst.gold >= 100)
            return true;

        EventHandler.CallShowMoenyMessageEvent("Not enough money");
        return false;
    }

    public void JoinRandomRoom()
    {
        if(CheckGold())
            PhotonNetwork.JoinRandomRoom();
    }
    public void JoinRoom(string roomName)
    {
        if (CheckGold())
            PhotonNetwork.JoinRoom(roomName);
    }

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    void LoadRoomInfos()
    {
        if (roomInfos == null || roomInfos.Count < 1)
            return;

        rooms = new List<string>();

        foreach(RoomInfo room in roomInfos)
        {
            if (room.IsOpen && room.PlayerCount == 1)
                rooms.Add(room.Name);
        }

        if (rooms.Count < 1) return;
        int btnCount = roomList.childCount;

        for (int i = 0; i < btnCount; i++)
            roomList.GetChild(i).gameObject.SetActive(false);

        for (int i = 0; i < rooms.Count; i++)
        {
            if (i < btnCount)
                roomList.GetChild(i).GetComponent<RoomButton>().ActiveRoomButton(rooms[i]);
            else
                Instantiate(roomBtn, roomList).GetComponent<RoomButton>().ActiveRoomButton(rooms[i]);
        }

    }

    public void SinglePlay()
    {
        if (GameManager.Inst.gold < 10)
            EventHandler.CallShowMoenyMessageEvent("Not enough money");
        else
            SceneChange.ChangeScene(SceneName.Lobby, SceneName.Single, null, null, null, null, false);
    }


    #endregion

    public void GetGold() => GameManager.Inst.ShowReward();
}
