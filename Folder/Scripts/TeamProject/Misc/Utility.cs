using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class Utility
{
    public static IEnumerator Waiting(float time, UnityAction done = null)
    {
        while(time > 0.0f)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        done?.Invoke();
    }
}
