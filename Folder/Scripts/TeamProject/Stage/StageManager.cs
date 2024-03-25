using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] StageSO stage;
    [SerializeField] int addPopulation = 3;
    float[] monstersDelay;
    Vector3 spawnPosition;
    int wave = 0;
    int monsterCnt = 0;
    float tick = 0.0f;
    bool activeBoss = false;

    bool stopRespoawn = false;

    private void Start()
    {
        spawnPosition = transform.GetChild(0).position;
        GameDatas.stageLevel = 0;
        monstersDelay= new float[stage.stageLevel[GameDatas.stageLevel].stageMonsters.Length];
        for (int i = 0; i < monstersDelay.Length; i++)
            GetRespawnTime(i);

        GameDatas.maxStage = stage.stageLevel.Length - 1;
    }

    private void OnEnable()
    {
        EventHandler.StartWaveEvent += StartWave;
        EventHandler.StageClearEvent += StageClear;
        EventHandler.StopRespawnEvent += StopResapwn;
        EventHandler.StageStartEvent += StageStart;
    }

    private void OnDisable()
    {
        EventHandler.StartWaveEvent -= StartWave;
        EventHandler.StageClearEvent -= StageClear;
        EventHandler.StopRespawnEvent -= StopResapwn;
        EventHandler.StageStartEvent -= StageStart;
    }

    
    void StageClear()
    {
        StopAllCoroutines();
        GameDatas.soilQueue.Clear();
        ResourceManager.Instance.InitGameDatas(addPopulation);

        if (GameDatas.stageLevel < stage.stageLevel.Length - 1)
        { 
            monstersDelay = new float[stage.stageLevel[GameDatas.stageLevel + 1].stageMonsters.Length];
            stopRespoawn = false;
            activeBoss = false;
            wave = 0;
        }
    }

    void StopResapwn(bool stop) { stopRespoawn = stop; }

    void StartWave()
    {
        GameDatas.stopUnit = false;
        if (GameDatas.stageLevel >= stage.stageLevel.Length) GameDatas.stageLevel = stage.stageLevel.Length - 1;
        if (wave >= stage.stageLevel[GameDatas.stageLevel].waves.Length)
        {
            CreateComUnit(stage.stageLevel[GameDatas.stageLevel].stageBoss.type);
            activeBoss = true;
#if UNITY_EDITOR
            stopRespoawn = true;
#endif
        }
        StartCoroutine(RespawningMonster());
    }

    IEnumerator RespawningMonster()
    {
        monsterCnt = 0;
        while (monsterCnt < (activeBoss ? 1 : stage.stageLevel[GameDatas.stageLevel].waves[wave]))
        {
            for (int i = 0; i < stage.stageLevel[GameDatas.stageLevel].stageMonsters.Length; i++)
            {
                if (stopRespoawn) continue;
                monstersDelay[i] -= Time.deltaTime;
                if (monstersDelay[i] < 0.0f)
                {
                    monsterCnt = activeBoss ? 0 : monsterCnt + 1;
                    /*Debug.Log("Stage level : "+GameDatas.stageLevel);
                    Debug.Log("i : " + i);*/
                    CreateComUnit(stage.stageLevel[GameDatas.stageLevel].stageMonsters[i].type);
                    GetRespawnTime(i);
                }
            }
            yield return null;
        }
        wave++;
        //EventHandler.CallPauseTimeEvent(false);
        GameDatas.pause = false;
    }

    void CreateComUnit(ComUnitType type)
    {
        GameObject obj = PoolManager.Instance.DeququeObject((PoolType)(int)type);
        obj.SetActive(true);
        //obj.GetComponent<ComUnit>().SpawnUnit(spawnPosition, (PoolType)(int)type);
        obj.GetComponent<UnitMovement>().SpawnUnit(spawnPosition, (PoolType)(int)type);
    }

   /* private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
            StopAllCoroutines();
    }*/

    void StageStart()
    {
        GameDatas.pause = false;
        tick = 0.0f;
    }

    private void Update()
    {
        if (!GameDatas.pause)
        {
            tick += Time.deltaTime;

            if (tick >= Settings.waveDelay)
            {
                tick -= Settings.waveDelay;
                GameDatas.pause = true;
                EventHandler.CallStartWaveEvent();
            }
        }
    }

    void GetRespawnTime(int i) { monstersDelay[i] = stage.stageLevel[GameDatas.stageLevel].stageMonsters[i].GetRespawnTime * (1f - 0.05f * wave); }
}
