using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class _Knight : MonoBehaviour
{
    int currentHP;
    public float spd = 5f;
    GameCharacterUI characterUI = null;
    Vector2 dir = Vector2.zero;
    CharacterState currentState;
    Coroutine currentRoutine = null;
    PlayerCharacterStatus status = null;


    void Start()
    {
        Init();
    }

    private void OnDisable()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
    }

    void ChangeState(CharacterState state)
    {
        if(state==currentState) return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        switch (state)
        {
            case CharacterState.idle: 
                currentRoutine = StartCoroutine(patroling(CharacterState.move));
                break;
            case CharacterState.move:
                currentRoutine = StartCoroutine(patroling(CharacterState.idle,
                    () => {
                    transform.Translate(dir * Time.deltaTime * status.speed);
                }));
                break;
            case CharacterState.follow: break;
            case CharacterState.attack: break;
            case CharacterState.hurt: break;
            case CharacterState.die: break;

            case CharacterState.none:
            default: break;

        }
    }

    void EndRoutine(CharacterState nextState)
    {
        Debug.Log(currentRoutine);
        StopCoroutine(currentRoutine);
        currentRoutine = null;
        Debug.Log(currentRoutine);
        currentState = CharacterState.none;
        ChangeState(nextState);
    }

    IEnumerator patroling(CharacterState nextState, Action action = null)
    {
        float t = UnityEngine.Random.Range(5, 16) * (nextState == CharacterState.idle?0.1f:0.3f); 
        float degree = UnityEngine.Random.Range(0, 36) * 10f;
        dir.x = Mathf.Cos(degree * Mathf.Deg2Rad);
        dir.y = Mathf.Sin(degree * Mathf.Deg2Rad);
        while (t > 0.0f)
        {
            t -= Time.deltaTime;
            action?.Invoke();
            yield return null;
        }
        EndRoutine(nextState);
    }

    void Init()
    {
        if (GameManager.inst.EndGame) return;

        currentState = CharacterState.none;
        currentHP = 100;

        if (characterUI == null)
            characterUI = Instantiate(Resources.Load("Prefab/CharacterInfo"), Vector3.zero, Quaternion.identity, StaticVariables.inst.gameCharacterInfo).GetComponent<GameCharacterUI>();

        characterUI.InitInfo(100);
        characterUI.MoveCharacter(transform.position);
        GameManager.inst.ActiveCharacter(PlayerCharacter.Knight);
        gameObject.SetActive(true);
        characterUI.ActiveObject(true);
        status = new PlayerCharacterStatus(StaticVariables.inst.characterSO.GetCharacterStatus(PlayerCharacter.Knight));
        ChangeState(CharacterState.idle);
        status.hp += 3;
    }

    public void OnDamage(int damage)
    {
        currentHP -= damage;

        if(currentHP > 0)
        {
            characterUI.OnDamage(currentHP);
        }
        else
        {
            gameObject.SetActive(false);
            characterUI.ActiveObject(false);
            Respawn();
        }
    }

    async void Respawn()
    {
        GameManager.inst.InActiveCharacter(PlayerCharacter.Knight);
        await Task.Delay(5000);
        Init();
    }

}
