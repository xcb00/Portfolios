using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �κ��丮�� ������ �� �迭�� �ƴ� ����Ʈ�� �� ����
// 

public class InventoryManager : Singleton<InventoryManager>
{
#if UNITY_EDITOR
    public enum InvenFunc { add, remove, swap}
    [Header("Unity Editor Only")]
    public int addItemCode;
    public int addItemQuantity;
    public int _origin;
    public int _target;
    public Inventories type;
    public InvenFunc func;
#endif


    // �κ��丮���� itemDetails���� ������ ������ isStackable�ۿ� ����
    // isStackable���� �������� ���� �Ź� GameDatas.itemDetailsList.Find�� �ϴ� ���� ��ȿ������
    // >> isStackable�� true�� itemCode�� List�� ������ �� contains�� �̿��� isStackable Ȯ��
    List<int> stackableItemCode;
    int maxQuantity = 0;

    // ������ �ڵ�� ������ �����Ͽ� ���� �����ϱ� ���� ���
    Dictionary<int, ItemDetails> itemDic;

    #region Load Inventory
    private void OnEnable()
    {
        EventHandler.DBDataLoadEvent += LoadInventory;
    }

    private void OnDisable()
    {
        EventHandler.DBDataLoadEvent -= LoadInventory;
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            TestFunc();
    }
#endif

    void LoadInventory()
    {
        maxQuantity = Settings.Instance.maxQuantity;



        itemDic = new Dictionary<int, ItemDetails>();
        foreach (ItemDetails item in GameDatas.itemDetailsList)
            itemDic.Add(item.code, item);

        stackableItemCode = new List<int>();
        foreach (ItemDetails details in GameDatas.itemDetailsList)
            if (details.isStackable) stackableItemCode.Add(details.code);

        /*Debug.Log("GgameDatas.itemDetailsList");
        foreach (ItemDetails item in GameDatas.itemDetailsList)
            Debug.Log($"{item.name} : {item.code}");
        Debug.Log("ItemDic");
        foreach (KeyValuePair<int, ItemDetails> pair in itemDic)
            Debug.Log($"{pair.Value.name} : {pair.Key}");*/

        
        // Json �������� ����� �κ��丮 ������ �ҷ��� Inventory�� ����           
        DataManager.Instance.LoadInventory();
    }

    
    #endregion

    #region Add Item

    public bool AddItem(Inventories type, int itemCode, int quantity) => AddItem(type, itemCode, ref quantity);

    public bool AddItem(Inventories type, int itemCode, ref int quantity)
    {
        // �߰��Ϸ��� ������ ������ �ִ��� Ȯ��
        if (!itemDic.ContainsKey(itemCode))
        {
            Debug.Log("������ ������ �����ϴ�");
            return false;
        }


        // �� ������ �ִ��� �˻�
        int idx = FindFirstItemSlotIndex(type);

        // ������ �ߺ��� �Ұ����� ���
        if (!stackableItemCode.Contains(itemCode))
        {
            // �� ������ ���� ���
            if (idx<0)
            {
                Debug.Log("�� ������ �����ϴ�");
                return false;
            }
            // �� ������ �ִ� ���
            else
            {
                //Debug.Log("�� ���Կ� ������ �߰� ����");
                AddNewItem(type, idx, itemCode);
            }

        }
        // ������ �ߺ��� ������ ���
        else
        {
            // itemCode�� ����ִ� ��� ���� �˻�
            List<int> idxs = FindAllItemSlotIndexs(type, itemCode);

            // itemCode�� ����ִ� ������ ���� ���
            if(idxs==null)
            {
                // �� ������ ���� ���
                if(idx<0)
                {
                    EventHandler.CallPrintSystemMassageEvent("������ ���� á���ϴ�");
                    return false;
                }
                // �� ������ �ִ� ���
                {
                    AddNewItem(type, idx, itemCode, quantity);
                }
            }
            // itemCode�� ����ִ� ������ �ִ� ���
            else
            {
                int i = 0;
                // quantity�� 0 ���ϰ� �� �� ���� �ݺ�
                while (quantity > 0)
                {
                    // idxs�� �ε����� idxs�� ũ�⺸�� Ŭ ��� ����
                    if (i >= idxs.Count)
                        break;

                    // idxs[i]�� ° ���Կ� ���� �������� ���� ���ϱ�
                    int _quantity = maxQuantity - GameDatas.inventories[(int)type][idxs[i]].quantity;// + quantity);

                    if (_quantity >= quantity)
                    {
                        AddItemQuantity(type, idxs[i], quantity);
                        quantity = 0;
                        break;
                    }
                    else
                    {
                        AddItemQuantity(type, idxs[i++], _quantity);
                        quantity -= _quantity;
                    }
                }

                if(quantity>0)
                {
                    if (idx < 0)
                    {
                        EventHandler.CallPrintSystemMassageEvent("������ ���� á���ϴ�");
                        return false;
                    }
                    else
                        AddNewItem(type, idx, itemCode, quantity);
                }    
            }
        }
        quantity = 0;
        return true;
    }

    void AddNewItem(Inventories type, int slotIdx, int itemCode, int quantity = 1)
    {
        InventoryItem item = new InventoryItem();
        item.itemCode = itemCode;
        item.stackable = stackableItemCode.Contains(itemCode);

        // ����ó��
        // ������ �ߺ��� �� �Ǵ� ��� quantity�� 1�� ����
        if (!item.stackable) item.quantity = 1;
        // ���� �߰��Ϸ��� �������� ������ maxQuantity���� ū ��� ������ ������ maxQuantity�� ����
        else item.quantity = quantity > Settings.Instance.maxQuantity ? Settings.Instance.maxQuantity : quantity;

        // �� ���Կ� ������ �߰�    
        GameDatas.inventories[(int)type][slotIdx] = item;
    }

    void AddItemQuantity(Inventories type, int index, int quantity)
    {
        InventoryItem item = GameDatas.inventories[(int)type][index];
        item.quantity += quantity;
        GameDatas.inventories[(int)type][index] = item;
    }
    #endregion

    #region Remove Item
    public bool RemoveItem(Inventories type, int itemCode, int quantity = 1)
    {
        if (!itemDic.ContainsKey(itemCode))
        {
            Debug.Log("������ ������ �����ϴ�");
            return false;
        }

        int idx = FindFirstItemSlotIndex(type, itemCode);
        if (idx < 0)
        {
            Debug.Log("�κ��丮�� �������� �����ϴ�");
            return false;
        }

        RemoveItemAtIndex(type, quantity, idx);

        return true;
    }

    public void RemoveItemAtIndex(Inventories type, int quantity, int index)
    {
        if (GameDatas.inventories[(int)type][index].quantity > quantity)
        {
            AddItemQuantity(type, index, -quantity);
        }
        else
        {
            DataManager.Instance.InitInventoryItem(type, index);
        }
    }

    public void RemoveItemForDIY(DIYMaterial material)
    {
        int code = material.materialCode;
        int quantity = material.materialCount;
        List<int> itemIdxs = new List<int>();
        for(int i = 0; i < GameDatas.inventories[0].Count; i++)
        {
            if (GameDatas.inventories[0][i].itemCode == code) itemIdxs.Add(i);
        }

        //foreach (int i in itemIdxs) Debug.Log(i);

        int idx = 0;
        while (quantity > 0)
        {
            int _idx = itemIdxs[idx++];
            if (quantity <= GameDatas.inventories[0][_idx].quantity)
            {
                RemoveItemAtIndex(Inventories.bag, quantity, _idx);
                break;
            }
            else
            {
                quantity -= GameDatas.inventories[0][_idx].quantity;
                RemoveItemAtIndex(Inventories.bag, GameDatas.inventories[0][_idx].quantity, _idx);
            }
        }
    }
    #endregion

    #region Swap Item
    public bool SwapItem(Inventories type, int originIdx, int targetIdx, int quantity = 1)
    {
        int _type = (int)type;
        // ����ó��
        if (GameDatas.inventories[_type][originIdx].itemCode <= 0) { Debug.Log("�� ����"); return false; }
        if (originIdx == targetIdx) { Debug.Log("���� ����"); return false; }
        if (targetIdx >= GameDatas.inventories[_type].Count) { Debug.Log("�κ��丮 ������ ���"); return false; }

        bool moveAll = true;

        // �ű� ������ ���� ����ó��
        if (GameDatas.inventories[_type][originIdx].stackable)
        {
            if (quantity >= GameDatas.inventories[_type][originIdx].quantity) 
                quantity = GameDatas.inventories[_type][originIdx].quantity;
            else
                moveAll = false;
        }
        else quantity = 1;

        // �������� �� �������� �̵��ϴ� ���
        if (GameDatas.inventories[_type][targetIdx].itemCode <= 0)
        {
            // ������ ���θ� �̵��ϴ� ���
            if (moveAll) MoveAll(type, originIdx, targetIdx);
            else MovePart(type, originIdx, targetIdx, quantity);
        }
        else
        {
            // ���� �ٸ� �������� �ٲ� ���
            if (GameDatas.inventories[_type][originIdx].itemCode != GameDatas.inventories[_type][targetIdx].itemCode)
            {
                MoveAll(type, originIdx, targetIdx);
            }
            // ���� �������� �ٲ� ���
            else
            {
                if (GameDatas.inventories[_type][originIdx].stackable)
                {
                    int _qunatity = GameDatas.inventories[_type][targetIdx].quantity + quantity > maxQuantity ? maxQuantity - GameDatas.inventories[_type][targetIdx].quantity : quantity;

                    // Bag���� ������ �ű� ���
                    if (moveAll && type == Inventories.bag && GetCanCroppedWithIndex(type, originIdx))
                        EventHandler.CallActiveSeedMoveEvent(originIdx, targetIdx);

                    RemoveItemAtIndex(type, _qunatity, originIdx);
                    AddItemQuantity(type, targetIdx, _qunatity);

                }
            }
        }

        return true;
    }

    void MoveAll(Inventories type, int origin, int target)
    {
        InventoryItem item = GameDatas.inventories[(int)type][target];
        GameDatas.inventories[(int)type][target] = GameDatas.inventories[(int)type][origin];
        GameDatas.inventories[(int)type][origin] = item;

        if (type == Inventories.bag /*&& GetCanCroppedWithIndex(type, target)*/)
        {
            EventHandler.CallActiveSeedMoveEvent(origin, target);
        }
    }

    void MovePart(Inventories type, int origin, int target, int quantity)
    {
        AddItemQuantity(type, origin, -quantity);
        AddNewItem(type, target, GameDatas.inventories[(int)type][origin].itemCode, quantity);
    }
    #endregion

    #region Shift Item(������ �κ��丮 �̵�)

    public void ShiftItem(Vector2Int from, Inventories toInventory, int toIndex, int quantity = 1)
    {
        // 1. to�� ����ִ� ��� : quantity��ŭ to�� �̵�
        if (IsEmpty(toInventory, toIndex))
        {
            AddNewItem(toInventory, toIndex, GameDatas.inventories[from.x][from.y].itemCode, quantity);
            RemoveItemAtIndex((Inventories)from.x, quantity, from.y);
        }
        // 2. to�� ������� �ʴ� ���
        else
        {
            InventoryItem _item = GameDatas.inventories[from.x][from.y];
            // 2-1. to�� �����۰� from�� �������� �ٸ� ��� : from�� to�� ��ġ ����
            if (!CompareItemWithIndex((Inventories)from.x, from.y, toInventory, toIndex))
            {
                GameDatas.inventories[from.x][from.y] = GameDatas.inventories[(int)toInventory][toIndex];
                GameDatas.inventories[(int)toInventory][toIndex] = _item;
            }
            // 2-2. to�� �����۰� from�� �������� ���� ��� : quantity�� �ű� �ִ� ������ ���� �̵�
            else
            {
                if (_item.stackable)
                {
                    int _qunatity = GameDatas.inventories[(int)toInventory][toIndex].quantity + quantity > maxQuantity ? maxQuantity - GameDatas.inventories[(int)toInventory][toIndex].quantity : quantity;

                    RemoveItemAtIndex((Inventories)from.x, _qunatity, from.y);
                    AddItemQuantity(toInventory, toIndex, _qunatity);
                }
            }
        }
    }
    #endregion

    #region Inventory Function


    /// <summary>
    /// type �κ��丮���� itemCode�� ����ִ� ���� ���� �ε��� ���� ��ȯ
    /// itemCode�� 0�� ��� �� ������ �ε����� ��ȯ
    /// </summary>
    /// <param name="type"></param>
    /// <param name="itemCode"></param>
    /// <returns></returns>
    int FindFirstItemSlotIndex(Inventories type, int itemCode = 0)=> GameDatas.inventories[(int)type].FindIndex(x => x.itemCode == itemCode);
    List<int> FindAllItemSlotIndexs(Inventories type, int itemCode = 0)
    {
        int inven = (int)type;

        // ���� �κ��丮�� ItemCode�� ����ִ� ������ ���� ��� null ����
        if (FindFirstItemSlotIndex(type, itemCode) < 0) return null;

        List<int> result = new List<int>();
        for (int i = 0; i < GameDatas.inventories[inven].Count; i++)
            if (GameDatas.inventories[inven][i].itemCode == itemCode) result.Add(i);
        return result;
    }

#if UNITY_EDITOR
    void TestFunc()
    {
        switch (func)
        {
            case InvenFunc.add: AddItem(type, addItemCode, ref addItemQuantity); break;
            case InvenFunc.remove: RemoveItem(type, addItemCode, addItemQuantity); break;
            case InvenFunc.swap: SwapItem(type, _origin, _target, addItemQuantity); break;
        }
        //PrintInventories((int)type);
    }

    void PrintInventories(int index)
    {
        foreach (InventoryItem item in GameDatas.inventories[index])
            Debug.Log($"{item.itemCode} : {item.quantity} ({item.stackable})");

        DataManager.Instance.SaveInventoryData(0);
    }
#endif
    #endregion

    #region GetItemDetail
    public string GetNameWithCode(int code) => itemDic[code].name;
    public string GetNameWithIndex(Vector2Int idx) => GetNameWithCode(GameDatas.inventories[idx.x][idx.y].itemCode);
    public Sprite GetItemSpriteWithItemCode(int code) => itemDic[code].sprite;
    public bool GetItemStackableWithItemCode(int code) => itemDic[code].isStackable;

    public int GetInventoryCount(Inventories type) => GameDatas.inventories[(int)type].Count;

    public Sprite GetItemSpriteWithIndex(Inventories type, int index) => GetItemSpriteWithItemCode(GameDatas.inventories[(int)type][index].itemCode);
    //public Sprite GetItemSpriteWithVector2Index(Vector2Int index) => GetItemSpriteWithItemCode(GameDatas.inventories[index.x][index.y].itemCode);
    public int GetItemQuantityWithIndex(Inventories type, int index) => GameDatas.inventories[(int)type][index].quantity;

    public int GetItemQuantityInInventory(Inventories type, int itemCode)
    {
        int quantity = 0;
        int invenIdx = (int)type;
        for(int i = 0; i < GameDatas.inventories[invenIdx].Count; i++)
        {
            if (GameDatas.inventories[invenIdx][i].itemCode == itemCode)
                quantity += GameDatas.inventories[invenIdx][i].quantity;
        }

        return quantity;
    }

    public bool IsEmpty(Inventories type, int index) => GameDatas.inventories[(int)type][index].itemCode <= 0;

    public bool GetCanCropped(int code) => itemDic[code].canCrop;

    public bool GetCanCroppedWithIndex(Inventories type, int index)
    {
        int idx = GameDatas.inventories[(int)type][index].itemCode;
        if (idx <= 0) return false;
        else return itemDic[idx].canCrop;
    }

    public int GetPirceWithIndex(Inventories type, int index) => itemDic[GameDatas.inventories[(int)type][index].itemCode].price;

    public bool CompareItemWithIndex(Inventories fromType, int fromIdx, Inventories toType, int toIdx)
        => GameDatas.inventories[(int)fromType][fromIdx].itemCode == GameDatas.inventories[(int)toType][toIdx].itemCode;

    #endregion
}
