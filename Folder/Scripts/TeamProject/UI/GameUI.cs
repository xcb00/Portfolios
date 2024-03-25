using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] CanvasGroup unitBtns;
    [SerializeField] CanvasGroup upgradeBtns;
    [SerializeField] CanvasGroup shopBtns;
    [SerializeField] Image activeMark;

    private void Start()
    {
        activeMark.rectTransform.localPosition = new Vector3(-221.25f, 70.0f, 0.0f);
    }

    public void ActiveBtns(int i)
    {
        activeMark.rectTransform.localPosition = new Vector3(-221.25f + 147.5f*(i-1), 70.0f, 0.0f);
        switch (i)
        {
            case 1:
                InactiveGroup(shopBtns);
                InactiveGroup(upgradeBtns);
                ActiveGroup(unitBtns);
                break;
            case 2:
                InactiveGroup(shopBtns);
                InactiveGroup(unitBtns);
                ActiveGroup(upgradeBtns);
                break;
            case 3:
                InactiveGroup(unitBtns);
                InactiveGroup(upgradeBtns);
                ActiveGroup(shopBtns);
                break;
        }
    }

    public void InactiveGroup(CanvasGroup cg)
    {
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;
    }

    public void ActiveGroup(CanvasGroup cg)
    {
        cg.alpha = 1f;
        cg.blocksRaycasts = true;
        cg.interactable = true;
    }
}
