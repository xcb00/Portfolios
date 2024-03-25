using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemMassage : MonoBehaviour
{
    Animator myAnimator = null;
    RectTransform systemMassageBox = null;
    RectTransform rect = null;
    Text systemMasageTxt = null;

    private void OnEnable()
    {
        myAnimator = GetComponentInParent<Animator>();
        systemMassageBox = transform.parent.GetComponent<RectTransform>();
        rect = GetComponent<RectTransform>();
        systemMasageTxt = GetComponent<Text>();

        EventHandler.PrintSystemMassageEvent += PrintMsg;
    }

    private void OnDisable()
    {
        EventHandler.PrintSystemMassageEvent -= PrintMsg;
    }

    void PrintMsg(string msg)
    {
        systemMasageTxt.text = msg;
        Canvas.ForceUpdateCanvases();
        systemMassageBox.sizeDelta = new Vector2(rect.rect.width, rect.rect.height);
        myAnimator.SetTrigger("Active");
    }
}
