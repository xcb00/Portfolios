using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    /*float tick = 0.0f;
    bool pause = true;

    private void OnEnable()
    {
        //EventHandler.PauseTimeEvent += PauseTime;
        EventHandler.StageStartEvent += StageClear;
    }
    private void OnDisable()
    {
        //EventHandler.PauseTimeEvent -= PauseTime;
        EventHandler.StageStartEvent -= StageClear;
    }

    void StageClear()
    {
        GameDatas.pause = false;
        tick = 0.0f;
    }

    private void Update()
    {
        if (!GameDatas.pause)
        {
            tick += Time.deltaTime;

            if (tick >= Settings.waveDelay)
            {
                tick -= Settings.waveDelay;
                GameDatas.pause = true;
                EventHandler.CallStartWaveEvent();
            }
        }
    }*/

    //void PauseTime(bool pause) { this.pause = pause; }
}
