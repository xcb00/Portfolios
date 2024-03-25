using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    bool b = true;
    //bool pause = true;
    float tick = 0.0f;
    [SerializeField] ExitPanel exit;

    private void OnEnable()
    {
        EventHandler.PlayerSleepEvent += NextDay;
    }

    private void OnDisable()
    {
        EventHandler.PlayerSleepEvent -= NextDay;
    }

    //void TimePause(bool pause) { this.pause = pause; }

    private void Update()
    {
        if (!GameDatas.pause)
            GameTick();
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!GameDatas.pause || !exit.activeSelf)
                exit.ActiveExit(!exit.activeSelf);
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha1))
            TestAdvanceDay(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            TestAdvanceDay(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            TestAdvanceDay(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            TestAdvanceDay(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            TestAdvanceDay(4);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            TestAdvanceDay(5);
        else if (Input.GetKey(KeyCode.Alpha7))
            TestAdvanceDay(6);
        else if (Input.GetKey(KeyCode.Alpha8))
            TestAdvanceDay(7);
        else if (Input.GetKeyDown(KeyCode.Tab))
            EventHandler.CallPrintSystemMassageEvent("Test Msg", 1.0f);
        else if (((!GameDatas.pause) || (GameDatas.pause && exit.activeSelf)) && Input.GetKeyDown(KeyCode.Alpha0))
        {
            exit.ActiveExit(!exit.activeSelf);
        }
#endif
    }

#if UNITY_EDITOR
    public void PrintTime()
    {
        Debug.Log($"YSD : {GameDatas.YearSeasonDay} / HMG : {GameDatas.HourMinuteGold}");
    }

    public void TestAdvanceDay(int num)
    {
        switch (num)
        {
            case 0:
                UpdateMinute(); break;
            case 1:
                UpdateHour(); break;
            case 2:
                UpdateDay(); break;
            case 3:
                UpdateSeason(); break;
            case 4:
                UpdateYear(); break;
            case 5:
                NextDay(); break;/*
            case 6:
                Utility.UseGold(100); break;
            case 7:
                Utility.UseGold(-100); break;*/
        }
    }
#endif

    void GameTick()
    {
        tick += Time.deltaTime;
        if (tick >= Settings.Instance.secondsPerMinute)
        {
            tick -= Settings.Instance.secondsPerMinute;
            UpdateMinute();
        }
    }

    void UpdateMinute(int minute=-1)
    {
        GameDatas.HourMinuteGold.y = minute < 0 ? GameDatas.HourMinuteGold.y + 1 : minute;
        if (GameDatas.HourMinuteGold.y > 3) UpdateHour();
        EventHandler.CallAdvanceGameMinuteEvent();
    }

    void UpdateHour(int hour = -1)
    {
        GameDatas.HourMinuteGold.y = 0;
        GameDatas.HourMinuteGold.x = hour < 0 ? GameDatas.HourMinuteGold.x + 1 : hour;

        if(GameDatas.HourMinuteGold.x == 7) EventHandler.CallNightFadeEvent(false);
        else if(GameDatas.HourMinuteGold.x == 19) EventHandler.CallNightFadeEvent(true);

        if (GameDatas.HourMinuteGold.x > 23)
        {
            UpdateDay();
            EventHandler.CallSleepPanaltyEvent();
        }
        EventHandler.CallAdvanceGameHourEvent();
    }

    void UpdateDay(int day = -1)
    {
        GameDatas.HourMinuteGold.x = 0;
        if (GameDatas.HourMinuteGold.x < 6) GameDatas.pause = true;
        GameDatas.YearSeasonDay.z = day < 0 ? GameDatas.YearSeasonDay.z + 1 : day;
        if (GameDatas.YearSeasonDay.z > Settings.Instance.monthDays) UpdateSeason();

        EventHandler.CallAdvanceGameDateEvent();



        //EventHandler.CallDayPassEvent();
        DataManager.Instance.SaveTimeData();
    }

    void UpdateSeason(Season season = Season.none)
    {
        GameDatas.YearSeasonDay.z = 1;
        GameDatas.YearSeasonDay.y = season != Season.none ? (int)season : (GameDatas.YearSeasonDay.y + 1);
        if (GameDatas.YearSeasonDay.y == (int)Season.none) UpdateYear();
        EventHandler.CallAdvanceGameSeasonEvent();
    }

    void UpdateYear(int year = -1)
    {
        GameDatas.YearSeasonDay.y = (int)Season.Spring;
        GameDatas.YearSeasonDay.x = year < 0 ? (GameDatas.YearSeasonDay.x + 1 > 9999 ? 1 : GameDatas.YearSeasonDay.x + 1) : year;

        // loan 이자 갱신 및 게임오버 여부 파악
        GameDatas.yearChange = true;

        EventHandler.CallAdvanceGameYearEvent();
        GameDatas.yearChange = true;
    }

    public void NextDay()
    {
        PlayerPrefs.SetInt("PlayerHP", 5); 
        SceneControlManager.Instance.ChangeScene(new PlayerData(SceneName.Home, CharacterDirection.down, Vector3.zero));
        if (GameDatas.HourMinuteGold.x > 2)
            UpdateDay();
        UpdateHour(6);
        DataManager.Instance.SaveTimeData();
    }
}
