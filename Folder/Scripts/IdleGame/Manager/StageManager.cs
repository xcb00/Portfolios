using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;


public class StageManager : Singleton<StageManager>
{
    bool startStage = false;
    public Transform slot1;
    StageInfo currentStage = null;
    List<Transform> monsters;
    float time = 0.0f;
    bool isBoss = false;

    public int cnt = 0;
    public int total = 0;
    public int current = 0;

    Dictionary<MonsterCharacter, Queue<GameObject>> monsterPool;

    Vector3 spawnPos = Vector3.zero;

    protected override void Awake()
    {
        base.Awake();
        monsters = new List<Transform>();
        monsterPool = new Dictionary<MonsterCharacter, Queue<GameObject>>();
        startStage = false;
    }

    public void StartStage(int stage)
    {
        if (startStage)
            return;
        stage = stage >= StaticVariables.inst.stageSO.stages.Length ? StaticVariables.inst.stageSO.stages.Length - 1 : stage;
        currentStage = StaticVariables.inst.stageSO.GetStageInfo(stage);
        CreateObjectPool();
        monsters.Clear();
        time = currentStage.spawnTime;
        startStage = true;
        isBoss = false;
        cnt = 0;
        total = currentStage.totalMonster;

        EventHandler.CallFadeInEvent();
    }

    void CreateObjectPool()
    {
        for(int i = (int)MonsterCharacter.None + 1; i < (int)MonsterCharacter.Count; i++)
        {
            MonsterCharacter monster = (MonsterCharacter)i;
            if (monsterPool.ContainsKey(monster))
            {
                if (monsterPool[monster].Count >= currentStage.maxMonster)
                    continue;
            }
            else
                monsterPool[(MonsterCharacter)i] = new Queue<GameObject>();

            for (int j = monsterPool[monster].Count; j < currentStage.maxMonster; j++)
            {
                GameObject _obj = LoadMosterPrefab(monster);
                monsterPool[monster].Enqueue(_obj);
                _obj.SetActive(false);
            }
        }
    }

    GameObject LoadMosterPrefab(MonsterCharacter character) => Instantiate(Resources.Load($"Prefab/{EnumCaching.ToString(character)}"), transform) as GameObject;

    private void Update()
    {
        if (!startStage)
            return;

        if (isBoss)
            return;

        if (total <= 0)
            return;

        if (monsters.Count >= currentStage.maxMonster)
            return;


        time += Time.deltaTime;
        if (time >= currentStage.spawnTime)
        {
            time = 0.0f;
            DequeueMonster(currentStage.stageMonsters[Random.Range(0, currentStage.stageMonsters.Length)]);
        }

    }

    public void EnqueueMonster(MonsterCharacter character, GameObject monster)
    {
        if (monsters.Contains(monster.transform))
        {
            monsters.Remove(monster.transform);
            EventHandler.CallEnquequeMonsterEvent(monster.transform);
        }

        cnt++;
        if(currentStage.totalMonster == cnt)
        {
            isBoss = true;
            DequeueMonster(currentStage.bossMonster);
        }
        monster.SetActive(false);
        monsterPool[character].Enqueue(monster);
    }

    public void DequeueMonster(MonsterCharacter character)
    {
        total--;
        float degree = Random.Range(0, 18) * 20f;
        float radius = Random.Range(5, 10) * 1f;
        spawnPos.x = Mathf.Cos(degree * Mathf.Deg2Rad);
        spawnPos.y = Mathf.Sin(degree * Mathf.Deg2Rad);

        GameObject _obj;
        if (monsterPool[character].Count < 1)
            _obj = LoadMosterPrefab(character);
        else
            _obj = monsterPool[character].Dequeue();

        monsters.Add(_obj.transform);
        _obj.transform.position = slot1.transform.position + spawnPos * radius;
        _obj.GetComponent<MonsterClass>().Initialize(character, isBoss);
    }

    public Transform GetNearestMonster(Vector3 sPos)
    {
        if (monsters.Count < 1)
            return null;
        if (monsters.Count == 1)
            return monsters[0];

        Transform result = monsters[0];
        float sqrMag = (result.position - sPos).sqrMagnitude;
        for(int i = 1; i < monsters.Count; i++)
        {
            if (sqrMag > (monsters[i].position - sPos).sqrMagnitude)
                result = monsters[i];
        }
        return result;
    }

    public void GetRangeMonsters(Vector3 sPos, float dist, out List<Transform> result)
    {
        float sqrDist = dist * dist;
        result = new List<Transform>();
        foreach(Transform trans in monsters)
        {
            if((trans.position-sPos).sqrMagnitude <= sqrDist)
                result.Add(trans);
        }
        if (result.Count < 1) result = null;
    }

    public void EnqueueAllMonster()
    {
        startStage = false;
        isBoss = true;
        foreach (Transform trans in monsters)
        {
            monsterPool[trans.GetComponent<MonsterClass>().GetMonsterCharacter()].Enqueue(trans.gameObject);
            trans.gameObject.SetActive(false);
        }
        monsters.Clear();
    }

    public int CalculateStageReward(bool addStageReward = true) => Mathf.RoundToInt(cnt * 100 + (addStageReward ? currentStage.clearGold : 0));
}
