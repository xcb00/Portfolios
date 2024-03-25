using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemTileManager : Singleton<SystemTileManager>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public bool TileExist(TilemapType type, Vector2Int coordinate)
    {
        MapTileData tile = GameDatas.mapTileList[(int)type - 1].Find(x => x.coordinate == coordinate);
        if (tile.type == type) return true;
        else return false;
    }

    public bool CreateDugTile(Vector2Int coordinate)
    {
        GameDatas.mapTileList[(int)TilemapType.dugGround - 1].Add(new MapTileData(coordinate, TilemapType.dugGround));
        // CropTile 생성
        GameDatas.cropTileList.Add(new CropTileDetails(coordinate));

        // EventHandler를 통해 MapTileManager에서 SetTile
        EventHandler.CallSetTileEvent(coordinate, TilemapType.dugGround);

        // 데이터 저장 DugTile, CropTile
        return true;
    }

    public bool CreateWaterTile(Vector2Int coordinate)
    {
        GameDatas.mapTileList[(int)TilemapType.waterGround - 1].Add(new MapTileData(coordinate, TilemapType.waterGround));
        GameDatas.cropTileList.Find(x => x.coordinate == coordinate).WaterGround(2);
        EventHandler.CallSetTileEvent(coordinate, TilemapType.waterGround);

        return true;
    }
}
