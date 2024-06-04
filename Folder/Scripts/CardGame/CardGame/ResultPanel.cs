using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultTxt;
    [SerializeField] bool isSingle = true;

    public virtual void SetResultPanel(GameResult result, bool perfect = false)
    {
        resultTxt.SetText(EnumCaching.ToString(result));
        GameManager.Inst.SetGameRessult(result, isSingle, perfect);
        gameObject.SetActive(true);
    }

    protected Vector3Int GameResultToRecord(GameResult result)
    {
        switch (result)
        {
            case GameResult.Win:return Vector3Int.right;
            case GameResult.Draw:return Vector3Int.up;
            case GameResult.Lose:return Vector3Int.forward;
            default: return Vector3Int.zero;
        }
    }
}
