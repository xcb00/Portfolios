using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField]
    //UnityEvent<PlayerCharacter, PlayerCharacter, PlayerCharacter, PlayerCharacter> SetGameCharacter; 
    UnityEvent<PlayerCharacter[]> SetGameCharacter; 
    [SerializeField] Button startGame;
    public PlayerCharacter[] characterSlot = new PlayerCharacter[4];

    private void Start()
    {
        ResetCharacterSlot();        
    }

    private void OnEnable() 
    {
        EventHandler.SetCharacterEvent += SetCharacterSlot;
        EventHandler.ResetCharacterCardEvent += ResetCharacterSlot;
    }

    private void OnDisable()
    {
        EventHandler.SetCharacterEvent -= SetCharacterSlot;
        EventHandler.ResetCharacterCardEvent -= ResetCharacterSlot;
    }

    bool CheckCharacterSlot()
    {
        foreach (PlayerCharacter character in characterSlot)
            if (character == PlayerCharacter.None) return false;

        return true;
    }

    public void ResetCharacterSlot()
    {
        startGame.interactable = false;
        for (int i = 0; i < characterSlot.Length; i++)
            characterSlot[i] = PlayerCharacter.None;
        EventHandler.CallResetCharacterSlotEvent();
    }

    public void StartGame() => EventHandler.CallFadeOutEvent(null, () => 
        {
            SetGameCharacter?.Invoke(characterSlot);
            EventHandler.CallActiveUIPanelEvent(UIType.Game, true);
            EventHandler.CallChangeStageTextEvent(1);
            EventHandler.CallActiveUIPanelEvent(UIType.Main, false);
            EventHandler.CallActiveUIPanelEvent(UIType.Menu, false);
        });

    void SetCharacterSlot(int slot, string character)
    {
        characterSlot[slot] = StringToPlayerCharacter(character);
        startGame.interactable = CheckCharacterSlot();
    }

    PlayerCharacter StringToPlayerCharacter(string character)
    {
        switch (character)
        {
            case "Knight": return PlayerCharacter.Knight;
            case "Archer": return PlayerCharacter.Archer;
            case "Priest": return PlayerCharacter.Priest;
            case "Thief": return PlayerCharacter.Thief;
            default: return PlayerCharacter.None;
        }
    }
}
