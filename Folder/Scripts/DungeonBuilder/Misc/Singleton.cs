using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour // where : 제네릭 제약 조건으로, T는 MonoBehaviour를 상속받고 있어야 함
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
