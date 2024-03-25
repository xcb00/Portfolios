using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerCursor : PlayerProperty
{
    [SerializeField] Tilemap system;
    [SerializeField] protected SpriteRenderer cursor;
    bool canFish = false;
    bool fishable = false;
    protected FishType fishType = FishType.count;

    CharacterDirection cursorDirection = CharacterDirection.down;
    protected bool canAction { get; private set; }

    protected IEnumerator TurningCursor(CharacterDirection dir, float timeToTurn)
    {
        float time = 0.0f;
        Vector3 origin = Utility.CharacterDirectionToVector3(cursorDirection);
        Vector3 target = Utility.CharacterDirectionToVector3(dir);
        while (time < timeToTurn)
        {
            time = time + Time.deltaTime > timeToTurn ? timeToTurn : time + Time.deltaTime;
            cursor.transform.localPosition = Vector3.Slerp(origin, target, time / timeToTurn);
            yield return null;
        }
        //cursorDirection = myJoystick.stickDirection;
        cursorDirection = myInput.inputDirection;
    }

    //protected Vector3 PlayerCursorVector3() => transform.position + Utility.CharacterDirectionToVector3(myDirection);
    protected Vector3 PlayerCursorVector3()
    {
        return transform.position + Utility.CharacterDirectionToVector3(myDirection);
    }
    protected Vector3Int PlayerCursorVector3Int()=> system.WorldToCell(PlayerCursorVector3());
    protected Vector2Int PlayerCursorCoordinate()
    {
        Vector3Int _vec = PlayerCursorVector3Int();
        return new Vector2Int(_vec.x, _vec.y);
    }

    public void CheckTile()
    {
        canAction = false;
        fishable = false;

        if (SystemTileManager.Instance.TileExist(TilemapType.ocean, PlayerCursorCoordinate()))
        {
            fishable = true;
            fishType = FishType.ocean;
        }
        else if (SystemTileManager.Instance.TileExist(TilemapType.river, PlayerCursorCoordinate()))
        {
            fishable = true;
            fishType = FishType.river;
        }
        else if(SystemTileManager.Instance.TileExist(TilemapType.pond, PlayerCursorCoordinate()))
        {
            fishable = true;
            fishType = FishType.pond;
        }

        if (fishable)
        {
            fishable = false;
            canAction = true;
            if (!canFish) { EventHandler.CallCanFishEvent(true); canFish = true; }
        }
        else
        {
            if (canFish) { EventHandler.CallCanFishEvent(false); canFish = false; fishType = FishType.count; }
            switch (myAction.currentTool)
            {
                case PlayerTool.Attack:
                    canAction = true; break;
                case PlayerTool.Axe:
                    canAction = TreeManager.Instance.TreeExist(PlayerCursorCoordinate());
                    break;
                /*case PlayerTool.Fish:
                    // 커서 위치에 mapTileList의 Fishable 타일이 있는지 확인
                    break;*/
                case PlayerTool.Hoe:
                    // 커서 위치에 mapTileList의 Diggable 타일이 있는지 확인
                    canAction = false;
                    // 커서의 위치가 Diggable 타일에 있는 경우
                    if (SystemTileManager.Instance.TileExist(TilemapType.diggable, PlayerCursorCoordinate()))
                    {
                        // 커서의 위치가 DugTile에 있지 않는 경우
                        if (!SystemTileManager.Instance.TileExist(TilemapType.dugGround, PlayerCursorCoordinate()))
                            canAction = true;
                    }
                    break;
                case PlayerTool.PickAxe:
                    //canAction = O_ScaneryManager.Instance.ScaneryExist(PlayerCursorCoordinate(), PlayerTool.PickAxe);
                    canAction = OreManager.Instance.OreExist(PlayerCursorCoordinate());
                    break;
                case PlayerTool.Pickup:
                    // 수확할 수 있는 농작물이 있는지 확인
                    CropPrefab crop = GameDatas.cropPrefabList.Find(x => x.coordinate == PlayerCursorCoordinate());
                    canAction = false;
                    if (crop != null)
                        if (crop.CanHarvest)
                            canAction = true;
                    break;
                case PlayerTool.Seed:
                    canAction = false;
                    if (SystemTileManager.Instance.TileExist(TilemapType.dugGround, PlayerCursorCoordinate()))
                    {
                        if (seedCode > 0)
                        {
                            if (GameDatas.cropPrefabList.Find(x => x.coordinate == PlayerCursorCoordinate()) == null && TreeManager.Instance.canPlant(PlayerCursorCoordinate()))
                                canAction = true;
                        }
                    }
                    break;
                case PlayerTool.Water:
                    // 물을 뿌릴 수 있는 땅(CropTile) 확인
                    canAction = SystemTileManager.Instance.TileExist(TilemapType.dugGround, PlayerCursorCoordinate());//&& 
                                                                                                                      //!SystemTileManager.Instance.TileExist(TilemapType.waterGround, PlayerCursorCoordinate());
                    break;
                default:
                    break;
            }
        }
        cursor.color = canAction ? Color.green : Color.red;
    }
}