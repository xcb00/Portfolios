using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountPanel : Panel
{
    public int swapCnt { get; private set; }
    Vector2Int dragItemIndex;
    [SerializeField] TextMeshProUGUI nameTxt;
    [SerializeField] TextMeshProUGUI count;
    [SerializeField] Slider sliderBar;
    [SerializeField] InventoryPanel inventory;

    private void Start()
    {
        Init();
    }

    public void ActivePanel(Vector2Int dragItemIndex, int max)
    {
        this.dragItemIndex = dragItemIndex;
        if(nameTxt!=null)
            nameTxt.SetText(InventoryManager.Instance.GetNameWithIndex(dragItemIndex));
        sliderBar.maxValue = max;
        sliderBar.value = 1;
        count.SetText("1");
        ActivePanel();
    }

    public void ValueChange()
    {
        swapCnt = Mathf.RoundToInt(sliderBar.value);
        sliderBar.value = swapCnt;
        count.SetText(swapCnt.ToString());
    }

    public void Confirm()
    {
        inventory.SwapItem(dragItemIndex, swapCnt);
        InactivePanel();
    }
}
