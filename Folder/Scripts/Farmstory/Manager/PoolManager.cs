using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class PoolManager : Singleton<PoolManager>
{
    [SerializeField] Pool[] objectPool = null;
    Dictionary<PoolPrefabName, Queue<GameObject>> poolDic;
    CharacterDirection playerDir = CharacterDirection.zero;
    StringBuilder sb;
    public CharacterDirection PlayerDir { set { playerDir = value; } }

    protected override void Awake()
    {
        base.Awake();
        sb = new StringBuilder();
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += EnqueueAllObject;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= EnqueueAllObject;
    }

    private void Start()
    {
        poolDic = new Dictionary<PoolPrefabName, Queue<GameObject>>();
        foreach (Pool pool in objectPool)
            CreatePool(pool.index, pool.prefab);
    }

    void CreatePool(PoolPrefabName index, GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError($"{transform.name} Error : PoolManager�� {EnumCaching.ToString(index)} Prefab�� ��ϵ��� ����");
            return;
        }

        GameObject parent = new GameObject($"{EnumCaching.ToString(index)}Prefab");
        parent.transform.SetParent(transform);

        if (parent.transform.GetSiblingIndex() != (int)index)
            Debug.LogError($"{transform.name} Error : {prefab.name}�� ������ PoolPrefabName�� ������ �ٸ�");

        if (!poolDic.ContainsKey(index))
        {
            poolDic.Add(index, new Queue<GameObject>());
            GameObject instance = Instantiate(prefab, parent.transform) as GameObject;
            instance.SetActive(false);
            poolDic[index].Enqueue(instance);
        }
    }

    void AddObject(PoolPrefabName index)
    {
        GameObject instance = DequeueObject(index);
        EnqueueObject(index, instance);
    }

    public GameObject DequeueObject(PoolPrefabName index)
    {
        if (!poolDic.ContainsKey(index))
        {
            Debug.LogError($"{transform.name} Error : PoolManager���� {index} Pool�� ��ϵ��� �ʾҽ��ϴ�");
            return null;
        }

        GameObject instance = poolDic[index].Dequeue();
        sb.Clear();
        sb.Append(EnumCaching.ToString(index));
        sb.Append(instance.transform.parent.childCount);
        instance.name = sb.ToString(); //index.ToString();

        // ���ӿ�����Ʈ�� Dequque���� �� Pool�� Object�� ���ٸ� GameObject�� ������ �� Enqueue��
        if (poolDic[index].Count < 1)
            poolDic[index].Enqueue(Instantiate(instance, instance.transform.parent));

        return instance;
    }

    public void SpawnItem(int code, Vector3 pos)
    {
        GameObject item = DequeueObject(PoolPrefabName.item);
        item.SetActive(true);
        item.GetComponent<ItemPrefab>().SpawnItem(pos, code, playerDir);
    }

    public void EnqueueObject(PoolPrefabName index, GameObject poolObject)
    {
        poolObject.SetActive(false);
        poolDic[index].Enqueue(poolObject);
    }

    public void EnqueueAllObject()
    {
        for (int i = 0; i < poolDic.Count;/*(int)PoolPrefabName.count;*/ i++)
        {
            Transform anchor = transform.GetChild(i);

            if (poolDic[(PoolPrefabName)i].Count < 2)
                AddObject((PoolPrefabName)i);

            for (int j = 0; j < anchor.childCount; j++)
            {
                if(anchor.GetChild(j).gameObject.activeSelf)
                    EnqueueObject((PoolPrefabName)i, anchor.GetChild(j).gameObject);
            }
        }
    }
}
