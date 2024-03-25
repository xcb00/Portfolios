using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour // where : ���׸� ���� ��������, T�� MonoBehaviour�� ��ӹް� �־�� ��
{
    static T inst;
    public static T Inst { get { return inst; } }
    protected virtual void Awake()
    {
        if (inst == null)
            inst = this as T;
        else 
            Destroy(gameObject);
    }
}
