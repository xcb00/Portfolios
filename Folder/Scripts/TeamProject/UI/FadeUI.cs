using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{
    [SerializeField] Text panelTxt;
    [SerializeField] Text endGameTxt;
    [SerializeField] Text preeKeyTxt;

    void Panel(SceneChange msg)
    {
        switch (msg)
        {
            case SceneChange.GameClear:
                PanelTxt("Game Clear", true);
                StartCoroutine(KeyPressing());
                break;
            case SceneChange.GameOver:
                PanelTxt("Game Over", true);
                StartCoroutine(KeyPressing());
                break;
            case SceneChange.StageClear:
                PanelTxt("Stage Clear", false);
                break;
        }
    }
    void PanelTxt(string msg, bool end)
    {
        panelTxt.text = msg;
        endGameTxt.gameObject.SetActive(end);
        preeKeyTxt.gameObject.SetActive(!end);
    }
    IEnumerator KeyPressing()
    {
        GameDatas.pause = true;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
            else if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
            }
            yield return null;
        }
    }

    
    private void OnEnable()
    {
        EventHandler.PanelTextEvent += Panel;
    }

    private void OnDisable()
    {
        EventHandler.PanelTextEvent -= Panel;
    }
}
