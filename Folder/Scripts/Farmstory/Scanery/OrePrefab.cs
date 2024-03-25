using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrePrefab : MonoBehaviour//, IHit
{
    SpriteRenderer[] sprites;
    OreType type;
    Vector2Int coordinate;
    public bool firstDestroy;

    public int dropItemCode => GameDatas.oreDataDic[(int)type].dropItem;
    public int hp;// { get; protected set; }
    protected int dayCount;

    public void SetOre(int type, Vector2Int coordinate, int hp, bool firstDestroy)
    {
        if (type == (int)OreType.count) return;
        
        this.type = (OreType)type;
        this.coordinate = coordinate;
        this.firstDestroy = firstDestroy;
        this.hp = hp;

        transform.position = Utility.CoordinateToPosition(coordinate);
        sprites = GetComponentsInChildren<SpriteRenderer>();

        if (!firstDestroy)
        {
            sprites[0].sprite = GameDatas.oreDataDic[type].sprites[1];
            sprites[1].sprite = GameDatas.oreDataDic[type].sprites[0];
            sprites[0].enabled = true;
            sprites[1].enabled = false;
        }
        else
        {
            sprites[1].sprite = GameDatas.oreDataDic[type].sprites[0];
            FirstDestroy(false);
        }

        gameObject.SetActive(true);
        EventHandler.CallSetTileEvent(coordinate, TilemapType.dynamicCollider);
    }

    void FirstDestroy(bool dropItem = true)
    {
        firstDestroy = true;
        sprites[0].enabled = false;
        sprites[1].enabled = true;
        if (dropItem)
        {
            PoolManager.Instance.SpawnItem(dropItemCode, transform.position);
            EventHandler.CallIncrementAchievementEvent(GPGSIds.achievement_miner_master);
            EventHandler.CallUnlockAchievementEvent(GPGSIds.achievement_beginner_miner);
        }
        hp = GameDatas.oreDataDic[(int)type].hp.y;
    }

    void Destroy()
    {
        EventHandler.CallSetNullTileEvent(coordinate, TilemapType.dynamicCollider);
        EventHandler.CallSetTileToListEvent(TilemapType.dynamicCollider);
        EventHandler.CallIncrementAchievementEvent(GPGSIds.achievement_miner_master);
        EventHandler.CallUnlockAchievementEvent(GPGSIds.achievement_beginner_miner);
        PoolManager.Instance.SpawnItem(20002, transform.position);
        PoolManager.Instance.EnqueueObject(PoolPrefabName.ore, gameObject);
    }

    /*void HitOre()
    {

    }*/

    public void OnHit(int dmg = 1)
    {
        hp -= dmg;
        if (hp < 0)
        {
            EventHandler.CallRemoveOreAtOreData(coordinate);
            if (!firstDestroy) FirstDestroy();
            else Destroy();
        }
        // ÇÇ°Ý ÀÌÆåÆ®
        //else HitOre();
    }

    public OrePrefabData GetPrefabData() => new OrePrefabData((int)type, hp, coordinate, firstDestroy);
}
