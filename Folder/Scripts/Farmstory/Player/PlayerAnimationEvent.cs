using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    Player player;
    CharacterDirection direction;
    private void Start()
    {
        player = GetComponentInParent<Player>();
    }

    public void AttackEnemy()
    {
        direction = player.GetPlayerDirection();
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position + Utility.CharacterDirectionToVector3(direction), 0.5f, 1 << (int)LayerName.Enemy);
        foreach(Collider2D enemy in targets)
        {
            enemy.GetComponent<AIState>().Hit((CharacterDirection)(((int)direction + 2) % 4));
        }
    }

    public void DeadEvent()
    {
        EventHandler.CallSleepPanaltyEvent();
    }
}
