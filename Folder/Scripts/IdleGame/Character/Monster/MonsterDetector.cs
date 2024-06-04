using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDetector : MonoBehaviour
{
    CircleCollider2D detectCollider;
    Action<Transform> enterCharacter;
    Action<Transform> exitCharacter;

    private void Awake()
    {
        detectCollider = GetComponent<CircleCollider2D>();        
    }

    public void Initialize(float detectDist, Action<Transform> enterCharacter, Action<Transform> exitCharacter)
    {
        detectCollider.radius = detectDist;
        this.enterCharacter += enterCharacter;
        this.exitCharacter += exitCharacter;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(((1 << (int)LayerName.PlayerCollider) & (1 << collision.gameObject.layer)) != 0)
        {
            enterCharacter?.Invoke(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << (int)LayerName.PlayerCollider) & (1 << collision.gameObject.layer)) != 0)
        {
            exitCharacter?.Invoke(collision.transform);
        }
    }
}
