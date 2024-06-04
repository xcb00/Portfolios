using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    CanvasGroup group = null;
    RectTransform rect = null;

    public Sprite GetCharacterSprite => transform.GetChild(0).GetComponent<Image>().sprite;

    private void OnEnable()
    {
        if(rect==null) rect = GetComponent<RectTransform>();
        rect.localPosition = Vector3.zero;

        if(group==null) group = GetComponent<CanvasGroup>();
        group.blocksRaycasts = true;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //group.interactable = false;
        group.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) => rect.anchoredPosition += eventData.delta * StaticVariables.inst.dragAdjust; 

    public void OnEndDrag(PointerEventData eventData)
    { 
        rect.localPosition = Vector3.zero;
        //group.interactable = true;
        group.blocksRaycasts = true;
    }
}
