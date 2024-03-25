using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class TreeCollider : MonoBehaviour
{
    [SerializeField] UnityEvent<bool> triggerEnter;
    [SerializeField] UnityEvent<bool> triggerExit;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((gameObject.activeSelf && (1 << collision.gameObject.layer & LayerMask.GetMask("Player")) != 0))
        {
            triggerEnter?.Invoke(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((gameObject.activeSelf && (1 << collision.gameObject.layer & LayerMask.GetMask("Player")) != 0))
        {
            triggerExit?.Invoke(false);
        }
    }
}
