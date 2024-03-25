using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageLevel", menuName = "Scriptable Object/Stage")]

public class StageSO : ScriptableObject
{
    public StageLevel[] stageLevel;
}