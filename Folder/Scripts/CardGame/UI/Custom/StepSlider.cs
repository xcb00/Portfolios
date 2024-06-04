using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StepSlider : MonoBehaviour
{
    [SerializeField] UnityEvent<bool> CanPlaying;
    [SerializeField] TextMeshProUGUI goldTxt;
    Slider slider = null;
    int currentValue = 1;
    int step = 1;
    
    void OnEnable()
    {
        if(slider==null)
            slider = GetComponent<Slider>();
        ChangeStep(0);
    }

    public void ChangeStep(int idx)
    {
        step = Mathf.RoundToInt(Mathf.Pow(10f, idx + 1));
        float max = GameManager.Inst.gold / step;
        if (max < 1f)
        {
            currentValue = -1;
            goldTxt.SetText("Not enough gold");
            CanPlaying?.Invoke(false);
            return;
        }

        CanPlaying?.Invoke(true);
        slider.maxValue = Mathf.FloorToInt(max);
        slider.minValue = 1;
        currentValue = 1;
        slider.value = 1;
        goldTxt.SetText((step).ToString("N0"));
    }

    public void OnValueChange(float value)
    {
        if (currentValue < 0) return;

        int _value = Mathf.RoundToInt(value);
        slider.value = _value;

        if (_value == currentValue) return;

        currentValue = _value;
        goldTxt.SetText((currentValue * step).ToString("N0"));
    }

    public void StartPlaying() => GameManager.Inst.StartPlaying(currentValue * step);
}
