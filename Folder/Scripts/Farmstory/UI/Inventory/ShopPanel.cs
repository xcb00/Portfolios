using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPanel : ItemPanel
{
    int shopItemCnt;
    List<int> selectItem;
    [SerializeField] RectTransform shop;
    [SerializeField] RectTransform inven;
    [SerializeField] protected ShopCountPanel countPanel;
    Slot[] shopSlot;
    Slot[] invenSlot;

    void Start()
    {
        Init();
        shopItemCnt = Settings.Instance.shopItemCount;
        selectItem = new List<int>();
        shopSlot = shop.GetComponentsInChildren<Slot>();
        invenSlot = inven.GetComponentsInChildren<Slot>();

        for (int i = 0; i < shopSlot.Length; i++) shopSlot[i].SetPanel(panelType);
        for (int i = 0; i < invenSlot.Length; i++) invenSlot[i].SetPanel(panelType);

    }

    private void OnEnable()
    {
        EventHandler.DragItemActiveEvent += ClickSlot;
    }
    private void OnDisable()
    {
        EventHandler.DragItemActiveEvent -= ClickSlot;
    }


    public override void ActivePanel()
    {
        if (Utility.CompareDay(GameDatas.VisitShop))
            RenewalItem();

        SetInventory();

        base.ActivePanel();
    }

    void RenewalItem()
    {
        // 중복된 아이템은 팔 지 않음
        selectItem.Clear();
        int cnt = shopItemCnt > GameDatas.shopItemList.Count ? GameDatas.shopItemList.Count : shopItemCnt;
        int idx = 0;
        int shopIdx = (int)Inventories.shop;

        for (int i = 0; i < cnt; i++)
        {
            InventoryItem item = new InventoryItem();
            GameDatas.inventories[shopIdx][i] = item;

            for (int j = 0; j < GameDatas.shopItemList.Count; j++)
            {
                //if (!selectItem.Contains(GameDatas.shopItemList[j].itemCode) && Utility.Random(Settings.Instance.shopItemRarity[GameDatas.shopItemList[j].rarity], 100))
                if (!selectItem.Contains(GameDatas.shopItemList[j].itemCode) && RandomItem(GameDatas.shopItemList[j].rarity))
                {
                    int code = GameDatas.shopItemList[j].itemCode;
                    //Debug.Log($"{GameDatas.itemDetailsList.Find(x => x.code == code).name} / {GameDatas.itemDetailsList.Find(x => x.code == code).type}");
                    item.itemCode = code;
                    item.stackable = InventoryManager.Instance.GetItemStackableWithItemCode(code);

                    if (!item.stackable) item.quantity = 1;
                    else if (GameDatas.shopItemList[j].min != GameDatas.shopItemList[j].max) item.quantity = Random.Range(GameDatas.shopItemList[j].min, GameDatas.shopItemList[j].max);
                    else item.quantity = GameDatas.shopItemList[j].min;

                    selectItem.Add(GameDatas.shopItemList[j].itemCode);
                    GameDatas.inventories[shopIdx][idx++] = item;
                    break;
                }
            }
        }
        DataManager.Instance.SaveInventoryData(shopIdx);
        GameDatas.VisitShop = Utility.GetDayData();
        DataManager.Instance.SaveTimeData();
    }

    protected override void SetInventory()
    {
        for (int i = 0; i < shopSlot.Length; i++)
        {
            if (InventoryManager.Instance.IsEmpty(Inventories.shop, i)) shopSlot[i].EmptySet();
            else shopSlot[i].ItemSet();
        }

        for(int i = 0; i < invenSlot.Length; i++)
        {
            if (InventoryManager.Instance.IsEmpty(Inventories.bag, i)) invenSlot[i].EmptySet();
            else invenSlot[i].ItemSet();
        }
    }

    void ClickSlot(Inventories panel, Inventories slot, int index)
    {
        if (panel != panelType) return;

        if (InventoryManager.Instance.IsEmpty(slot, index))
        {
            Debug.Log("Empty slot");
            return;
        }
        
        countPanel.ActivePanel(slot, index, InventoryManager.Instance.GetPirceWithIndex(slot, index), InventoryManager.Instance.GetItemQuantityWithIndex(slot, index));
    }

    public void TradeItem(Inventories slot, int index, int quantity)
    {
        if (slot == Inventories.bag) 
            InventoryManager.Instance.RemoveItemAtIndex(slot, quantity, index);
        else
        {
            int origin = quantity;
            InventoryManager.Instance.AddItem(Inventories.bag, GameDatas.inventories[(int)slot][index].itemCode, ref quantity);
            InventoryManager.Instance.RemoveItemAtIndex(slot, origin - quantity, index);

            if (quantity > 0)
                Utility.GoldChange(InventoryManager.Instance.GetPirceWithIndex(slot, index) * quantity);
        }

        SetInventory();
        DataManager.Instance.SaveInventoryData((int)Inventories.bag);
        DataManager.Instance.SaveInventoryData((int)Inventories.shop);
        DataManager.Instance.SaveTimeData();
    }

    bool RandomItem(int grade)
    {
        int percentage = 0;
        for (int i = grade; i < (int)Grade.count; i++)
            percentage += GameDatas.gradePercentage[i];

        return Random.Range(1, 101) <= percentage;
    }
}
