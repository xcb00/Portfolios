using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagPanel : InventoryPanel
{
    [SerializeField]Slot[] slots;
    [SerializeField] Image itemImg;
    [SerializeField] Image seedMark;
    int seedCode;
    int seedIdx;
    //[SerializeField] MenuGroup menu = null;

    /*public override void InactivePanel()
    {
        base.InactivePanel();
        menu.InactivePanel();
    }*/

    private void Start()
    {
        slots = GetComponentsInChildren<Slot>();
        for (int i = 0; i < slots.Length; i++) slots[i].SetPanel(panelType);
        //menu = GetComponentInParent<MenuGroup>();

        Init();
        maxQuantity = Settings.Instance.maxQuantity;
    }

    private void OnEnable()
    {
        EventHandler.DragItemActiveEvent += ActiveDragItem;
        EventHandler.DropItemeEvent += DropItem;
        EventHandler.DragItemInactiveEvent += InactiveDragItem;
        EventHandler.ActiveSeedMoveEvent += SwapSeed;
        EventHandler.UseSeedEvent += UseSeed;
    }

    private void OnDisable()
    {
        EventHandler.DragItemActiveEvent -= ActiveDragItem;
        EventHandler.DropItemeEvent -= DropItem;
        EventHandler.DragItemInactiveEvent -= InactiveDragItem;
        EventHandler.ActiveSeedMoveEvent -= SwapSeed;
        EventHandler.UseSeedEvent -= UseSeed;
    }

    //public void InitDragItemIndex() { dragItemIndex = noneSelect; dragItemCode = 0; }

    #region Inventory
    //protected override void SetInventory(Inventories type)
    protected override void SetInventory()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!InventoryManager.Instance.IsEmpty(Inventories.bag, i))
                slots[i].ItemSet();
            else
                slots[i].EmptySet();
        }
    }

    public override void InactivePanel()
    {
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
        canvas.alpha = 0f;
    }

    public override void ActivePanel()
    {

        if (seedCode > 0)
        {
            if (InventoryManager.Instance.IsEmpty(Inventories.bag, seedIdx))
                InactiveSeedMark();
            else
                ActiveSeedMark(seedIdx);
        }

        SetInventory();

        canvas.interactable = true;
        canvas.blocksRaycasts = true;
        canvas.alpha = 1f;
    }
    #endregion

    #region Seed
    public void SelectSeed(int seedCode, int seedIdx)
    {
        if (seedCode <= 0 || seedIdx < 0) return;

        this.seedCode = seedCode;
        this.seedIdx = seedIdx;
        ActiveSeedMark(seedIdx);
        EventHandler.CallSeedChangeEvent(seedCode);
    }

    public void SwapSeed(int from, int to)
    {
        // 씨앗이 선택되지 않았을 경우 실행하지 않음
        if (seedCode <= 0)
            return;
        else
        {
            if (from == seedIdx || to == seedIdx)
            {
                if (to == seedIdx && InventoryManager.Instance.CompareItemWithIndex(panelType, from, panelType, to))
                    seedIdx = to;
                else
                    seedIdx = seedIdx == to ? from : to;
                /*if (InventoryManager.Instance.IsEmpty(panelType, from)) seedIdx = to;
                else if (InventoryManager.Instance.IsEmpty(panelType, to)) seedIdx = from;*/
                //seedIdx = seedIdx == to ? (!InventoryManager.Instance.IsEmpty(panelType, from) ? to : from) : to;

                ActiveSeedMark(seedIdx);
            }
        }
    }

    void UseSeed()
    {
        InventoryManager.Instance.RemoveItemAtIndex(Inventories.bag, 1, seedIdx);
        if (InventoryManager.Instance.GetItemQuantityWithIndex(Inventories.bag, seedIdx) <= 0)
        {
            InactiveSeedMark();
            EventHandler.CallSeedChangeEvent(-1);
        }
    }

    void ActiveSeedMark(int seedIdx)
    {
        seedMark.rectTransform.localPosition = new Vector2(-225f + 90f * (seedIdx % 6), 45f - 90f * (seedIdx / 6));
        seedMark.enabled = true;
    }

    void InactiveSeedMark()
    {
        seedMark.enabled = false;
        seedCode = -1;
        seedIdx = -1;
        EventHandler.CallSeedChangeEvent(seedCode);
    }
    #endregion
}
