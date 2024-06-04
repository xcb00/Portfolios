using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T inst;
    public static T Inst { get { return inst; } }
    protected virtual void Awake()
    {
        if (inst == null)
        {
            inst = this as T;
        }
        else
        {
            Destroy(inst.gameObject);
        }
    }
}