using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class GameInfoUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] CanvasGroup info = null;
    [SerializeField] TextMeshProUGUI seasonTxt = null;
    [SerializeField] TextMeshProUGUI timeTxt = null;
    [SerializeField] TextMeshProUGUI godlTxt = null;
    [SerializeField] Image hpImg = null;
    [SerializeField] Image hpEffectImg = null;
    //[SerializeField] BagPanel inventoryPanel = null;
    [SerializeField] Panel menuPanel = null;
    float hpEffectTime = 0.6f;
    StringBuilder sb = new StringBuilder();
    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += UpdateClock;
        EventHandler.AdvanceGameHourEvent += UpdateClock;
        EventHandler.GoldChangeEvent += UpdateClock;
        EventHandler.UpdateHPBarEvent += UpdateHPBar;
        EventHandler.AfterSceneLoadEvent += ActiveInfo;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= UpdateClock;
        EventHandler.AdvanceGameHourEvent -= UpdateClock;
        EventHandler.GoldChangeEvent -= UpdateClock;
        EventHandler.UpdateHPBarEvent -= UpdateHPBar;
        EventHandler.AfterSceneLoadEvent -= ActiveInfo;
    }

    void ActiveInfo()
    {
        info.alpha = 1f;
        info.interactable = true;
        info.blocksRaycasts = true;
    }

    void UpdateClock()//(int year, Season season, int day, int hour, int minute)
    {
        sb.Clear();
        sb.Append(GameDatas.YearSeasonDay.x.ToString().PadLeft(2, '0'));
        sb.Append(". ");
        sb.Append(EnumCaching.ToString((Season)GameDatas.YearSeasonDay.y));
        sb.Append(". ");
        sb.Append(GameDatas.YearSeasonDay.z.ToString().PadLeft(2, '0'));
        seasonTxt.SetText(sb);

        sb.Clear();
        sb.Append(GameDatas.HourMinuteGold.x.ToString().PadLeft(2, '0'));
        sb.Append(":");
        sb.Append((GameDatas.HourMinuteGold.y * 15).ToString().PadLeft(2, '0'));
        timeTxt.color = GameDatas.HourMinuteGold.x >= 23 ? Color.red : Color.black;
        timeTxt.SetText(sb);

        godlTxt.SetText(GameDatas.HourMinuteGold.z.ToString("N0"));
    }

    void UpdateHPBar(int hp, float divideValue)
    {
        hpImg.fillAmount = divideValue * hp; // < 0f ? (float)(hp / maxHP) : hp * divideValue;
        StopAllCoroutines();
        StartCoroutine(ReducingHPEffect(hpImg.fillAmount));
    }

    IEnumerator ReducingHPEffect(float to)
    {
        float from = hpEffectImg.fillAmount;
        float t = 0.0f;
        while (t < hpEffectTime)
        {
            t = t + Time.deltaTime > hpEffectTime ? hpEffectTime : t + Time.deltaTime;
            hpEffectImg.fillAmount = Mathf.Lerp(from, to, t / hpEffectTime);
            yield return null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(Mathf.Approximately(menuPanel.GetCanvasAlhpa(), 0f))
            menuPanel.ActivePanel();
    }
}
