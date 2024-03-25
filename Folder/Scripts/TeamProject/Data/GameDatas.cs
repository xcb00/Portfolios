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
    // List가 아닌 Queue로 하는 이유 : List를 Remove 하기 전에 다른 유닛이 Transform을 가져와 중복될 수 있음
    public static Queue<Transform> soilQueue = new Queue<Transform>();
    public static Queue<Transform> treeQueue = new Queue<Transform>();

    public static int stageLevel;
    public static int maxStage;

    public static bool pause;
    public static bool stopUnit;
}
