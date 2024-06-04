using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCardSlot : MonoBehaviour
{
    private void OnEnable() => EventHandler.ResetCharacterSlotEvent += ResetCard;
    private void OnDisable() => EventHandler.ResetCharacterSlotEvent -= ResetCard;
    void ResetCard() => transform.GetChild(0).gameObject.SetActive(true);
}
