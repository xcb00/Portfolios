using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiResult : ResultPanel
{
    public void LobbyButton() => SceneChange.ChangeScene(SceneName.Multi, SceneName.Lobby, null, null, null, null);//PhotonNetwork.LoadLevel(EnumCaching.ToString(SceneName.Lobby));
}
