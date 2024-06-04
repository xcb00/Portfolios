using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterClass : MonoBehaviour
{
    protected Coroutine currentRoutine = null;
    protected Coroutine timer = null;

    protected virtual void OnDisable()
    {
        StopAllCoroutines();

        if (timer != null)
            timer = null;
        if (currentRoutine != null)
            currentRoutine = null;
    }
}
