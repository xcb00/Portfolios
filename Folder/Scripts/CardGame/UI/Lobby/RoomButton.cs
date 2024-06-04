using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class RoomButton : MonoBehaviour
{
    LobbyManager pun = null;
    TextMeshProUGUI roomNameTxt = null;
    Button roomBtn = null;
    string roomName = string.Empty;

    public void ActiveRoomButton(string roomName)
    {
        if(roomNameTxt==null)
            roomNameTxt = GetComponentInChildren<TextMeshProUGUI>();
        if(roomBtn == null)
            roomBtn = GetComponentInChildren<Button>();
        if(pun == null)
            pun = transform.root.GetComponentInChildren<LobbyManager>();

        this.roomName = System.String.Copy(roomName);
        roomNameTxt.SetText(roomName);
        transform.name = roomName;
        gameObject.SetActive(true);

        roomBtn.onClick.AddListener(()=>JoinRoom());
    }

    void JoinRoom()
    {
        pun.JoinRoom(this.roomName);
    }

    private void OnDisable() => roomBtn.onClick.RemoveAllListeners();
}
