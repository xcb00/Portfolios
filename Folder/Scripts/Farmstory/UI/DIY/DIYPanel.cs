using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DIYPanel : Panel
{
    [SerializeField] Transform content;
    [SerializeField] TextMeshProUGUI productName;
    [SerializeField] Image productImage;
    //DIYSlot[] slots;
    MaterialInfo[] materialInfo;

    public bool canProduce = true;
    int diyIndex = 0;

    private void Start()
    {
        Init();
    }

    public override void ActivePanel()
    {
        SpawnSlots();
        ShowRecipeWithIndex(0);
        base.ActivePanel();
    }

    void SpawnSlots()
    {
        if (materialInfo != null) return;

        GameObject slot = Resources.Load("Prefab/DIYSlot") as GameObject;
        materialInfo = GetComponentsInChildren<MaterialInfo>();

        //slots = new DIYSlot[GameDatas.diyRecipeList.Count];
        for (int i = 0; i < GameDatas.diyRecipeList.Count; i++) {
            GameObject _slot = Instantiate(slot);
            _slot.transform.SetParent(content);
            _slot.GetComponent<DIYSlot>().SpawnSlot(GameDatas.diyRecipeList[i].productCode);
        }

        ShowRecipeWithIndex(0);
    }

    public void ShowRecipe(int productCode)
    {
        canProduce = true;
        int recipeIdx = -1;
        for(int i=0;i<GameDatas.diyRecipeList.Count;i++)
            if (GameDatas.diyRecipeList[i].productCode == productCode)
            {
                recipeIdx = i;
                break;
            }
        ShowRecipeWithIndex(recipeIdx, productCode);
    }

    public void ShowRecipeWithIndex(int recipeIndex, int productCode=-1)
    {
        int pCode = productCode > 0 ? productCode : GameDatas.diyRecipeList[recipeIndex].productCode;
        productName.SetText(InventoryManager.Instance.GetNameWithCode(pCode));
        productImage.sprite = InventoryManager.Instance.GetItemSpriteWithItemCode(pCode);

        diyIndex = recipeIndex;

        int materialCnt = GameDatas.diyRecipeList[recipeIndex].materials.Length;
        for(int i = 0; i < 8; i++)
        {
            if (i < materialCnt) materialInfo[i].ActiveMaterialInfo(recipeIndex, i, ()=>IsLack());
            else materialInfo[i].InactiveMaterialInfo();
        }
    }

    void IsLack() { canProduce = false; }

    public void ProduceProduct()
    {
        if (!canProduce) { EventHandler.CallPrintSystemMassageEvent("재료가 부족합니다"); return; }

        for (int i = 0; i < GameDatas.diyRecipeList[diyIndex].materials.Length; i++)
            InventoryManager.Instance.RemoveItemForDIY(GameDatas.diyRecipeList[diyIndex].materials[i]);

        InventoryManager.Instance.AddItem(Inventories.bag, GameDatas.diyRecipeList[diyIndex].productCode, 1);

        ShowRecipeWithIndex(diyIndex);
    }
}
