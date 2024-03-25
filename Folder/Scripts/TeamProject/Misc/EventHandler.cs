using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    #region Wave Event
    public static Action StartWaveEvent; // WaveManager.StartWave
    public static void CallStartWaveEvent() { StartWaveEvent?.Invoke(); }

    public static Action EndWaveEvent; // WaveManager.EndWave
    public static void CallEndWaveEvent() { EndWaveEvent?.Invoke(); }

    public static Action<bool> StopRespawnEvent;
    public static void CallStopRespawnEvent(bool stop) { StopRespawnEvent?.Invoke(stop); }
    #endregion

    #region Time Event
    public static Action AdvanceTimeEvent;
    public static void CallAdvanceTimeEvent() { AdvanceTimeEvent?.Invoke(); }

    public static Action AdvanceDayEvent;
    public static void CallAdvanceDayEvent() { AdvanceDayEvent?.Invoke(); }

    public static Action<bool> PauseTimeEvent;
    public static void CallPauseTimeEvent(bool pause) { PauseTimeEvent?.Invoke(pause); }
    #endregion

    #region UI Event
    public static Action<string> PrintSystemMassageEvent;
    public static void CallPrintSystemMassageEvent(string msg) { PrintSystemMassageEvent?.Invoke(msg); }

    #endregion

    #region Player Event
    public static Action<float, bool> PlayerMoveEvent;
    public static void CallPlayerMoveEvent(float inputValue, bool isRun) { PlayerMoveEvent?.Invoke(inputValue, isRun); }

    public static Action<int> PlayerAttackEvent;
    public static void CallPlayerAttackEvent(int attackType) { PlayerAttackEvent?.Invoke(attackType); }

    public static Action<int> PlayerEquipmentEvent;
    public static void CallPlayerEquipmentEvent(int index) { PlayerEquipmentEvent?.Invoke(index); }

    #endregion

    #region Input Event
    public static Action<bool> CanUseSkillEvent;
    public static void CallCanUseSkillEvent(bool b) { CanUseSkillEvent?.Invoke(b); }

    public static Action<bool> CanAttackEvent;
    public static void CallCanAttackEvent(bool b) {CanAttackEvent?.Invoke(b); }

    public static Action<bool> CoolDownEvent;
    public static void CallCoolDownEvent(bool firstSkill) { CoolDownEvent?.Invoke(firstSkill); }
    #endregion

    public static Action<bool> BannerChangeEvent;
    public static void CallBannerchangeEvent(bool moveForward) { BannerChangeEvent?.Invoke(moveForward); }

   /* public static Action BeforeFadeEvent;
    public static void CallBeforeFadeEvent() { BeforeFadeEvent?.Invoke(); }*/

    public static Action StageClearEvent;
    public static void CallStageClearEvent() { StageClearEvent?.Invoke(); }

    public static Action StageStartEvent;
    public static void CallStageStartEvent() { StageStartEvent?.Invoke(); }

    public static Action<float> BossRespawnEvent;
    public static void CallBossRespawnEvent(float hp) { BossRespawnEvent?.Invoke(hp); }

    public static Action<float> BossGetDamageEvent;
    public static void CallBossGetDamageEvent(float hp) { BossGetDamageEvent?.Invoke(hp); }

    public static Action<SceneChange> PanelTextEvent;
    public static void CallPanelTextEvent(SceneChange sc) { PanelTextEvent?.Invoke(sc); }

    public static Action<Transform> PoolClearEvent;
    public static void CallPoolClearevent(Transform pool) { PoolClearEvent?.Invoke(pool); }

    public static Action ShowExplanation;
    public static void CallshowExplanation() { ShowExplanation?.Invoke(); }
}
