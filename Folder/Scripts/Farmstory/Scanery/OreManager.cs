using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreManager : Singleton<OreManager>
{
    [SerializeField] SpawnOreData[] oreList; 
    //List<OrePrefabData> oreData = new List<OrePrefabData>();
    int oreListIdx = -1;
    List<OreType>[] spawnOreType;
    Dictionary<Vector2Int, OrePrefab> orePrefabDic;

    //Dictionary<OreType, JsonOreData> oreDic;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadBeforeFadeInEvent += LoadOreData;
        EventHandler.BeforeSceneUnloadEvent += SaveOreData;
        EventHandler.RemoveOreAtOreDataEvent += RemoveOreAtOreData;

        spawnOreType = new List<OreType>[(int)Grade.count];
        for (int i = 0; i < (int)Grade.count; i++)
            spawnOreType[i] = new List<OreType>();

        orePrefabDic = new Dictionary<Vector2Int, OrePrefab>();
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadBeforeFadeInEvent -= LoadOreData;
        EventHandler.BeforeSceneUnloadEvent -= SaveOreData;
        EventHandler.RemoveOreAtOreDataEvent -= RemoveOreAtOreData;
    }

    void SaveOreData()
    {
        if (oreListIdx < 0) return;

        int mineIdx = GetSceneIndex();
        if (mineIdx < 0) return;

        List<OrePrefabData> oreData = new List<OrePrefabData>();
        foreach (KeyValuePair<Vector2Int, OrePrefab> data in orePrefabDic)
            oreData.Add(new OrePrefabData(data.Value.GetPrefabData()));

        string str = JsonUtility.ToJson(new Data<OrePrefabData>(oreData));
        GameDatas.mineDataList[mineIdx] = new MineDatas((int)oreList[oreListIdx].scene, GameDatas.YearSeasonDay, str);
        DataManager.Instance.SaveMineData();
    }

    void LoadOreData()
    {
        oreListIdx = -1;
        for(int i = 0; i < oreList.Length; i++)
        {
            if (oreList[i].scene == GameDatas.currentScene)
            {
                oreListIdx = i;
                break;
            }
        }

        if (oreListIdx < 0) return;

        int mineIdx = GetSceneIndex();

        //Debug.Log("Get GameDatas.mineDataList " + GameDatas.mineDataList.Count);

        if (mineIdx < 0)
            GameDatas.mineDataList.Add(new MineDatas((int)GameDatas.currentScene, GameDatas.YearSeasonDay, InitOre()));
        else if (Utility.CompareDay(GameDatas.mineDataList[mineIdx].lastVisit))
            GameDatas.mineDataList[mineIdx] = new MineDatas((int)GameDatas.currentScene, GameDatas.YearSeasonDay, InitOre());
        Debug.Log(GameDatas.mineDataList.Count);
        SpawnOrePrefab();
    }

    int GetSceneIndex()
    {
        int result = -1;
        for (int i = 0; i < GameDatas.mineDataList.Count; i++)
        {
            if (GameDatas.mineDataList[i].sceneNum == (int)GameDatas.currentScene)
            {
                result = i;
                break;
            }
        }
        return result;
    }

    void SpawnOrePrefab()
    {
        orePrefabDic.Clear();
        int mineIdx = GetSceneIndex();

        List<OrePrefabData> oreData = JsonUtility.FromJson<Data<OrePrefabData>>(GameDatas.mineDataList[mineIdx].orePos).dataList;
        foreach (OrePrefabData data in oreData)
        {
            GameObject obj = PoolManager.Instance.DequeueObject(PoolPrefabName.ore);
            obj.SetActive(true);
            OrePrefab ore = obj.GetComponent<OrePrefab>();
            if (ore == null)
            {
                Debug.LogWarning($"{obj.name}'s OrePrefab Component is null");
                continue;
            }
            ore.SetOre(data.oreType, data.coordinate, data.hp, data.firstDestroy);
            orePrefabDic.Add(data.coordinate, ore);
        }
        EventHandler.CallSetTileToListEvent(TilemapType.dynamicCollider);
    }

    string InitOre()
    {
        int mapIdx = (int)TilemapType.oreArea - 1;
        int oreNum = oreList[oreListIdx].oreNum > GameDatas.mapTileList[mapIdx].Count ? GameDatas.mapTileList[mapIdx].Count : oreList[oreListIdx].oreNum;

        bool[] grades = new bool[(int)Grade.count];
        int[] gradeOre = new int[(int)Grade.count];

        int minGrade = (int)Grade.count;

        for (int i = 0; i < oreList[oreListIdx].oreList.Length; i++)
        {
            int _grade = (int)oreList[oreListIdx].oreList[i].grade;
            grades[_grade] = true;
            if (minGrade > _grade) minGrade = _grade;
            if (!spawnOreType[_grade].Contains(oreList[oreListIdx].oreList[i].type))
                spawnOreType[_grade].Add(oreList[oreListIdx].oreList[i].type);
        }

        // orePos 초기화
        List<OrePrefabData> oreData = new List<OrePrefabData>();
        int _idx = 0;

        // 랜덤하게 PrefabOreData 생성coordinate
        for(int i = 0; i < oreNum; i++)
        {
            _idx = Utility.GetRandomGrade();
            while (!grades[_idx])
            {
                if (_idx > 0) --_idx;
                else _idx = minGrade;
            }
            gradeOre[_idx] += 1;
        }

        int[] oreCoordinateIndex = Utility.ShuffleArray(GameDatas.mapTileList[mapIdx].Count, 200);
        int _oreType = (int)OreType.count;
        int _cnt = 0;
        for(int i = 0; i < (int)Grade.count; i++)
        {
            for(int j = 0; j < gradeOre[i]; j++)
            {
                _oreType = (int)spawnOreType[i][Random.Range(0, spawnOreType[i].Count)];
                if (_cnt >= oreNum) break;
                if (_oreType != (int)OreType.count)
                    oreData.Add(new OrePrefabData(_oreType, GameDatas.oreDataDic[_oreType].hp.x, GameDatas.mapTileList[mapIdx][oreCoordinateIndex[_cnt++]].coordinate, _oreType == (int)OreType.Stone));
            }
        }
        SaveOreData();
        return JsonUtility.ToJson(new Data<OrePrefabData>(oreData));
    }

    void RemoveOreAtOreData(Vector2Int coordinate)
    {
        if (!orePrefabDic.ContainsKey(coordinate)) return;
        else if (orePrefabDic[coordinate].firstDestroy)
            orePrefabDic.Remove(coordinate);
        /*else
        {
            oreData[oreIdx]=new OrePrefabData()
            oreData[oreIdx].FirstDestroy();
        }*/
    }

    public void HitOre(Vector2Int coordinate)
    {
        orePrefabDic[coordinate].OnHit();
    }

    public bool OreExist(Vector2Int coordinate) => orePrefabDic.ContainsKey(coordinate);

    
    
}