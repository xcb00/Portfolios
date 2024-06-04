using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterSlot : MonoBehaviour, IDropHandler
{
    Image character = null;
    bool isFilled = false;

    public void OnDrop(PointerEventData eventData)
    {
        if (isFilled) return;

        isFilled = true;
        character.sprite = eventData.pointerDrag.GetComponentInChildren<CharacterCard>().GetCharacterSprite;
        EventHandler.CallSetCharacterEvent(transform.GetSiblingIndex(), character.sprite.name);
        character.color = Color.white;
        eventData.pointerDrag.SetActive(false);
    }

    private void OnEnable()
    {
        if(character==null)
            character = transform.GetChild(0).GetComponent<Image>();

        EventHandler.ResetCharacterSlotEvent += ResetSlot;
    }

    private void OnDisable()
    {
        EventHandler.ResetCharacterSlotEvent -= ResetSlot;
    }

    void ResetSlot()
    {
        isFilled = false;
        character.color = Color.clear; 
        character.sprite = null;
    }


}
