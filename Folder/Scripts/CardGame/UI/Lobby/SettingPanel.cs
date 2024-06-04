using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI singleRecordTxt;
    [SerializeField] TextMeshProUGUI multiRecordTxt;

    public void ActivePanel(bool active)
    {
        singleRecordTxt.SetText(RecordToString(GameManager.Inst.GetSingleRecord));
        multiRecordTxt.SetText(RecordToString(GameManager.Inst.GetMultiRectord));
        gameObject.SetActive(active);
    }

    string RecordToString(Vector3Int record) => $"W[{record.x.ToString("N0")}] D[{record.y.ToString("N0")}] L[{record.z.ToString("N0")}]";

    public void SaveCloud() => GameManager.Inst.SaveCloud();

    public void LoadCloud() => GameManager.Inst.LoadCloud();

    public void ShowPolicy() => GameManager.Inst.ShowPolicy();
    public void ShowLicense() => Application.OpenURL("https://github.com/xcb00/License/blob/main/License%20Folder/FarmStory.md");
}
