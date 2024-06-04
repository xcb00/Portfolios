using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCharacterProperty : CharacterClass
{
    protected Transform moveTransform = null;
    protected PlayerCharacter character;
    protected CharacterState currentState;
    protected PlayerCharacterStatus status;
    protected int currentHP;
    protected int level;
    protected int currentExp;
    protected GameCharacterUI characterUI = null;
    protected Animator animator;

    public virtual void Initialize(PlayerCharacter character)
    {
        this.character = character;
        currentState = CharacterState.none;
        status = StaticVariables.inst.characterSO.GetCharacterStatus(character);
        currentHP = status.hp;
        moveTransform = transform.parent;

        if(characterUI == null)
            characterUI = Instantiate(Resources.Load("Prefab/CharacterInfo"), Vector3.zero, Quaternion.identity, 
                StaticVariables.inst.gameCharacterInfo).GetComponent<GameCharacterUI>();

        if(animator==null)
        {
            animator = GetComponentInParent<Animator>();
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
        }

        gameObject.SetActive(true);
    }
}
