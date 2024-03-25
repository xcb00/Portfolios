using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 싱글톤 디자인을 사용하기 위한 함수
/// 싱글톤으로 만드려는 스크립트에 Singleton을 상속해 사용
/// ScriptName : Singleton<ScripName> 형식으로 사용
/// 
/// 싱글톤을 사용할 경우 바인딩을 하지 않아도 쉽게 접근 가능
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance => instance;
    protected virtual void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Destroy " + gameObject.name);
            Debug.Log("Instance " + instance.name);
            Destroy(gameObject);
        }
        /*
        T _instance = instance;
        instance = gameObject.GetComponent<T>();
        Destroy(_instance.gameObject);*/

        else
            instance = this as T;
    }
}
