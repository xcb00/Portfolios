using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapTileManager : Singleton<MapTileManager>
{
    [SerializeField] TilemapTypePair[] tilemaps;
    [SerializeField] RuleTile dugTile;
    [SerializeField] Tile waterTile;
    Vector2Int adjust = new Vector2Int(-1, -1);

    List<CropTileDetails> removeCropTiles;
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += InitMapTileList;
        EventHandler.SetTileEvent += SetTile;
        EventHandler.SetNullTileEvent += SetNullTile;
        EventHandler.SetTileToListEvent += TilemapToList;

        removeCropTiles = new List<CropTileDetails>();
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= InitMapTileList;
        EventHandler.SetTileEvent -= SetTile;
        EventHandler.SetNullTileEvent -= SetNullTile;
        EventHandler.SetTileToListEvent -= TilemapToList;
    }

    void InitMapTileList()
    {
        DataManager.Instance.LoadCropTileData(GameDatas.currentScene);

        #region
        // none             : 0
        // dugGround        : 1 / tilemaps[0] / mapTileList[0]
        // waterGround      : 2 / tilemaps[1] / mapTileList[1]
        // saticCollider    : 3 / tilemaps[2] / mapTileList[2]
        // dynamicCollider  : 4 / tilemaps[3] / mapTileList[3]
        // diggable         : 5 / tilemaps[4] / mapTileList[4]
        // fishable         : 6 / tilemaps[5] / mapTileList[5]
        // count            : 7
        #endregion
        for (int i = 0; i < tilemaps.Length; i++)
            TilemapToList(tilemaps[i].type);

        // Dug/Water Tile 그리기
        LoadTilemap();
    }

    void LoadTilemap()
    {
        removeCropTiles.Clear();
        GameDatas.removeCropCoordinateList.Clear();
        foreach (CropTileDetails tile in GameDatas.cropTileList)
        {
            Vector2Int result = tile.CheckRemain();

            // dugRemain이 0보다 작을 경우
            if (result.x < 0)
                // cropTiles 삭제
                // mapTileList에 추가하지 않으면 dug/water 타일을 그리지 않음
                removeCropTiles.Add(tile);
            else
            {
                // dugRemain이 0보다 큰 경우
                GameDatas.mapTileList[(int)TilemapType.dugGround - 1].Add(new MapTileData(tile.coordinate, TilemapType.dugGround));
                //GameDatas.cropPrefabList.Find(x => x.coordinate == tile.coordinate).CheckDay();

                if (result.y < 0)
                {
                    GameDatas.removeCropCoordinateList.Add(tile.coordinate);
                    tile.Wither();
                }
                else if (result.y > 0)
                    GameDatas.mapTileList[(int)TilemapType.waterGround - 1].Add(new MapTileData(tile.coordinate, TilemapType.waterGround));
            }
        }

        foreach (CropTileDetails tile in removeCropTiles)
            if (tile != null) GameDatas.cropTileList.Remove(tile);

        foreach (MapTileData tile in GameDatas.mapTileList[(int)TilemapType.dugGround - 1])
            SetDugTile(tile.coordinate);

        foreach (MapTileData tile in GameDatas.mapTileList[(int)TilemapType.waterGround - 1])
            SetWaterTile(tile.coordinate);

        DataManager.Instance.SaveMapTileData();
    }

    void SetTile(Vector2Int coordinate, TilemapType type)
    {
        switch (type)
        {
            case TilemapType.dugGround:
                SetDugTile(coordinate);
                break;
            case TilemapType.waterGround:
                SetWaterTile(coordinate);
                break;
            case TilemapType.dynamicCollider:
                SetColliderTile(coordinate);
                break;
            default: break;
        }
    }

    void SetDugTile(Vector2Int coordinate)
    {
        if (GameDatas.currentScene != SceneName.Farm) return;
        tilemaps[FindTilemapIndex(TilemapType.dugGround)].map.SetTile(new Vector3Int(coordinate.x, coordinate.y, 0), dugTile);
    }

    void SetWaterTile(Vector2Int coordinate)
    {
        if (GameDatas.currentScene != SceneName.Farm) return;
        tilemaps[FindTilemapIndex(TilemapType.waterGround)].map.SetTile(new Vector3Int(coordinate.x, coordinate.y, 0), waterTile);
    }

    void SetNullTile(Vector2Int coordinate, TilemapType type)
    {
        tilemaps[FindTilemapIndex(type)].map.SetTile(new Vector3Int(coordinate.x, coordinate.y, 0), null);
        GameDatas.mapTileList[(int)type - 1].Remove(GameDatas.mapTileList[(int)type - 1].Find(x => x.coordinate == coordinate));
    }

    void SetColliderTile(Vector2Int coordinate)
    {
        tilemaps[FindTilemapIndex(TilemapType.dynamicCollider)].map.SetTile(new Vector3Int(coordinate.x, coordinate.y, 0), waterTile);
    }

    void TilemapToList(TilemapType type)
    {
        int index = FindTilemapIndex(type);
        //Debug.Log(tilemaps[index].map.name);
        tilemaps[index].map.CompressBounds();
        Vector3Int start = tilemaps[index].map.cellBounds.min;
        Vector3Int end = tilemaps[index].map.cellBounds.max;
        if (type == TilemapType.staticCollider) { GameDatas.minMap = start; GameDatas.maxMap = end; }

        GameDatas.mapTileList[(int)type - 1].Clear();
        for (int x=start.x; x <= end.x; x++)
        {
            for(int y = start.y; y <= end.y; y++)
            {
                if (tilemaps[index].map.GetTile(new Vector3Int(x, y, 0)) != null)
                {
                    GameDatas.mapTileList[(int)type - 1].Add(new MapTileData(new Vector2Int(x, y), type));
                }
            }
        }

       // if (type == TilemapType.dynamicCollider)
       //     Debug.Log($"After Update {type}'s tile count :  {GameDatas.mapTileList[(int)type - 1].Count}");
        /*
                if (type == TilemapType.staticCollider)
                    foreach (MapTileData data in GameDatas.mapTileList[(int)type - 1]) Debug.Log($"{data.coordinate}");

                if(type==TilemapType.oreArea)
                    Debug.Log($"{type}'s tile count :  {GameDatas.mapTileList[(int)type - 1].Count}");
            */
    }

    int FindTilemapIndex(TilemapType type)
    {
        for (int i = 0; i < tilemaps.Length; i++)
        {
            if (type == tilemaps[i].type)
                return i;
        }
        return -1;
    }

    public bool IsCollider(CharacterDirection dir, Vector3 position)
    {
        Vector3Int coordinate = tilemaps[FindTilemapIndex(TilemapType.staticCollider)].
            map.WorldToCell(position + Utility.CharacterDirectionToVector3(dir));

        
        if (GameDatas.mapTileList[(int)TilemapType.staticCollider - 1].FindIndex(x => x.coordinate == new Vector2(coordinate.x, coordinate.y)) >= 0)
            return true;
        else if (GameDatas.mapTileList[(int)TilemapType.dynamicCollider - 1].FindIndex(x => x.coordinate == new Vector2(coordinate.x, coordinate.y)) >= 0)
            return true;
        else
            return false;
    }

    public bool IsCollider(Vector2Int coordinate)
    {
        if (GameDatas.mapTileList[(int)TilemapType.staticCollider - 1].FindIndex(x => x.coordinate == coordinate + adjust) >= 0)
            return true;
        if (GameDatas.mapTileList[(int)TilemapType.dynamicCollider - 1].FindIndex(x => x.coordinate == coordinate + adjust) >= 0)
            return true;
        else
            return false;
    }

    public bool IsFishable(Vector2Int coordinate)
    {
        return false;
    }

    public Vector3Int PositionToTilemapCoordinate(Vector3 position) => tilemaps[0].map.WorldToCell(position);
}
