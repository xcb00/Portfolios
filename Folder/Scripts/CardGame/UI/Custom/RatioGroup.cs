using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RatioGroup : MonoBehaviour
{
    public int gold;
    ToggleGroup toggleGroup;
    Toggle[] toggles;

    private void OnEnable()
    {
        toggleGroup = GetComponent<ToggleGroup>(); 
        toggles = GetComponentsInChildren<Toggle>();
        for (int i = 0; i < toggles.Length; i++)
            toggles[i].isOn = i == 0;
    }
}
