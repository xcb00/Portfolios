using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterProperty : CharacterClass
{
    protected MonsterCharacter character;
    protected CharacterState currentState;
    protected MonsterStatus status;
    protected int currentHP;
    protected Animator animator = null;
    protected bool isBoss = false;

    public virtual void Initialize(MonsterCharacter character, bool isBoss = false)
    {
        this.character = character;
        currentState = CharacterState.none;
        status = StaticVariables.inst.characterSO.GetCharacterStatus(character);
        this.isBoss = isBoss;

        float stageAdjust = (9 + GameManager.inst.currentStage) * (isBoss ? 0.2f : 0.1f);
        status.hp = Mathf.RoundToInt(status.hp * stageAdjust);
        status.damage = Mathf.RoundToInt(status.damage * stageAdjust);
        status.attackCooldown = status.attackCooldown / stageAdjust * (isBoss ? 1.8f : 1.0f);
        transform.GetChild(0).localScale *= (isBoss ? 2f : 1f);

        currentHP = status.hp;

        if (animator == null)
        {
            animator = GetComponentInParent<Animator>();
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
        }
        gameObject.SetActive(true);
    }
}
