using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 인벤토리를 구현할 때 배열이 아닌 리스트로 한 이유
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


    // 인벤토리에서 itemDetails에서 가져올 정보를 isStackable밖에 없음
    // isStackable만을 가져오기 위해 매번 GameDatas.itemDetailsList.Find를 하는 것을 비효율적임
    // >> isStackable이 true인 itemCode를 List에 저장한 후 contains를 이용해 isStackable 확인
    List<int> stackableItemCode;
    int maxQuantity = 0;

    // 아이템 코드로 아이템 디테일에 쉽게 접근하기 위해 사용
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

        
        // Json 형식으로 저장된 인벤토리 정보를 불러와 Inventory에 저장           
        DataManager.Instance.LoadInventory();
    }

    
    #endregion

    #region Add Item

    public bool AddItem(Inventories type, int itemCode, int quantity) => AddItem(type, itemCode, ref quantity);

    public bool AddItem(Inventories type, int itemCode, ref int quantity)
    {
        // 추가하려는 아이템 정보가 있는지 확인
        if (!itemDic.ContainsKey(itemCode))
        {
            Debug.Log("아이템 정보가 없습니다");
            return false;
        }


        // 빈 슬롯이 있는지 검색
        int idx = FindFirstItemSlotIndex(type);

        // 아이템 중복이 불가능한 경우
        if (!stackableItemCode.Contains(itemCode))
        {
            // 빈 슬롯이 없는 경우
            if (idx<0)
            {
                Debug.Log("빈 슬롯이 없습니다");
                return false;
            }
            // 빈 슬롯이 있는 경우
            else
            {
                //Debug.Log("빈 슬롯에 아이템 추가 성공");
                AddNewItem(type, idx, itemCode);
            }

        }
        // 아이템 중복이 가능한 경우
        else
        {
            // itemCode가 들어있는 모든 슬롯 검색
            List<int> idxs = FindAllItemSlotIndexs(type, itemCode);

            // itemCode가 들어있는 슬롯이 없는 경우
            if(idxs==null)
            {
                // 빈 슬롯이 없는 경우
                if(idx<0)
                {
                    EventHandler.CallPrintSystemMassageEvent("가방이 가득 찼습니다");
                    return false;
                }
                // 빈 슬롯이 있는 경우
                {
                    AddNewItem(type, idx, itemCode, quantity);
                }
            }
            // itemCode가 들어있는 슬롯이 있는 경우
            else
            {
                int i = 0;
                // quantity가 0 이하가 될 때 까지 반복
                while (quantity > 0)
                {
                    // idxs의 인덱스가 idxs의 크기보다 클 경우 중지
                    if (i >= idxs.Count)
                        break;

                    // idxs[i]번 째 슬롯에 넣을 아이템의 개수 구하기
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
                        EventHandler.CallPrintSystemMassageEvent("가방이 가득 찼습니다");
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

        // 예외처리
        // 아이템 중복이 안 되는 경우 quantity를 1로 저장
        if (!item.stackable) item.quantity = 1;
        // 새로 추가하려는 아이템의 개수가 maxQuantity보다 큰 경우 아이템 개수를 maxQuantity로 저장
        else item.quantity = quantity > Settings.Instance.maxQuantity ? Settings.Instance.maxQuantity : quantity;

        // 빈 슬롯에 아이템 추가    
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
            Debug.Log("아이템 정보가 없습니다");
            return false;
        }

        int idx = FindFirstItemSlotIndex(type, itemCode);
        if (idx < 0)
        {
            Debug.Log("인벤토리에 아이템이 없습니다");
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
        // 예외처리
        if (GameDatas.inventories[_type][originIdx].itemCode <= 0) { Debug.Log("빈 슬롯"); return false; }
        if (originIdx == targetIdx) { Debug.Log("같은 슬롯"); return false; }
        if (targetIdx >= GameDatas.inventories[_type].Count) { Debug.Log("인벤토리 범위를 벗어남"); return false; }

        bool moveAll = true;

        // 옮길 아이템 개수 예외처리
        if (GameDatas.inventories[_type][originIdx].stackable)
        {
            if (quantity >= GameDatas.inventories[_type][originIdx].quantity) 
                quantity = GameDatas.inventories[_type][originIdx].quantity;
            else
                moveAll = false;
        }
        else quantity = 1;

        // 아이템을 빈 슬롯으로 이동하는 경우
        if (GameDatas.inventories[_type][targetIdx].itemCode <= 0)
        {
            // 아이템 전부를 이동하는 경우
            if (moveAll) MoveAll(type, originIdx, targetIdx);
            else MovePart(type, originIdx, targetIdx, quantity);
        }
        else
        {
            // 서로 다른 아이템을 바꿀 경우
            if (GameDatas.inventories[_type][originIdx].itemCode != GameDatas.inventories[_type][targetIdx].itemCode)
            {
                MoveAll(type, originIdx, targetIdx);
            }
            // 같은 아이템을 바꿀 경우
            else
            {
                if (GameDatas.inventories[_type][originIdx].stackable)
                {
                    int _qunatity = GameDatas.inventories[_type][targetIdx].quantity + quantity > maxQuantity ? maxQuantity - GameDatas.inventories[_type][targetIdx].quantity : quantity;

                    // Bag에서 씨앗을 옮길 경우
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

    #region Shift Item(아이템 인벤토리 이동)

    public void ShiftItem(Vector2Int from, Inventories toInventory, int toIndex, int quantity = 1)
    {
        // 1. to가 비어있는 경우 : quantity만큼 to로 이동
        if (IsEmpty(toInventory, toIndex))
        {
            AddNewItem(toInventory, toIndex, GameDatas.inventories[from.x][from.y].itemCode, quantity);
            RemoveItemAtIndex((Inventories)from.x, quantity, from.y);
        }
        // 2. to가 비어있지 않는 경우
        else
        {
            InventoryItem _item = GameDatas.inventories[from.x][from.y];
            // 2-1. to의 아이템과 from의 아이템이 다른 경우 : from과 to의 위치 변경
            if (!CompareItemWithIndex((Inventories)from.x, from.y, toInventory, toIndex))
            {
                GameDatas.inventories[from.x][from.y] = GameDatas.inventories[(int)toInventory][toIndex];
                GameDatas.inventories[(int)toInventory][toIndex] = _item;
            }
            // 2-2. to의 아이템과 from의 아이템이 같은 경우 : quantity와 옮길 최대 개수를 비교해 이동
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
    /// type 인벤토리에서 itemCode가 들어있는 가장 낮은 인덱스 값을 반환
    /// itemCode가 0일 경우 빈 슬롯의 인덱스를 반환
    /// </summary>
    /// <param name="type"></param>
    /// <param name="itemCode"></param>
    /// <returns></returns>
    int FindFirstItemSlotIndex(Inventories type, int itemCode = 0)=> GameDatas.inventories[(int)type].FindIndex(x => x.itemCode == itemCode);
    List<int> FindAllItemSlotIndexs(Inventories type, int itemCode = 0)
    {
        int inven = (int)type;

        // 만약 인벤토리에 ItemCode가 들어있는 슬롯이 없을 경우 null 리턴
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
