using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DIYSlot : MonoBehaviour, IPointerClickHandler
{
    DIYPanel parent;
    int productCode = -1;

    void Start()
    {
        parent = GetComponentInParent<DIYPanel>();
    }

    public void SpawnSlot(int productCode)
    {
        this.productCode = productCode;
        transform.localScale = Vector3.one;
        GetComponentInChildren<TextMeshProUGUI>().SetText(InventoryManager.Instance.GetNameWithCode(productCode));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        parent.ShowRecipe(productCode);
    }
}
