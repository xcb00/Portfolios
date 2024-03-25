using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(KeyPressing());
    }

    IEnumerator KeyPressing()
    {
        //EventHandler.CallStopRespawnEvent(true);
        GameDatas.pause = true;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
            else
            {
                if (Input.anyKeyDown) break;
            }
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
