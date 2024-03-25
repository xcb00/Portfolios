using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] GameObject loadingImg;
    [SerializeField] Slider loadingBar;
    Coroutine loading = null;
    //float barValue = 0.0f;
    float barSpeed = 0.1f;

    public void StartLoading()
    {
        loadingImg.SetActive(true);
    }

    public void EndLoading(float time)
    {
        if (loading != null) StopCoroutine(loading);
        StartCoroutine(EndingBar(time));
    }

    public void MaxLoadingValue(float maxValue)
    {
        if (loading != null) StopCoroutine(loading);
        loading = StartCoroutine(MovingBar(maxValue));
    }

    IEnumerator MovingBar(float maxValue)
    {
        while(loadingBar.value < maxValue)
        {
            float delta = Time.deltaTime * barSpeed;
            if (loadingBar.value + delta > maxValue) break;// delta = maxValue - barValue;
            loadingBar.value += delta;
            //loadingBar.value = barValue;
            yield return null;
        }
        loadingBar.value = maxValue;
        //loadingBar.value = barValue;
    }

    IEnumerator EndingBar(float t)
    {
        float time = 0.0f;
        float barValue = loadingBar.value;
        while (time < t)
        {
            time += Time.deltaTime;
            loadingBar.value = Mathf.Lerp(barValue, 1f, time / t);
            yield return null;
        }
        loadingImg.SetActive(false);
        gameObject.SetActive(false);
    }
}
