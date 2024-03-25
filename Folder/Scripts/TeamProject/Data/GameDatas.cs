using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDatas
{
    //public static UnitSO unitSO;
    public static List<PlayerUnitDetails> playerUnitDetailList;
    public static List<ComUnitDetails> comUnitDetailList;
    public static List<BossSkillDetails> bossSkillDetailsList;
    public static List<Item> itemList;
    public static int[] enhanceProbability;
    // List�� �ƴ� Queue�� �ϴ� ���� : List�� Remove �ϱ� ���� �ٸ� ������ Transform�� ������ �ߺ��� �� ����
    public static Queue<Transform> soilQueue = new Queue<Transform>();
    public static Queue<Transform> treeQueue = new Queue<Transform>();

    public static int stageLevel;
    public static int maxStage;

    public static bool pause;
    public static bool stopUnit;
}
