using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPrefab : MonoBehaviour
{
    /*public string i;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            SpawnItem();
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            InitItem();
            //gameObject.SetActive(false);
        }
    }*/

    public int itemCode { get; private set; }
    public bool isStackable { get; private set; }

    void InitItem(int code = -1, bool stackable = false, Sprite sprite = null, CharacterDirection playerDir = CharacterDirection.zero, string name = "Item")
    {
        itemCode = code;
        isStackable = stackable;
        transform.name = name;
        transform.position = Vector3.zero;
        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = playerDir == CharacterDirection.down ? 3 : 1;
    }

    //public void SpawnItem(int code = -1, string name = "InactiveItem", Sprite sprite = null, CharacterDirection playerDir = CharacterDirection.zero, bool stackable = false)
    public void SpawnItem(Vector3 pos, int itemCode, CharacterDirection playerDir = CharacterDirection.zero)
    {
        InitItem(itemCode, InventoryManager.Instance.GetItemStackableWithItemCode(itemCode), InventoryManager.Instance.GetItemSpriteWithItemCode(itemCode), playerDir);
        transform.position = pos;
        gameObject.SetActive(true);

        if (playerDir != CharacterDirection.zero)
        {
            CharacterDirection dir = (CharacterDirection)Random.Range(0, 4);

            if (SystemTileManager.Instance.TileExist(TilemapType.staticCollider, Utility.PositionToCoordinate(pos, dir)))
                dir = (CharacterDirection)(((int)dir + 2) % 2);
            GetComponentInChildren<Animator>().SetTrigger(EnumCaching.ToString(dir));
        }
    }

    public void OnReturn()
    {
        if (InventoryManager.Instance.AddItem(Inventories.bag, itemCode, 1))
        {
            InitItem();
            PoolManager.Instance.EnqueueObject(PoolPrefabName.item, gameObject);
        }
        else
            Debug.Log("인벤토리가 가득 찼습니다");
    }
}
