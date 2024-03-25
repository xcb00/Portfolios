using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplanationUI : MonoBehaviour
{
    int idx = 0;
    [SerializeField] Image panel;
    [SerializeField] Sprite[] explanations;
    const string show = "ShowExplanation";

    private void Start()
    {
        idx = 0;
        panel.sprite = explanations[idx];
        if (!PlayerPrefs.HasKey(show))
            PlayerPrefs.SetInt(show, 0);
        else
            gameObject.SetActive(PlayerPrefs.GetInt(show) == 0);
    }

    public void NextBtn() { idx = idx + 1 >= explanations.Length ? idx : idx + 1; panel.sprite = explanations[idx]; }
    public void PreviousBtn() { idx = idx - 1 < 0 ? idx : idx - 1; panel.sprite = explanations[idx]; }

    public void NeverShow() { PlayerPrefs.SetInt(show, 1); ClosePanel(); }

    public void ClosePanel() { gameObject.SetActive(false); }


    public void ShowExplanation()
    {
        PlayerPrefs.DeleteKey(show);
        gameObject.SetActive(true);
    }
}
