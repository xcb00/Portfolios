using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleCom : MonoBehaviour
{
    [SerializeField] SingleComTable[] tables;

    SingleManager single = null;
    List<int> comCards = new List<int>();
    int[] comOrder;
    int[,] comTableArr = new int[3, 2];

    int comDefaultCard = 8;
    int comHardCard = 2;
    int turn = 0;

    Dictionary<TableType, SingleComTable> tableDic;

    private void OnEnable()
    {
        if(single==null)
            single = GetComponent<SingleManager>();

        if (tableDic != null)
            tableDic.Clear();
        else
            tableDic = new Dictionary<TableType, SingleComTable>();

        foreach (SingleComTable com in tables)
            tableDic[com.GetTableType] = com;

    }

    public void SetComCards()
    {
        int _diff = Random.Range(0, 6);
        _diff = _diff < 2 ? 2 : (_diff < 5 ? 1 : 0);


        for (int i = 0; i < comDefaultCard; i++)
            comCards.Add(single.DrawFromDeck());

        SetDifficult((Difficulty)_diff);
        SuffleOrder();
    }

    void SuffleOrder()
    {
        turn = 0;
        comOrder = new int[6];
        for (int i = 0; i < comOrder.Length; i++)
            comOrder[i] = i;

        int suffleTime = 10;
        for(int i = 0; i < suffleTime; i++)
        {
            int _idx1 = Random.Range(0, comOrder.Length);
            int _idx2 = Random.Range(0, comOrder.Length);
            int tmp = comOrder[_idx1];
            comOrder[_idx1] = comOrder[_idx2];
            comOrder[_idx2] = tmp;
        }
    }

    void SetDifficult(Difficulty diff)
    {
        switch (diff)
        {
            case Difficulty.Hard:
                for (int i = 0; i < comHardCard; i++)
                    comCards.Add(single.DrawFromDeck());
                CardSort();
                break;
            case Difficulty.Easy:
                ComEasy();
                break;
            case Difficulty.Normal:
            default:
                CardSort();
                break;
        }
    }

    void ComEasy()
    {
        List<int> _list = new List<int>();
        _list = comCards.GetRange(0, 5);
        _list.Sort();

        int _gI = GetGapIdx(_list);
        int _gap = _list[_gI + 1] - _list[_gI];

        if (_gap < 4)
        {
            comTableArr[(int)TableType.Gap, 0] = _list[_gI];
            comTableArr[(int)TableType.Gap, 1] = _list[_gI + 1];
            _list.RemoveAt(_gI);
            _list.RemoveAt(_gI);

            comTableArr[(int)TableType.Max, 0] = _list[_list.Count - 1];
            _list.RemoveAt(_list.Count - 1);

            comTableArr[(int)TableType.Min, 0] = _list[0];
            _list.RemoveAt(0);

            _list.AddRange(comCards.GetRange(5, 3));
            _list.Sort();

            comTableArr[(int)TableType.Max, 1] = _list[_list.Count - 1];
            comTableArr[(int)TableType.Min, 1] = _list[0];
        }
        else
        {
            comTableArr[(int)TableType.Max, 0] = _list[_list.Count - 1];
            comTableArr[(int)TableType.Max, 1] = _list[_list.Count - 2];
            comTableArr[(int)TableType.Min, 0] = _list[0];
            comTableArr[(int)TableType.Min, 1] = _list[1];

            _list.RemoveAt(_list.Count - 1);
            _list.RemoveAt(_list.Count - 1);
            _list.RemoveAt(0);
            _list.RemoveAt(0);

            _list.AddRange(comCards.GetRange(5, 3));
            _gI = GetGapIdx(_list);

            comTableArr[(int)TableType.Gap, 0] = _list[_gI];
            comTableArr[(int)TableType.Gap, 1] = _list[_gI + 1];
        }
    }

    int GetGapIdx(List<int> _list)
    {
        int _gapIdx = 0;
        int _gap = _list[1] - _list[0];

        for(int i=0;i<_list.Count - 1; i++)
        {
            if(_gap > _list[i + 1] - _list[i])
            {
                _gap = _list[i + 1] - _list[i];
                _gapIdx = i;
            }
        }
        return _gapIdx;
    }

    void CardSort()
    {
        comCards.Sort();

        comTableArr[(int)TableType.Max, 0] = comCards[comCards.Count - 1];
        comTableArr[(int)TableType.Max, 1] = comCards[comCards.Count - 2];
        comTableArr[(int)TableType.Min, 0] = comCards[0];
        comTableArr[(int)TableType.Min, 1] = comCards[1];

        comCards.RemoveAt(comCards.Count - 1);
        comCards.RemoveAt(comCards.Count - 1);
        comCards.RemoveAt(0);
        comCards.RemoveAt(0);

        int _gI = GetGapIdx(comCards);

        comTableArr[(int)TableType.Gap, 0] = comCards[_gI];
        comTableArr[(int)TableType.Gap, 1] = comCards[_gI + 1];
    }

    public void PutCard()
    {
        if (turn >= 6)
            return;

        int _order = comOrder[turn++];
        TableType _type = (TableType)Mathf.FloorToInt(_order / 2);
        int _idx = _order % 2;
        tableDic[_type].PutCard(comTableArr[(int)_type, _idx], _idx);
    }
}
