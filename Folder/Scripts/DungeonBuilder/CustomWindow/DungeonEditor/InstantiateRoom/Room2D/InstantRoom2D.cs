using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class InstantRoom2D : MonoBehaviour
{
    [HideInInspector] public List<Room2DDoorClass> doorwayList;
    [HideInInspector] public List<Vector2Int> spawnPosition;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap decoration2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    bool[] doorwayOrientations = new bool[4];
    bool alreadyDoorwayBlock = false;

    private void Awake()
    {
        for (int i = 0; i < doorwayOrientations.Length; i++)
            doorwayOrientations[i] = false;
    }

    public void Initialise(List<Room2DDoorClass> doorwayList, List<Vector2Int> spawnPosition)
    {
        this.doorwayList = doorwayList;
        this.spawnPosition = spawnPosition;
        BindlingTilemap();
    }

    public void DoorwayConnect(Orientation orientation)
    {
        if (orientation == Orientation.none)
            return;

        doorwayOrientations[(int)orientation] = true;
    }

    void BindlingTilemap()
    {
        grid = GetComponentInChildren<Grid>();

        // 타일맵의 좌표와 월드 좌표를 맞추기 위해 x와 y를 -0.5씩 이동
        grid.transform.position = -Vector3.right * 0.5f;
        grid.transform.position -= Vector3.up * 0.5f;

        foreach(Tilemap tilemap in transform.GetComponentsInChildren<Tilemap>())
        {
            if (tilemap.gameObject.tag.Equals("groundTilemap"))
                groundTilemap = tilemap;
            else if (tilemap.gameObject.tag.Equals("decoration1Tilemap"))
                decoration1Tilemap = tilemap;
            else if (tilemap.gameObject.tag.Equals("decoration2Tilemap"))
                decoration2Tilemap = tilemap;
            else if (tilemap.gameObject.tag.Equals("frontTileamap"))
                frontTilemap = tilemap;
            else if (tilemap.gameObject.tag.Equals("collisionTilemap"))
                collisionTilemap = tilemap;
        }

        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }

    void BlockOffUnusedDoorways()
    {
        for(int i=0;i<doorwayOrientations.Length; i++)
        {
            Room2DDoorClass doorData = doorwayList.Find(x => x.orientation == (Orientation)i);
            if (doorData == null)
            {
                Debug.LogError($"The doorway data of {EnumCaching.ToString((Orientation)i)} isn't exists ");
                return;
            }

            if (doorwayOrientations[i])
            {
                SpawnDoorPrefab(doorData);
                continue;
            }

            if (collisionTilemap != null)
                BlockDoorwayOnTilemapLayer(collisionTilemap, doorData);
            if (groundTilemap != null)
                BlockDoorwayOnTilemapLayer(groundTilemap, doorData);
            if (decoration1Tilemap != null)
                BlockDoorwayOnTilemapLayer(decoration1Tilemap, doorData);
            if (decoration2Tilemap != null)
                BlockDoorwayOnTilemapLayer(decoration2Tilemap, doorData);
            if (frontTilemap != null)
                BlockDoorwayOnTilemapLayer(frontTilemap, doorData);
        }

        alreadyDoorwayBlock = true;
    }

    void SpawnDoorPrefab(Room2DDoorClass doorData)
    {
        GameObject doorPrefab = Instantiate(doorData.doorPrefab, Vector3.zero, Quaternion.identity, transform);
        doorPrefab.transform.localPosition = new Vector3(doorData.doorPosition.x, doorData.doorPosition.y, 0f);
        doorPrefab.GetComponent<DoorPrefab2D>().SpawnDoorPrefab(doorData.orientation);
    }


    void BlockDoorwayOnTilemapLayer(Tilemap tilemap, Room2DDoorClass doorData)
    {
        switch (doorData.orientation)
        {
            case Orientation.north:
            case Orientation.south:
                BlockDoorwayHorizontally(tilemap, doorData);
                break;
            case Orientation.east:
            case Orientation.west:
                BlockDoorwayVertically(tilemap, doorData);
                break;
            default:
                break;

        }
    }

    void BlockDoorwayHorizontally(Tilemap tilemap, Room2DDoorClass doorData)
    {
        Vector2Int startPosition = doorData.copyPosition;

        for(int x = 0; x < doorData.width; x++)
        {
            for(int y =0; y < doorData.height; y++)
            {
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + x, startPosition.y - y, 0));

                tilemap.SetTile(new Vector3Int(startPosition.x + 1 + x, startPosition.y - y, 0), // 복사한 타일을 놓을 위치
                    tilemap.GetTile(new Vector3Int(startPosition.x + x, startPosition.y - y, 0))); // 가져올 타일

                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + 1 + x, startPosition.y - y, 0), transformMatrix);
            }
        }
    }

    void BlockDoorwayVertically(Tilemap tilemap, Room2DDoorClass doorData)
    {
        Vector2Int startPosition = doorData.copyPosition;

        for (int y = 0; y < doorData.height; y++)
        {
            for (int x = 0; x < doorData.width; x++)
            {
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + x, startPosition.y - y, 0));

                tilemap.SetTile(new Vector3Int(startPosition.x + x, startPosition.y - 1 - y, 0), // 복사한 타일을 놓을 위치
                    tilemap.GetTile(new Vector3Int(startPosition.x + x, startPosition.y - y, 0))); // 가져올 타일

                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + x, startPosition.y - 1 - y, 0), transformMatrix);
            }
        }
    }

    public void DisableRoom()
    {
        gameObject.SetActive(false);
    }

    public void EnableRoom()
    {
        if (!alreadyDoorwayBlock)
            BlockOffUnusedDoorways();

        gameObject.SetActive(true);
    }

    public Vector2Int GetSpawnPosition(Orientation orientation)
    {
        if (orientation == Orientation.none)
            return spawnPosition[Random.Range(0, spawnPosition.Count)];
        else
        {
            orientation = (Orientation)(((int)orientation + 2) % 4);

            Vector2Int adjust = DungeonBuilder.Inst.OrientationToVector2Int(orientation);
            return doorwayList.Find(x => x.orientation == orientation).doorPosition - adjust;
        }
    }
}
