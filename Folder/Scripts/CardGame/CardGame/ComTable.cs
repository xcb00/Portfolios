using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComTable : TableSript
{
    public virtual void PutCard(int cardNum, int index)
    {
        CalculateResult(cardNum);
        resultTxt.SetText(result.ToString());
        cardScript[index].Initialise(cardNum);
    }

    public TableType GetTableType => type;
    public int GetResult => result;
}
