using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    TextMeshProUGUI numberTxt = null;

    public void Initialise(int num)
    {
        if (numberTxt == null) numberTxt = GetComponentInChildren<TextMeshProUGUI>();

        transform.localPosition = Vector3.zero;
        numberTxt.SetText(num.ToString());
        gameObject.SetActive(true);
    }
}
