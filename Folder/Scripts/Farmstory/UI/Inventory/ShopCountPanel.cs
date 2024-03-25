using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class ShopCountPanel : Panel
{
    [SerializeField] TextMeshProUGUI nameTxt;
    [SerializeField] TextMeshProUGUI countTxt;
    [SerializeField] TextMeshProUGUI priceTxt;
    [SerializeField] Slider sliderBar;
    [SerializeField] ShopPanel shop;
    [SerializeField] Color sell;
    [SerializeField] Color buy;
    StringBuilder sb;
    int price;
    int swapCnt;
    int index;
    Inventories type;

    private void Start()
    {
        Init();
        sb = new StringBuilder();
    }

    public void ActivePanel(Inventories type, int index, int price, int max)
    {
        sliderBar.maxValue = max;
        sliderBar.value = 1;
        sliderBar.gameObject.SetActive(max != 1);
        nameTxt.SetText(InventoryManager.Instance.GetNameWithIndex(new Vector2Int((int)type, index)));
        swapCnt = 1;
        this.type = type;
        this.index = index;
        this.price = price;
        priceTxt.color = type == Inventories.bag ? sell : buy;
        GetPrice();
        ActivePanel();
    }

    public void ValueChange()
    {
        swapCnt = Mathf.RoundToInt(sliderBar.value);
        sliderBar.value = swapCnt;
        GetPrice();
    }

    public void Confirm()
    {
        if (type == Inventories.bag) SellItem();
        else BuyItem();
        InactivePanel();
    }

    void BuyItem()
    {
        if (!Utility.GoldChange(-price * swapCnt)) return;
        shop.TradeItem(type, index, swapCnt);
    }

    void SellItem()
    {
        Utility.GoldChange(price * swapCnt);
        shop.TradeItem(type, index, swapCnt);
    }

    void GetPrice()
    {
        int _price = type == Inventories.bag ? Mathf.RoundToInt(swapCnt * price * 0.8f) : swapCnt * price;
        sb.Clear();
        sb.Append(type == Inventories.bag ? "+" : "-");
        sb.Append(_price.ToString("N0"));
        sb.Append("G");
        countTxt.SetText(swapCnt.ToString());
        priceTxt.SetText(sb.ToString());
    }
}
