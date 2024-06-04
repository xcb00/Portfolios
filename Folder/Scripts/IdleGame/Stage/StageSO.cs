using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageInfo
{
    public MonsterCharacter[] stageMonsters;
    public MonsterCharacter bossMonster;
    public int totalMonster;
    public int maxMonster;
    public int clearGold;
    public float spawnTime;

    public StageInfo(StageInfo origin)
    {
        bossMonster = origin.bossMonster;
        stageMonsters = new MonsterCharacter[origin.stageMonsters.Length];
        for (int i = 0; i < origin.stageMonsters.Length; i++)
            stageMonsters[i] = origin.stageMonsters[i];

        totalMonster = origin.totalMonster;
        maxMonster = origin.maxMonster;
        clearGold = origin.clearGold;
        spawnTime = origin.spawnTime;
    }
}

[CreateAssetMenu(fileName = "StageSO", menuName = "ScriptableSO/StageSO")]
public class StageSO : ScriptableObject
{
    public StageInfo[] stages;

    public StageInfo GetStageInfo(int stage) => new StageInfo(stages[stage]);

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (stages.Length < 1)
            Debug.LogWarning($"{this.name} stages is null");

        List<MonsterCharacter> list = new List<MonsterCharacter>();

        foreach(StageInfo info in stages)
        {
            if(info.bossMonster == MonsterCharacter.None)
            {
                Debug.LogWarning($"{this.name}.stages's bossMonster is none value");
                continue;
            }

            if (info.stageMonsters.Length < 1)
            {
                Debug.LogWarning($"{this.name}.stages's stageMonsters is null");
                continue;
            }

            list.Clear();
            foreach(MonsterCharacter character in info.stageMonsters)
            {
                if(character == MonsterCharacter.None)
                {
                    Debug.LogWarning($"{this.name}.stages's stageMonsters has none value");
                    continue;
                }

                if(list.Contains(character))
                {
                    Debug.LogWarning($"{this.name}.stages's stageMonsters has duplicate value");
                    continue;
                }

                list.Add(character); 
            }

        }
    }
#endif
}
