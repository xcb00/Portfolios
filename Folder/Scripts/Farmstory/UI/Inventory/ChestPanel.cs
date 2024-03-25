using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestPanel : InventoryPanel
{
    [SerializeField] Transform chest;
    [SerializeField] Transform inventory;
    Slot[] chestSlots;
    Slot[] inventorySlots;

    private void Start()
    {
        chestSlots = chest.GetComponentsInChildren<Slot>();
        inventorySlots=inventory.GetComponentsInChildren<Slot>();
        for (int i = 0; i < chestSlots.Length; i++) chestSlots[i].SetPanel(panelType);
        for (int i = 0; i < inventorySlots.Length; i++) inventorySlots[i].SetPanel(panelType);
        Init();
        maxQuantity = Settings.Instance.maxQuantity;
    }

    private void OnEnable()
    {
        EventHandler.DragItemActiveEvent += ActiveDragItem;
        EventHandler.DragItemInactiveEvent += InactiveDragItem;
        EventHandler.DropItemeEvent += DropItem;
    }

    private void OnDisable()
    {
        EventHandler.DragItemActiveEvent -= ActiveDragItem;
        EventHandler.DragItemInactiveEvent -= InactiveDragItem;
        EventHandler.DropItemeEvent -= DropItem;
    }

    public override void ActivePanel()
    {
        base.ActivePanel();
        SetInventory();
    }

    //protected override void SetInventory(Inventories type)
    protected override void SetInventory()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (!InventoryManager.Instance.IsEmpty(Inventories.bag, i))
                inventorySlots[i].ItemSet();
            else
                inventorySlots[i].EmptySet();
        }

        for (int i = 0; i < chestSlots.Length; i++)
        {
            if (!InventoryManager.Instance.IsEmpty(Inventories.chest, i))
                chestSlots[i].ItemSet();
            else
                chestSlots[i].EmptySet();
        }
    }
}
