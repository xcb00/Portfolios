using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PurchasedDic
{
    public ProductID id;
    public GameObject purchased;
}

public class MarketPanel : Panel
{
    [SerializeField] List<PurchasedDic> purchasedList;

    void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        EventHandler.AlreadyPurchasedProductEvent += AlreadyPurchased;
    }


    void AlreadyPurchased(ProductID id)
    {
        GameObject obj = purchasedList.Find(x=>x.id==id).purchased;
        if (obj != null && !obj.activeSelf)
            obj.SetActive(true);
    }

    public override void ActivePanel()
    {
        EventHandler.CallOpenMarketEvent();
        base.ActivePanel();
    }

    public override void InactivePanel()
    {
        base.InactivePanel();
    }

    public void BuyButton(int productID)
    {
        EventHandler.CallPurchaseProductEvent((ProductID)productID);
    }
}
