using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : ItemPanel
{
    [SerializeField] protected Image dragItem;
    [SerializeField] protected CountPanel countPanel;
    public int dragItemCode { get; protected set; }
    protected Vector2Int noneSelect = new Vector2Int(-1, -1);
    protected Vector2Int dragItemIndex;
    Vector2Int dropItemIndex;


    //int dragInventories;

    public void InitDragItemIndex() {dragItemIndex = noneSelect; dragItemCode = 0; }


    // CountPanel을 통해 Swap할 때
    public virtual void SwapItem(Vector2Int dragIdx, int quantity)
    {
        if (dragIdx.x != dropItemIndex.x)
            InventoryManager.Instance.ShiftItem(dragIdx, (Inventories)dropItemIndex.x, dropItemIndex.y, quantity);
        else
        {
            InventoryManager.Instance.SwapItem((Inventories)dropItemIndex.x, dragIdx.y, dropItemIndex.y, quantity);
            //SetInventory((Inventories)dropItemIndex.x);
        }

        /*for(int i = 0; i < GameDatas.inventories[0].Count; i++)
        {
            if (GameDatas.inventories[0][i].quantity > 0)
                Debug.Log($"{i} : {GameDatas.inventories[0][i].itemCode}");
        }*/

        SetInventory();
    }

    #region Drop Item
    protected void ActiveDragItem(Inventories panel, Inventories type, int index)
    {
        if (panel != panelType) return;

        dragItemIndex = new Vector2Int((int)type, index);
        //dragInventories = dragItemIndex.x;
        dragItemCode = GameDatas.inventories[dragItemIndex.x][dragItemIndex.y].itemCode;
        dragItem.sprite = InventoryManager.Instance.GetItemSpriteWithIndex((Inventories)dragItemIndex.x, dragItemIndex.y);
        dragItem.enabled = true;
        StartCoroutine(FollowingCursor());
    }

    protected void InactiveDragItem()
    {
        StopAllCoroutines();
        InitDragItemIndex();
        dragItem.enabled = false;
        dragItemCode = -1;
        dragItem.sprite = null;
    }

    protected void DropItem(Inventories panel, Inventories type, int index)
    {
        if (panel != panelType) return;

        if (dragItemIndex.x < 0)
            return;

        if (GameDatas.inventories[dragItemIndex.x][dragItemIndex.y].quantity > 1)
        {
            if (GameDatas.inventories[(int)type][index].itemCode <= 0 || GameDatas.inventories[(int)type][index].itemCode == GameDatas.inventories[dragItemIndex.x][dragItemIndex.y].itemCode)
            {
                if (GameDatas.inventories[(int)type][index].quantity >= maxQuantity) return;
                dropItemIndex = new Vector2Int((int)type, index);
                countPanel.ActivePanel(dragItemIndex, GameDatas.inventories[dragItemIndex.x][dragItemIndex.y].quantity);
            }
            else
                SwapItem(type, index);
        }
        else
            SwapItem(type, index);
    }

    void SwapItem(Inventories type, int index)
    {
        if ((int)type != dragItemIndex.x)
            InventoryManager.Instance.ShiftItem(dragItemIndex, type, index);
        else
        {
            InventoryManager.Instance.SwapItem(type, dragItemIndex.y, index, 1);
        }
        //SetInventory(type);
        SetInventory();
    }

    protected IEnumerator FollowingCursor()
    {
        while (dragItem.enabled)
        {
            dragItem.rectTransform.position = Input.mousePosition;
            yield return null;
        }
    }
    #endregion
}
