using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TableSript : MonoBehaviour, IDropHandler
{
    [SerializeField] protected TableType type;
    [SerializeField] protected TextMeshProUGUI resultTxt;
    protected CardScript[] cardScript;
    protected int result;
    protected int cardIdx = 0;
    protected int cardNum = 0;

    protected virtual void OnEnable()
    {
        cardScript = GetComponentsInChildren<CardScript>();
        for (int i = 0; i < cardScript.Length; i++)
            cardScript[i].gameObject.SetActive(false);
        resultTxt.SetText("0");
        cardIdx = 0;
    }

    protected void CalculateResult(int num)
    {
        switch (type)
        {
            case TableType.Max:
            case TableType.Min:
                result += num;
                break;
            case TableType.Gap:
                result = Mathf.Abs(num - result);
                break;
            default:
                break;
        }
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        if (cardIdx > 1) return;

        cardNum = eventData.pointerDrag.GetComponent<PlayerCard>().GetCardNumber;
        CalculateResult(cardNum);
        resultTxt.SetText(result.ToString());
        cardScript[cardIdx++].Initialise(cardNum);
        eventData.pointerDrag.SetActive(false);
    }
}
