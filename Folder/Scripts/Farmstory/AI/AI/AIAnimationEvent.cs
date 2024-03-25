using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnimationEvent : MonoBehaviour
{
    [SerializeField] PoolPrefabName poolType;
    public void DeadEvent()
    {
        PoolManager.Instance.EnqueueObject(poolType, transform.parent.parent.gameObject);
    }
}
