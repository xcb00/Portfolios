using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropPrefab : MonoBehaviour
{
    CropDetails details;
    CropTileDetails tileDetails;
    public Vector2Int coordinate { get; private set; }
    bool canHarvest = false;
    public bool CanHarvest{ get { return tileDetails.growthDay >= details.totalGrowthDay;} private set { canHarvest = value; } }
            SpriteRenderer sprite;

    public bool hasCropTile { get { return tileDetails != null; } }
    public int GetSeedCode() => details.seedCode;
    //public Vector2Int GetCoordinate => tileDetails.coordinate;

    // 처음 생성할 때
    public bool SpawnCropPrefab(CropDetails crop, Vector2Int coordinate)
    //public bool SpawnCropPrefab(Vector2Int coordinate)
    {
        this.coordinate = coordinate;
        details = crop;
        tileDetails = GameDatas.cropTileList.Find(x => x.coordinate == coordinate);

        if (tileDetails == null) return false;

        tileDetails.seedCode = crop.seedCode;
        //tileDetails.seedCode = details.seedCode;
        sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.sprite = CheckCropSprite();
        canHarvest = false;
        transform.position = Utility.CoordinateToPosition(coordinate);
        return true;
    }

    // Json 데이터를 불러와 생성할 때
    public CropPrefab(CropPrefabJson json)
    {
        coordinate = json.coordinate;
        details = GameDatas.cropDetailsList.Find(x => x.seedCode == json.seedCode);
        tileDetails = GameDatas.cropTileList.Find(x => x.coordinate == coordinate);
    }

    public void Harvest()
    {
        //PoolManager.Instance.DequeueObject(transform.position)
        PoolManager.Instance.SpawnItem(details.harvestCode, Utility.CoordinateToPosition(tileDetails.coordinate));
        EventHandler.CallIncrementAchievementEvent(GPGSIds.achievement_farmer_master);
        EventHandler.CallUnlockAchievementEvent(GPGSIds.achievement_beginner_farmer);
        tileDetails.Wither();
        OnReturn();
    }

    public void CheckDay()
    {
        if (tileDetails.remain.y < 0)
        {
            OnReturn();
        }
        else
            sprite.sprite = CheckCropSprite();
    }

    void OnReturn() {
        tileDetails = null;
        PoolManager.Instance.EnqueueObject(PoolPrefabName.crop, gameObject);
        GameDatas.cropPrefabList.Remove(this);
    }

    Sprite CheckCropSprite()
    {
        int stage = details.growthDay.Length;
        int currentStage = 0;
        int dayCounter = details.totalGrowthDay;

        for (int i = stage - 1; i >= 0; i--)
        {
            if (tileDetails.growthDay >= dayCounter)
            {
                currentStage = i;
                break;
            }
            dayCounter -= details.growthDay[i];
        }
        //canHarvest = tileDetails.growthDay >= details.totalGrowthDay;//currentStage == stage - 1;
        return details.growthSprite[currentStage];
    }
}
