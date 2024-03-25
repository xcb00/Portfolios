using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IDropHandler
{
    protected int itemCode;// { get; private set; }
    protected int index;// { get; private set; }
    Image itemImg;
    TextMeshProUGUI itemQuantityTxt;
    protected BagPanel inventory;
    protected Inventories panel;
    [SerializeField] protected Inventories type;
    //[SerializeField] bool chestPanel;

    private void Start()
    {
        itemImg = transform.GetChild(0).GetComponent<Image>();
        itemQuantityTxt = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        inventory = transform.GetComponentInParent<BagPanel>();
        index = int.Parse(transform.name.Substring(4, transform.name.Length-4));
        //panel = type==Inventories.chest?type:(chestPanel ? Inventories.chest : Inventories.bag);
    }

    private void OnDisable()
    {
        itemImg.enabled = false;
        itemCode = -1;
        itemQuantityTxt.SetText(string.Empty);
    }

    public void SetPanel(Inventories panel) { this.panel = panel; }

    public void ItemSet()
    {
        itemImg.enabled = true;
        itemImg.sprite = InventoryManager.Instance.GetItemSpriteWithIndex(type, index);
        itemCode = GameDatas.inventories[(int)type][index].itemCode;
        int i = InventoryManager.Instance.GetItemQuantityWithIndex(type, index);
        itemQuantityTxt.SetText(i <= 1 ? string.Empty : i.ToString());
    }

    public void EmptySet()
    {
        itemImg.enabled = false;
        itemImg.sprite = null;
        itemCode = 0;
        itemQuantityTxt.SetText(string.Empty);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        BegineDragEvent();
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDragEvent();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ClickEvent();
    }
        
    public void OnDrop(PointerEventData eventData)
    {
        DropEvent();
    }

    protected virtual void ClickEvent()
    {

    }

    protected virtual void BegineDragEvent()
    {

    }

    protected virtual void EndDragEvent()
    {

    }

    protected virtual void DropEvent()
    {

    }
}
