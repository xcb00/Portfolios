using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoubleClick : MonoBehaviour
{
    public int clickCount { get; private set; }
    protected void AddClickCount() { clickCount += 1; }

    protected IEnumerator ClickingSingle(UnityAction singleClick, UnityAction doubleClick)
    {
        clickCount = 1;
        yield return StartCoroutine(ClickingDouble(doubleClick));

        if(clickCount==1)
            singleClick?.Invoke();
        clickCount = 0;
    }

    protected IEnumerator ClickingDouble(UnityAction doubleClick)
    {
        float time = Settings.Instance.doubleClickTime;
        while (time > 0.0f)
        {
            if (clickCount > 1)
            {
                clickCount = 0;
                doubleClick?.Invoke();
                break;
                //StopAllCoroutines();
            }
            time -= Time.deltaTime;
            yield return null;
        }
    }
}
