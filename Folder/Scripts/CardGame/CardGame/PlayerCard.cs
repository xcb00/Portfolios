using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    TextMeshProUGUI numberTxt = null;
    RectTransform rect = null;
    Image image = null;
    int cardNumber;
    int index;
    Action<int> DropCard;
    Color transparent = new Color(1f, 1f, 1f, 0.6f);

    float adjust;

    public int GetCardNumber => cardNumber;

    void OnDisable()
    {
        DropCard?.Invoke(index);
        DropCard = null;
    }

    public void DestoryCard()
    {
        DropCard = null;
        gameObject.SetActive(false);
    }

    public void Initialise(int num, int index, Action<int> DropCard, float adjust)
    {
        if (rect == null) rect = GetComponent<RectTransform>();
        if(image == null) image = GetComponent<Image>();
        if(numberTxt == null) numberTxt = GetComponentInChildren<TextMeshProUGUI>();

        rect.localPosition = Vector3.zero;
        cardNumber = num;
        this.index = index;
        this.DropCard += DropCard;
        this.adjust = adjust;
        numberTxt.SetText(cardNumber.ToString());

        image.raycastTarget = false;
        rect.localPosition = Vector3.zero;
        image.color = Color.white;
        gameObject.SetActive(true);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        image.color = transparent;
    }

    public void OnDrag(PointerEventData eventData) => rect.anchoredPosition += eventData.delta * adjust;

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        rect.localPosition = Vector3.zero;
        image.color = Color.white;
    }
}
