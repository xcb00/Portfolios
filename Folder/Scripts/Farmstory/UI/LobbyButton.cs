using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyButton : MonoBehaviour
{
    [SerializeField] GameObject startBtn;

    private void OnEnable()
    {
        EventHandler.LoadLobby += ActiveButton;
    }

    private void OnDisable()
    {
        EventHandler.LoadLobby -= ActiveButton;
    }

    void ActiveButton()
    {
        transform.GetComponent<CanvasGroup>().blocksRaycasts = true;
        startBtn.SetActive(DataManager.Instance.PreviousDataExist);
    }

    public void StartGameBtn(bool NewGame = false)
    {
        if (NewGame)
        {
            DataManager.Instance.DeleteJson();
        }

        EventHandler.CallStartGameEvent();
        DataManager.Instance.LoadSettingValue();
        SceneControlManager.Instance.ChangeScene();
    }
}
