using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    public static Action ResetCharacterSlotEvent;
    public static void CallResetCharacterSlotEvent() => ResetCharacterSlotEvent?.Invoke();

    public static Action ResetCharacterCardEvent;
    public static void CallResetCharacterCardEvent() => ResetCharacterCardEvent?.Invoke();


    public static Action<int, string> SetCharacterEvent;
    public static void CallSetCharacterEvent(int slot, string character) => SetCharacterEvent?.Invoke(slot, character);


    public static Action<UIType, bool> ActiveUIPanelEvent;
    public static void CallActiveUIPanelEvent(UIType type, bool active) => ActiveUIPanelEvent?.Invoke(type, active);

    public static Action<int, int> OpenRewardWindonEvent;
    public static void CallOpenRewardWindowEvent(int maxStage, int reward) => OpenRewardWindonEvent?.Invoke(maxStage, reward);

    public static Action<PlayerCharacter> ActivePlayerCharacterEvent;
    public static void CallActivePlayerCharacterEvent(PlayerCharacter character) => ActivePlayerCharacterEvent?.Invoke(character);

    public static Action<PlayerCharacter> InactivePlayerCharacterEvent;
    public static void CallInactivePlayerCharacterEvent(PlayerCharacter character) => InactivePlayerCharacterEvent?.Invoke(character);

    public static Action<Transform> EnquequeMonsterEvent;
    public static void CallEnquequeMonsterEvent(Transform trans) => EnquequeMonsterEvent?.Invoke(trans);

    public static Action<Action, Action> FadeInEvent;
    public static void CallFadeInEvent(Action beforeFadeIn = null, Action afterFadeIn = null) => FadeInEvent?.Invoke(beforeFadeIn, afterFadeIn);

    public static Action<Action, Action> FadeOutEvent;
    public static void CallFadeOutEvent(Action beforeFadeOut = null, Action afterFadeOut = null) => FadeOutEvent?.Invoke(beforeFadeOut, afterFadeOut);

    public static Action<int> ChangeStageTextEvent;
    public static void CallChangeStageTextEvent(int stage) => ChangeStageTextEvent?.Invoke(stage);

    public static Action UpdateGoldEvent;
    public static void CallUpdateGoldEvent() => UpdateGoldEvent?.Invoke();

    public static Action ResetPositionEvent;
    public static void CallResetPositionEvent() => ResetPositionEvent?.Invoke();
}
