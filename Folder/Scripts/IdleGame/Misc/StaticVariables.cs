using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticVariables : Singleton<StaticVariables>
{
    public RectTransform gameCharacterInfo;
    public float dragAdjust;
    public CharacterSO characterSO;
    public StageSO stageSO;
    public Material hurtMat;

    protected override void Awake()
    {
        base.Awake();
        characterSO = Resources.Load("ScriptableObject/CharacterStatusSO") as CharacterSO;
        stageSO = Resources.Load("ScriptableObject/StageSO") as StageSO;
    }
}
