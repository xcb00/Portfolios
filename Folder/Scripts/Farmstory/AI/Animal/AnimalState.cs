using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalState : AIState
{
    SpriteRenderer sprite;

    public override void Respawn(Vector3 position)
    {
        hit = false;
        sprite = GetComponentInChildren<SpriteRenderer>();
        base.Respawn(position);
    }

    protected override void AttackState()
    {
        IdleState();
    }

    protected override void FollowState()
    {
        IdleState();
    }

    protected override IEnumerator Hitting()
    {
        aiState = AIStateType.none;
        ChangeState(AIStateType.idle);
        float time = 0.5f;
        float t = 0.0f;
        Color current = sprite.color;
        while (t < time)
        {
            t += t + Time.deltaTime >= time ? time : t + Time.deltaTime;
            sprite.color = Color.Lerp(current, Color.red, t / (time * 2f));
            yield return null;
        }

        t = 0.0f;
        current = sprite.color;
        while (t < time)
        {
            t += t + Time.deltaTime >= time ? time : t + Time.deltaTime;
            sprite.color = Color.Lerp(current, Color.white, t / time);
            yield return null;
        }
    }

    public void InteractAnimal()
    {

    }
}
