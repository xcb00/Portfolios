using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �̱��� �������� ����ϱ� ���� �Լ�
/// �̱������� ������� ��ũ��Ʈ�� Singleton�� ����� ���
/// ScriptName : Singleton<ScripName> �������� ���
/// 
/// �̱����� ����� ��� ���ε��� ���� �ʾƵ� ���� ���� ����
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
