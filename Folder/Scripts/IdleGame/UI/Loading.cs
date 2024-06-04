using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class Loading : MonoBehaviour
{
    public UnityEvent<Vector2Int> CalculateOffline;
    Vector2Int sTime;
    const string ServerURL = "https://www.naver.com/";
    CanvasGroup loading;

    private void Awake()
    {
        loading = GetComponent<CanvasGroup>();
        StartCoroutine(GettingServerTime());
    }

    async void StartFade()
    {
        await Task.Delay(1000);

        EventHandler.CallUpdateGoldEvent();
        EventHandler.CallActiveUIPanelEvent(UIType.Menu, true);
        EventHandler.CallActiveUIPanelEvent(UIType.Upgrade, false);
        EventHandler.CallActiveUIPanelEvent(UIType.Main, false);
        EventHandler.CallActiveUIPanelEvent(UIType.Reward, false);
        EventHandler.CallActiveUIPanelEvent(UIType.Offline, true);
        CalculateOffline?.Invoke(sTime);
        await Fade(0.5f, 1f, 0f);
    }

    private void OnEnable()
    {
        EventHandler.FadeInEvent += FadeIn;
        EventHandler.FadeOutEvent += FadeOut;
    }

    private void OnDisable()
    {
        EventHandler.FadeInEvent -= FadeIn;
        EventHandler.FadeOutEvent -= FadeOut;
    }

    IEnumerator GettingServerTime()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(ServerURL))
        {
            www.timeout = 30;

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Network Error");
            }
            else
            {
                DateTime dateTime = DateTime.Parse(www.GetResponseHeader("date")).ToUniversalTime();
                DataManager.inst.today = int.Parse(dateTime.ToString("dd"));
                sTime = new Vector2Int(int.Parse(dateTime.ToString("HH")), int.Parse(dateTime.ToString("mm")));
            }
            www.Dispose();
        }

        StartFade();
    }

    async void FadeIn(Action before = null, Action after = null)
    {
        before?.Invoke();
        await Fade(0.3f, 1f, 0f);
        after?.Invoke();
    }

    async void FadeOut(Action before = null, Action after = null)
    {
        before?.Invoke();
        await Fade(0.3f, 0f, 1f);
        after?.Invoke();
    }

    public async Task Fade(float time, float startAlpha, float targetAlpha)/*, Action beforeFading = null, Action AfterFading = null)*/
    {
        float t = 0.0f;
        while (t < time)
        {
            t += Time.deltaTime;
            loading.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / time);
            await Task.Yield();
        }
        loading.alpha = targetAlpha;
    }
}
