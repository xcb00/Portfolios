using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] EnemySpawnData[] spawnDatas;
    int index = -1;
    int currentEnemy = -1;
    int maxPos = 0;
    float time = 0.0f;
    Coroutine spawning = null;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadBeforeFadeInEvent += SpawnEnemy;
        EventHandler.EnemyDieEvent += EnemyDie;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadBeforeFadeInEvent -= SpawnEnemy;
        EventHandler.EnemyDieEvent -= EnemyDie;
    }

    void Init()
    {
        index = -1;
        currentEnemy = 0;
        time = 0.0f;

        if(spawning!=null)
        {
            StopCoroutine(spawning);
            spawning = null;
        }
    }

    void EnemyDie(bool enqueue)
    {
        if (enqueue) return;
        currentEnemy--;
        if (spawning == null)
            spawning = StartCoroutine(EnemySpawning());
    }

    void SpawnEnemy()
    {
        Init();

        for (int i = 0; i < spawnDatas.Length; i++)
        {
            if (GameDatas.currentScene == spawnDatas[i].spawnScene)
            {
                index = i;
                break;
            }
        }

        if (index < 0) return;

        maxPos = GameDatas.mapTileList[(int)TilemapType.enemySpawn - 1].Count;
        for (int i = 0; i < spawnDatas[index].startEnemy; i++)
            DequeueEnemy();
        
        time = spawnDatas[index].spawnTime;
        spawning = StartCoroutine(EnemySpawning());
    }

    // 코루틴을 이용해 currentEnemy가 max가 될 때까지 spawnTime마다 emeny 생성
    IEnumerator EnemySpawning()
    {
        float t = 0.0f;
        while (currentEnemy < spawnDatas[index].maxEnemy)
        {
            //if (index < 0) break;
            
            t = 0.0f;
            while (t < time)
            {
                t += Time.deltaTime;
                yield return null;
            }
            DequeueEnemy();

            if (currentEnemy == spawnDatas[index].maxEnemy)
                spawning = null;
        }
    }

    void DequeueEnemy()
    {
        if (index < 0 || currentEnemy >= spawnDatas[index].maxEnemy) return;
        currentEnemy++;

        int idx = spawnDatas[index].enemies.Length == 1 ? 0 : Random.Range(0, spawnDatas[index].enemies.Length);
        GameObject obj = PoolManager.Instance.DequeueObject(spawnDatas[index].enemies[idx]); 
        obj.GetComponent<EnemyState>().Respawn(Utility.CoordinateToPosition(GameDatas.mapTileList[(int)TilemapType.enemySpawn - 1][Random.Range(0, maxPos)].coordinate));
    }

    // enemy가 죽으면 이벤트 핸들러를 이용해 currentEnemy 감소
}
