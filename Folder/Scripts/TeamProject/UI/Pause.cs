using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    private void OnEnable()
    {
        //EventHandler.CallPauseTimeEvent(true);
        GameDatas.pause = true;
        GameDatas.stopUnit = true;
        EventHandler.CallStopRespawnEvent(true);
        StartCoroutine(KeyPressing());
    }

    IEnumerator KeyPressing()
    {
        EventHandler.CallStopRespawnEvent(true);
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
            else
            {
                if (Input.anyKeyDown) break;
            }
            yield return null;
        }

        //EventHandler.CallPauseTimeEvent(false);
        GameDatas.pause = false;
        GameDatas.stopUnit = false;
        EventHandler.CallStopRespawnEvent(false);
        gameObject.SetActive(false);
    }
}
