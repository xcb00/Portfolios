using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyMessage : MonoBehaviour
{
    public TextMeshProUGUI msg;
    CanvasGroup group = null;


    public void Active(string msg)
    {
        if (group == null)
            group = GetComponent<CanvasGroup>();

        this.msg.SetText(msg);
        gameObject.SetActive(true);
        StartCoroutine(Showing());
    }



    private void OnDisable() => StopAllCoroutines();

    IEnumerator Showing()
    {
        float t = 0.3f;
        float time = 0.0f;
        while(time < t)
        {
            time += Time.deltaTime;
            if (time > t) time = t;

            group.alpha = Mathf.Lerp(0f, 1f, time / t);
            yield return null;
        }

        group.alpha = Mathf.Lerp(0f, 1f, 1f);
        t = 0.75f;
        while (t > 0.0f)
        {
            t -= Time.deltaTime;
            yield return null;
        }

        t = 0.3f;
        time = 0.0f;
        while (time < t)
        {
            time += Time.deltaTime;
            if (time > t) time = t;

            group.alpha = Mathf.Lerp(1f, 0f, time / t);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
