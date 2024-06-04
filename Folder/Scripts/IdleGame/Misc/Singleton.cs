using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T inst => instance;
    protected virtual void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this as T;
    }
}
