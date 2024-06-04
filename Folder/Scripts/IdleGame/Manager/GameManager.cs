using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    bool healSelf = false;
    [SerializeField] Transform gameSlot = null;
    bool[] characterActive = new bool[4];
    public bool EndGame { get; private set; }

    public int currentStage { get; private set; }
    int reward = 0;

    Dictionary<PlayerCharacter, PlayerCharacterClass> playerCharacters;
    PlayerCharacter[] slots;

    public PlayerCharacter camCharacter;

    public Vector3 GetCameraPosition => gameSlot.GetChild(0).transform.position;
    public Vector2 GetCameraDirection()
    {
        if (camCharacter == PlayerCharacter.None) return Vector2.zero;
        return playerCharacters[camCharacter].GetPlayerDirection;
    }

    protected override void Awake()
    {
        base.Awake();
        LoadPlayerCharacter();
    }

    void LoadPlayerCharacter()
    {
        playerCharacters = new Dictionary<PlayerCharacter, PlayerCharacterClass>();
        for (int i = 1; i < 5; i++)
        {
            GameObject obj = Instantiate(Resources.Load($"Prefab/{EnumCaching.ToString((PlayerCharacter)i)}"), transform) as GameObject;
            playerCharacters[(PlayerCharacter)i] = obj.GetComponent<PlayerCharacterClass>();
            obj.SetActive(false);
        }
    }

    public void SetGameCharacterSlot(PlayerCharacter[] slot)
    {
        //Debug.Log(DataManager.inst.GetOfflineTime);
        slots = new PlayerCharacter[4];
        for (int i = 0; i < slot.Length; i++)
            slots[i] = slot[i];
        camCharacter = slot[0];
        currentStage = 1;
        reward = 0;
        StartStage(currentStage);
    }

    void StartStage(int stage)
    {
        EventHandler.CallChangeStageTextEvent(stage);
        for (int i = 0; i < slots.Length; i++)
        {
            playerCharacters[slots[i]].transform.SetParent(gameSlot.GetChild(i));
            playerCharacters[slots[i]].Initialize(slots[i]);
            playerCharacters[slots[i]].SetLevel(stage == 1);
            gameSlot.GetChild(i).localPosition = Vector3.zero;
        }

        EventHandler.CallResetPositionEvent();

        gameSlot.GetChild(0).localPosition += Vector3.right;
        gameSlot.GetChild(1).localPosition += Vector3.up;
        gameSlot.GetChild(2).localPosition += -Vector3.right;
        gameSlot.GetChild(3).localPosition += -Vector3.up;

        StageManager.inst.StartStage(stage - 1);
        EndGame = false;

    }

    public void ActiveCharacter(PlayerCharacter type) => characterActive[(int)type - 1] = true;

    public void InActiveCharacter(PlayerCharacter type)
    {
        characterActive[(int)type - 1] = false;
        for (int i = 0; i < characterActive.Length; i++)
            if (characterActive[i]) return;
        Debug.Log("All character is inactive");
        EndGame = true;
        StageClear(false);
    }

    public PlayerCharacterClass GetPlayerCharacter()
    {
        PlayerCharacter idx = PlayerCharacter.Knight;
        int minHP = playerCharacters[idx].GetCurrentHP;
        for (int i = 2; i < 5; i++)
        {
            if (!healSelf && (int)PlayerCharacter.Priest == i)
                continue;

            if (minHP > playerCharacters[(PlayerCharacter)i].GetCurrentHP)
            {
                idx = (PlayerCharacter)i;
                minHP = playerCharacters[idx].GetCurrentHP;
            }
        }

        return playerCharacters[idx];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            StageClear(false);
    }

    public void StageClear(bool nextStage = true)
    {
        EndGame = true;

        reward += StageManager.inst.CalculateStageReward(nextStage);
        EventHandler.CallFadeOutEvent(null, () =>
        {
            StageManager.inst.EnqueueAllMonster();
            for (int i = 1; i < 5; i++)
                playerCharacters[(PlayerCharacter)i].DisablePlayerCharacter(false);

            EventHandler.CallFadeInEvent(() =>
            {
                if (nextStage && currentStage++ < StaticVariables.inst.stageSO.stages.Length)
                {
                    StartStage(currentStage);
                }
                else
                    ReturnMain();
            }, null);
        });
    }

    void ReturnMain()
    {
        EndGame = true;

        EventHandler.CallActiveUIPanelEvent(UIType.Menu, true);
        EventHandler.CallActiveUIPanelEvent(UIType.Main, true);
        EventHandler.CallActiveUIPanelEvent(UIType.Reward, true);
        EventHandler.CallResetCharacterCardEvent();
        EventHandler.CallOpenRewardWindowEvent(currentStage, reward);
        EventHandler.CallActiveUIPanelEvent(UIType.Game, false);
    }
}
