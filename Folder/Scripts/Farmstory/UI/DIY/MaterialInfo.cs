using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class MaterialInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI materialCountTxt;
    [SerializeField] TextMeshProUGUI inventoryCountTxt;
    [SerializeField] Image materialImg;

    public void ActiveMaterialInfo(int recipeIdx, int materialIdx, UnityAction isLack)
    {
        int materialCode = GameDatas.diyRecipeList[recipeIdx].materials[materialIdx].materialCode;
        int materialCount = GameDatas.diyRecipeList[recipeIdx].materials[materialIdx].materialCount;
        int invenCount = InventoryManager.Instance.GetItemQuantityInInventory(Inventories.bag, materialCode);

        materialCountTxt.SetText(materialCount.ToString());
        inventoryCountTxt.SetText(invenCount.ToString());
        if (invenCount >= materialCount)
            inventoryCountTxt.color = Color.green;
        else
        {
            inventoryCountTxt.color = Color.red;
            isLack.Invoke();
        }
        materialImg.sprite = InventoryManager.Instance.GetItemSpriteWithItemCode(materialCode);
        gameObject.SetActive(true);
    }

    public void InactiveMaterialInfo()
    {
        gameObject.SetActive(false);
    }
}
