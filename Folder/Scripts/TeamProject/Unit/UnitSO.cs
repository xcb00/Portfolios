using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="UnitDetailList", menuName ="Scriptable Object/Unit")]
public class UnitSO : ScriptableObject
{
    public List<PlayerUnitDetails> playerUnitDetailList;
    public List<ComUnitDetails> comUnitDetailList;
    public List<BossSkillDetails> bossSkillDetailsList;
}
