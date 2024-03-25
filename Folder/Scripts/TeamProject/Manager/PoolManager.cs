using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    [SerializeField] RectTransform iconAnchor;
    [SerializeField] PoolObject[] poolObj;
    [SerializeField] StageSO stage;
    Dictionary<PoolType, Queue<GameObject>> pool;
    public List<int> poolTypeList;
    public int GetMonsterCount => poolObj.Length - 6;

    Transform tr;

    private void OnEnable()
    {
        EventHandler.PoolClearEvent += StageClear;
    }
    private void OnDisable()
    {
        EventHandler.PoolClearEvent -= StageClear;
    }

    private void Start()
    {
        tr = GetComponent<Transform>();
        pool = new Dictionary<PoolType, Queue<GameObject>>();

        // PoolObjects 중 stage에 등록되지 않은 몬스터는 Pool에 생성하지 않기 위해 제외할 PoolType 설정
        poolTypeList = new List<int>();

        for(int i=0; i < stage.stageLevel.Length; i++)
        {
            for(int j=0;j<stage.stageLevel[i].stageMonsters.Length; j++)
            {
                int _type = (int)stage.stageLevel[i].stageMonsters[j].type;
                if (!poolTypeList.Contains(_type)) poolTypeList.Add(_type);
            }
            if(!poolTypeList.Contains((int)stage.stageLevel[i].stageBoss.type))
                poolTypeList.Add((int)stage.stageLevel[i].stageBoss.type);
        }

        for (int i = (int)PoolType.PlayerFarmer; i < (int)PoolType.cnt; i++)
            poolTypeList.Add(i);

        for(int i = 0; i < poolTypeList.Count; i++)
        {
            foreach (PoolObject _pool in poolObj) 
            {
                if((int)_pool.type==poolTypeList[i])
                    CreatePool(_pool.type, _pool.poolObj);
            }
        }

        /*foreach (PoolObject _pool in poolObj)
            CreatePool(_pool.type, _pool.poolObj);*/

        /*foreach (KeyValuePair<PoolType, Queue<GameObject>> pair in pool)
            Debug.Log($"{pair.Key} : {pair.Value.Dequeue().name}");*/
    }

    void CreatePool(PoolType type, GameObject prefab)
    {
        if ((int)type < (int)PoolType.PlayerFarmer)
            if (!poolTypeList.Contains((int)type)) return;

        if(prefab==null)
        {
            Debug.LogError($"{transform.name} Error : PoolManager에 {type} Prefab이 등록되지 않음");
            return;
        }

        GameObject parent = null;
        if (type != PoolType.MinimapIcon)
        {
            parent = new GameObject($"{EnumCaching.ToString(type)}Anchor");
            parent.transform.SetParent(transform);
        }
        else
            parent = iconAnchor.gameObject;

        if (!pool.ContainsKey(type))
        {
            pool.Add(type, new Queue<GameObject>());
            AddObject(type, prefab, parent.transform);
        }
    }

    void AddObject(PoolType type, GameObject obj, Transform parent)
    {
        GameObject instance = Instantiate(obj, parent) as GameObject;
        instance.SetActive(false);
        //Debug.Log("Add Object's ID : " + instance.GetInstanceID());
        pool[type].Enqueue(instance);
    }

    public GameObject DeququeObject(PoolType type, bool active = false)
    {
        if (!pool.ContainsKey(type))
        {
            Debug.LogError($"{transform.name} Error : PoolManager에 {type} Pool이 등록되지 않음");
            return null;
        }

        GameObject instance = pool[type].Dequeue();
        instance.name = EnumCaching.ToString(type);

        if (pool[type].Count < 1)
            AddObject(type, instance, instance.transform.parent);
        //pool[type].Enqueue(Instantiate(instane, instane.transform.parent));

        //Debug.Log("Dequque Object's ID : " + instance.GetInstanceID());
        instance.SetActive(active);
        return instance;
    }

    public void SpawnObject(PoolType type, Vector3 position)
    {
        if (!pool.ContainsKey(type))
        {
            Debug.LogError($"{transform.name} Error : PoolManager에 {type} Pool이 등록되지 않음");
            return ;
        }

        GameObject instance = pool[type].Dequeue();
        instance.name = EnumCaching.ToString(type);
        instance.transform.position = position;
        instance.SetActive(true);
        if (pool[type].Count < 1)
            AddObject(type, instance, instance.transform.parent);
    }

    public void EnqueueObject(PoolType type, GameObject obj)
    {
        obj.SetActive(false);
        pool[type].Enqueue(obj);
    }

    void StageClear(Transform trans)
    {
        List<int> nextMonster = new List<int>();

        if (GameDatas.stageLevel >= GameDatas.maxStage) Debug.Log("Game Clear");


        foreach (StageMonster monster in stage.stageLevel[GameDatas.stageLevel + 1 ].stageMonsters)
            nextMonster.Add((int)monster.type); 
        
        for(int i = 0; i < trans.childCount; i++)
        {
            for (int j = 0; j < trans.GetChild(i).childCount; j++)
            {
                /*if (transform.GetChild(i).GetChild(j).gameObject.activeSelf)
                {
                    Debug.Log($"({i}, {j}) {(PoolType)poolTypeList[i]}");
                    EnqueueObject((PoolType)poolTypeList[i], transform.GetChild(i).GetChild(j).gameObject);
                }*/
                // 적 유닛일 경우
                if (poolTypeList[i] < (int)PoolType.PlayerFarmer)
                {
                    // 다음 스테이지에 나오는 몬스터일 경우
                    if (nextMonster.Contains(poolTypeList[i]))
                    {
                        if (trans.GetChild(i).GetChild(j).gameObject.activeSelf)
                            EnqueueObject((PoolType)poolTypeList[i], trans.GetChild(i).GetChild(j).gameObject);
                    }
                    // 다음 스테이지에 나오지 않는 몬스터일 경우
                    else
                    {
                        if (trans.GetChild(i).GetChild(j).gameObject.activeSelf)
                        {
                            // Pool에 3 초과일 경우
                            if (pool[(PoolType)poolTypeList[i]].Count > 5)
                                Destroy(trans.GetChild(i).GetChild(j).gameObject);
                            // Pool에 3 이하일 경우
                            else
                                EnqueueObject((PoolType)poolTypeList[i], trans.GetChild(i).GetChild(j).gameObject);
                        }
                        else
                        {
                            if (pool[(PoolType)poolTypeList[i]].Count > 5)
                                Destroy(DeququeObject((PoolType)poolTypeList[i]));
                        }
                    }
                }
                else
                {
                    if (trans.GetChild(i).GetChild(j).gameObject.activeSelf)
                        EnqueueObject((PoolType)poolTypeList[i], trans.GetChild(i).GetChild(j).gameObject);
                }
            }
        }

        for(int i = 0; i < iconAnchor.childCount; i++)
        {
            if (iconAnchor.GetChild(i).gameObject.activeSelf)
                EnqueueObject(PoolType.MinimapIcon, iconAnchor.GetChild(i).gameObject);
            else
                continue;
        }
    }
}
