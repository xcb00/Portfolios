using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightMask : MonoBehaviour
{
    SpriteRenderer sRender = null;
    private void OnEnable()
    {
        EventHandler.NightFadeEvent += NightFadeEvent;
        //EventHandler.NightEvent += NightEvent;
        EventHandler.NightEvent += CheckScene;
        EventHandler.AfterSceneLoadBeforeFadeInEvent += CheckScene;

        if (sRender==null)
            sRender = GetComponent<SpriteRenderer>();
    }

    private void OnDisable()
    {
        EventHandler.NightFadeEvent -= NightFadeEvent;
        EventHandler.NightEvent -= CheckScene;
    }

    void NightFadeEvent(bool day2night)
    {
        switch (GameDatas.currentScene)
        {
            case SceneName.Home:
            case SceneName.Shop:
            case SceneName.Casino:
                day2night = false;
                break;
            case SceneName.MineF:
            case SceneName.MineB1:
            case SceneName.MineB2:
                day2night = true;
                break;
            default:
                break;
        }
        EventHandler.CallNightMaskSwitch(day2night);
        StartCoroutine(Utility.Fading(sRender, new Color(0f, 0f, 0f, day2night ? Settings.Instance.nightAlpha : 0.0f), 0.5f));
    }

    void NightEvent(bool isNight)
    {
        EventHandler.CallNightMaskSwitch(isNight);
        sRender.color = new Color(0f, 0f, 0f, isNight ? Settings.Instance.nightAlpha : 0.0f);
    }

    void CheckScene()
    {
        switch(GameDatas.currentScene)
        {
            case SceneName.Lobby:
            case SceneName.Shop:
            case SceneName.Home:
            case SceneName.Casino:
                NightEvent(false);
                break;
            case SceneName.MineF:
            case SceneName.MineB1:
            case SceneName.MineB2:
                NightEvent(true);
                break;
            default:
                if (GameDatas.HourMinuteGold.x > 7 && GameDatas.HourMinuteGold.x < 19)
                    NightEvent(false);
                else
                    NightEvent(true);
                break;
        }
    }
}
