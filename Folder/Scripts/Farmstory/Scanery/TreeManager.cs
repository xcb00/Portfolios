using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : Singleton<TreeManager>
{
    int cnt = 0;
    Dictionary<Vector2Int, TreePrefab> treePrefabDic;
    [SerializeField] List<Vector2Int> cantPlantTree;

    protected override void Awake()
    {
        base.Awake();
    }


    private void OnEnable()
    {
        EventHandler.AfterSceneLoadBeforeFadeInEvent += LoadTree;
        EventHandler.BeforeSceneUnloadEvent += SaveTree;
        EventHandler.RemoveTreeAtTreeDataEvent += RemoveTree;

        treePrefabDic = new Dictionary<Vector2Int, TreePrefab>();
        cantPlantTree = new List<Vector2Int>();
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadBeforeFadeInEvent -= LoadTree;
        EventHandler.BeforeSceneUnloadEvent -= SaveTree;
        EventHandler.RemoveTreeAtTreeDataEvent -= RemoveTree;
    }


    void LoadTree()
    {
        if (!DataManager.Instance.ExceptScene(GameDatas.currentScene)) return;
        DataManager.Instance.LoadTreePrefabData(GameDatas.currentScene);
        //SpawnTree(new TreePrefabData(10206, 0, 4, 0, new Vector2Int(-1, -7), new Vector3Int(1, 0, 4)));
        foreach (TreePrefabData data in GameDatas.treePrefabList)
        {
            SpawnTree(data);
        }
    }

    void SaveTree()
    {
        if (!DataManager.Instance.ExceptScene(GameDatas.currentScene)) return;
        GameDatas.treePrefabList.Clear();
        foreach(KeyValuePair<Vector2Int, TreePrefab> data in treePrefabDic)
            GameDatas.treePrefabList.Add(data.Value.GetPrefabData());
        DataManager.Instance.SaveTreeData();
        treePrefabDic.Clear();
    }

    public bool SpawnTree(Vector2Int coordinate, int fruitCode)
    {
        if (treePrefabDic.ContainsKey(coordinate)) return false;
        TreePrefab tree = PoolManager.Instance.DequeueObject(PoolPrefabName.tree).GetComponent<TreePrefab>();
        tree.SetTree(fruitCode, coordinate);
        treePrefabDic.Add(coordinate, tree);
        AddCantPlant(coordinate, true);

        EventHandler.CallUseSeedEvent();
        return true;
    }

    public bool SpawnTree(TreePrefabData data)
    {
        if (treePrefabDic.ContainsKey(data.coordinate)) return false;
        TreePrefab tree = PoolManager.Instance.DequeueObject(PoolPrefabName.tree).GetComponent<TreePrefab>();
        treePrefabDic.Add(data.coordinate, tree);
        AddCantPlant(data.coordinate, true);
        tree.SetTree(data);
        return false;
    }

    public void HitTree(Vector2Int coordinate)
    {
        if (treePrefabDic[coordinate].OnHit())
        {
            EventHandler.CallSetNullTileEvent(coordinate, TilemapType.dynamicCollider);
            EventHandler.CallSetTileToListEvent(TilemapType.dynamicCollider);
        }
    }

    void RemoveTree(Vector2Int coordinate)
    {
        if (!treePrefabDic.ContainsKey(coordinate)) return;
        treePrefabDic.Remove(coordinate);
        AddCantPlant(coordinate, false);
    }

    void AddCantPlant(Vector2Int coordinate, bool add)
    {
        Vector2Int coor = Vector2Int.zero;
        for(int i = -2; i < 3; i++)
        {
            for(int j = -2; j < 3; j++)
            {
                if (Mathf.Abs(i) + Mathf.Abs(j) < 4)
                {
                    coor.x = i;
                    coor.y = j;

                    if(add && !cantPlantTree.Contains(coordinate+coor))
                        cantPlantTree.Add(coordinate + coor);
                    else if(!add && cantPlantTree.Contains(coordinate + coor))
                        cantPlantTree.Remove(coordinate + coor);

                }
            }
        }
    }

    public bool TreeExist(Vector2Int coordinate) => treePrefabDic.ContainsKey(coordinate);
    public bool canPlant(Vector2Int coordinate) => !cantPlantTree.Contains(coordinate);
}
