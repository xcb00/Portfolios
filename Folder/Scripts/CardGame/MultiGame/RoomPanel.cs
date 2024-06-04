using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RoomPanel : MonoBehaviour
{
    float spd = 100f;
    [SerializeField] TextMeshProUGUI roomName;
    [SerializeField] Transform loadingImg;

    public void Inactive() => gameObject.SetActive(false);

    public void EnterRoom(string room)
    {
        roomName.SetText(room);
        gameObject.SetActive(true);

        StartCoroutine(Loading());
    }

    void OnDisable() => StopAllCoroutines();

    IEnumerator Loading()
    {
        loadingImg.transform.localRotation = Quaternion.identity;
        while (true)
        {
            loadingImg.Rotate(Vector3.forward * Time.deltaTime * spd);
            yield return null;
        }
    }

    public void LeaveRoom()
    {
        StopAllCoroutines(); 
        SceneChange.ChangeScene(SceneName.Multi, SceneName.Lobby, null, null, null, null);
        //PhotonNetwork.LoadLevel(EnumCaching.ToString(SceneName.Lobby));
    }
}
