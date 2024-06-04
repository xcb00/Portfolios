using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RatioButton : MonoBehaviour
{
    public UnityEvent<int> ChangeRatio;
    int idx = -1;

    private void OnEnable()
    {
        if (idx >= 0)
            return;

        idx = transform.GetSiblingIndex();
        transform.GetComponent<Toggle>().onValueChanged.AddListener((isOn) =>
        {
            if(isOn)
                ChangeRatio?.Invoke(idx);
        });
    }
}
