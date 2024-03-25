using System;
using UnityEngine;
using UnityEngine.Events;

public static class EventHandler
{

    #region Scene Event
    // 게임이 처음 실행될 때 DB에서 데이터 로드가 끝나면 실행할 함수들
    // DataManager.LoadJsonData : 저장된 Json 파일을 불러와 GameDatas에 저장
    // LoadingImage.FadeOut : Fade Out을 하는 함수
    public static event Action DBDataLoadEvent;
    public static void CallDBDataLoadEvent() { DBDataLoadEvent?.Invoke(); }

    // 씬 전환 전 FadeOut을 할 때 실행할 함수들
    // LoadingImage.FadeOut : Fade Out을 하는 함수
    // DataManager.SaveWaterTileData : 현재 씬의 Dug/Water Tile Data 저장

    // Joystick.InActive
    public static event Action BeforeSceneUnloadFadeOutEvent;
    public static void CallBeforeSceneUnloadFadeOutEvent() { BeforeSceneUnloadFadeOutEvent?.Invoke(); }

    // FadeOut 후 씬을 언로드 하기 전에 실행할 함수들
    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent() { BeforeSceneUnloadEvent?.Invoke(); }

    // 새로운 씬을 로드한 후 실행할 함수들
    public static event Action AfterSceneLoadEvent;
    public static void CallAfterSceneLoadEvent() { AfterSceneLoadEvent?.Invoke(); }

    // 씬의 데이터(타일, Prefab)을 로드한 후 FadeIn 하기 전에 실행할 함수들
    // Player.ActivePlayer
    // Joystick.InitStickDirection : 플레이어의 방향으로 스틱 방향 변경
    // NightMask.CheckScene
    // LobbyButton.ActiveButton
    public static event Action AfterSceneLoadBeforeFadeInEvent;
    public static void CallAfterSceneLoadBeforeFadeInEvent() { AfterSceneLoadBeforeFadeInEvent?.Invoke(); }

    // 씬 전환 후 FadeIn을 한 후 실행할 함수
    public static event Action AfterSceneLoadFadeInEvent;
    public static void CallAFterSceneLoadFadeInEvent() { AfterSceneLoadFadeInEvent?.Invoke(); }
    #endregion

    #region Time Event
    // 게임 내 시간을 Pause/Resume 할 때 샐행할 함수들
    /*public static event Action<bool> TimePauseEvent;
    public static void CallTimePauseEvent(bool isPause) { TimePauseEvent?.Invoke(isPause); }*/
    
    // Time : Year Season Day Hour Minute
    // 1일 : 480초 / 1시간 : 20초 / 15분 : 10초

    // 분이 바뀔 때 호출하는 함수들을 추가할 델리게이트
    public static event Action AdvanceGameMinuteEvent;

    // 매개변수를 받아 AdvanceGameMinuteEvent를 호출하는 함수
    public static void CallAdvanceGameMinuteEvent()
    {
        AdvanceGameMinuteEvent?.Invoke();
    }

    public static event Action AdvanceGameHourEvent;

    public static void CallAdvanceGameHourEvent()
    {
        AdvanceGameHourEvent?.Invoke();
    }

    public static event Action AdvanceGameDateEvent;

    public static void CallAdvanceGameDateEvent()
    {
        AdvanceGameDateEvent?.Invoke();
    }

    public static event Action AdvanceGameSeasonEvent;

    public static void CallAdvanceGameSeasonEvent()
    {
        AdvanceGameSeasonEvent?.Invoke();
    }

    public static event Action AdvanceGameYearEvent;

    public static void CallAdvanceGameYearEvent()
    {
        AdvanceGameHourEvent?.Invoke();
    }

    public static event Action PlayerSleepEvent; // DayPassEvent
    public static void CallPlayerSleepEvent() { PlayerSleepEvent?.Invoke(); }

    // NightMask.NightFadeEvent
    public static event Action<bool> NightFadeEvent; // SunsetEvent
    public static void CallNightFadeEvent(bool day2night) { NightFadeEvent?.Invoke(day2night); }

    // NightMask.NightEvent
    public static event Action NightEvent;
    public static void CallNightEvent() { NightEvent?.Invoke(); }

    public static event Action<bool> NightMaskSwitch;
    public static void CallNightMaskSwitch(bool isNight) { NightMaskSwitch?.Invoke(isNight); }

    public static event Action GoldChangeEvent;
    public static void CallGoldChangeEvent ()
    {
        GoldChangeEvent?.Invoke();
    }


    /*public static event Action<Vector3Int, Vector3Int> AdvanceGameMinuteEvent;

    // 매개변수를 받아 AdvanceGameMinuteEvent를 호출하는 함수
    public static void CallAdvanceGameMinuteEvent(Vector3Int YearSeasonDay, Vector3Int HourMinuteGold)
    {
        AdvanceGameMinuteEvent?.Invoke(YearSeasonDay, HourMinuteGold);
    }

    public static event Action<Vector3Int, Vector3Int> AdvanceGameHourEvent;

    public static void CallAdvanceGameHourEvent(Vector3Int YearSeasonDay, Vector3Int HourMinuteGold)
    {
        AdvanceGameHourEvent?.Invoke(YearSeasonDay, HourMinuteGold);
    }

    public static event Action<Vector3Int, Vector3Int> AdvanceGameDateEvent;

    public static void CallAdvanceGameDateEvent(Vector3Int YearSeasonDay, Vector3Int HourMinuteGold)
    {
        AdvanceGameDateEvent?.Invoke(YearSeasonDay, HourMinuteGold);
    }

    public static event Action<Vector3Int, Vector3Int> AdvanceGameSeasonEvent;

    public static void CallAdvanceGameSeasonEvent(Vector3Int YearSeasonDay, Vector3Int HourMinuteGold)
    {
        AdvanceGameSeasonEvent?.Invoke(YearSeasonDay, HourMinuteGold);
    }

    public static event Action<Vector3Int, Vector3Int> AdvanceGameYearEvent;

    public static void CallAdvanceGameYearEvent(Vector3Int YearSeasonDay, Vector3Int HourMinuteGold)
    {
        AdvanceGameHourEvent?.Invoke(YearSeasonDay, HourMinuteGold);
    }*/
    #endregion

    #region Tile Event
    // 타일이 1개 바뀔 때 실행하는 이벤트
    // MapTileManager.SetTile
    public static event Action<Vector2Int, TilemapType> SetTileEvent;
    public static void CallSetTileEvent(Vector2Int coordinate, TilemapType type) { SetTileEvent?.Invoke(coordinate, type); }

    // MapTileManager.SetNullTile
    public static event Action<Vector2Int, TilemapType> SetNullTileEvent;
    public static void CallSetNullTileEvent(Vector2Int coordinate, TilemapType type) { SetNullTileEvent?.Invoke(coordinate, type); }

    // 타일 전체를 다시 그릴 때 실행하는 이벤트
    public static event Action SetAllTileEvent;
    public static void CallSetAllTileEvent() { SetAllTileEvent?.Invoke(); }

    // MapTileManager.TileToList
    public static event Action<TilemapType> SetTileToListEvent;
    public static void CallSetTileToListEvent(TilemapType type) { SetTileToListEvent?.Invoke(type); }
    #endregion

    #region UI Event
    // GameInfoUI.UpdateHPBar
    public static event Action<int, float> UpdateHPBarEvent;
    public static void CallUpdateHPBar(int currentHP, float divideValue) { UpdateHPBarEvent?.Invoke(currentHP, divideValue); }

    // SystemMassageUI.PrintMassage
    public static event Action<string, float> PrintSystemMassageEvent;
    public static void CallPrintSystemMassageEvent(string msg, float t = 0.5f) { PrintSystemMassageEvent?.Invoke(msg, t); }
    #endregion

    #region Inventory Event    
    public static event Action DragItemInactiveEvent;
    public static void CallDragItemInactiveEvent() { DragItemInactiveEvent?.Invoke(); }
        
    public static event Action<Inventories> SetInventoryEvent;
    public static void CallSetInventoryEvent(Inventories type) { SetInventoryEvent?.Invoke(type); }


    public static event Action<Inventories, Inventories, int> DragItemActiveEvent;
    public static void CallDragItemActiveEvent(Inventories panel, Inventories type, int index) { DragItemActiveEvent?.Invoke(panel, type, index); }
    
    public static event Action<Inventories, Inventories, int> DropItemeEvent;
    public static void CallDropItemeEvent(Inventories panel, Inventories type, int index) { DropItemeEvent?.Invoke(panel, type, index); }

    #endregion

    // Attack Player
    // Player.GetDamage
    public static event Action<int> AttackPlayerEvent;
    public static void CallAttackPlayerEvent(int dmg) { AttackPlayerEvent?.Invoke(dmg); }

    #region Player Event
    // PlayerActionButton.cs
    // Player.cs
    public static event Action<int> SeedChangeEvent;
    public static void CallSeedChangeEvent(int code) { SeedChangeEvent?.Invoke(code); }

    public static event Action<int, int> ActiveSeedMoveEvent;
    public static void CallActiveSeedMoveEvent(int to, int from) { ActiveSeedMoveEvent?.Invoke(to, from); }

    public static event Action UseSeedEvent;
    public static void CallUseSeedEvent() { UseSeedEvent?.Invoke(); }

    public static event Action<bool> CanFishEvent;
    public static void CallCanFishEvent(bool canFish/*, FishType type = FishType.none*/) { CanFishEvent?.Invoke(canFish); }
    #endregion

    // Fishing
    public static event Action<bool, FishType> OpenFishPanelEvent;
    public static void CallOpenFishPanel(bool open, FishType type = FishType.count) { OpenFishPanelEvent?.Invoke(open, type); }

    #region Interactor
    public static event Action<InteractionType, SceneName> SetInteractionTypeEvent;
    public static void CallSetInteractionTypeEvent(InteractionType type, SceneName activeScene) { SetInteractionTypeEvent?.Invoke(type, activeScene); }

    public static event Action<bool> ActiveInteractorEvent;
    public static void CallActiveInteractor(bool active) { ActiveInteractorEvent?.Invoke(active); }

    public static event Action OpenPanelEvent;
    public static void CallOpenPanelEvent() { OpenPanelEvent?.Invoke(); }
    #endregion

    public static event Action NetworkErrorEvent;
    public static void CallNetworkErrorEvent() { NetworkErrorEvent?.Invoke(); }

    // Setting
    public static event Action ChangeSettingEvent;
    public static void CallChangeSetingEvent() { ChangeSettingEvent?.Invoke(); }


    public static event Action CreateMineEvent;
    public static void CallCreateMineEvent() { CreateMineEvent?.Invoke(); }

    public static event Action<bool> EnemyDieEvent;
    public static void CallEnemyDieEvent(bool enqueue = false) { EnemyDieEvent?.Invoke(enqueue); }

    public static event Action<MenuIndex> OpenMenuEvent;
    public static void CallOpenMenuEvent(MenuIndex menuID) { OpenMenuEvent?.Invoke(menuID); }


    public static event Action GameClearEvent;
    public static void CallGameClearEvent() { GameClearEvent?.Invoke(); }
    public static event Action GameOverEvent;
    public static void CallGameOverEvent() { GameOverEvent?.Invoke(); }

    // OreManager
    public static event Action<Vector2Int> RemoveOreAtOreDataEvent;
    public static void CallRemoveOreAtOreData(Vector2Int coordinate) { RemoveOreAtOreDataEvent?.Invoke(coordinate); }

    // SleepPanalty
    public static event Action SleepPanaltyEvent;
    public static void CallSleepPanaltyEvent() { SleepPanaltyEvent?.Invoke(); }

    // TreeManager
    public static event Action<Vector2Int> RemoveTreeAtTreeDataEvent;
    public static void CallRemoveTreeAtTreeData(Vector2Int coordinate) { RemoveTreeAtTreeDataEvent?.Invoke(coordinate); }

    // Admob
    public static event Action<AdType, UnityAction> ShowAdEvent;
    public static void CallShowAdEvent(AdType type, UnityAction done = null) { ShowAdEvent?.Invoke(type, done); }

    // Load Lobby
    public static event Action LoadLobby;
    public static void CallLoadLobby() { LoadLobby?.Invoke(); }

    public static event Action SavePlayerPosition;
    public static void CallSavePlayerPosition() { SavePlayerPosition?.Invoke(); }

    public static event Action<bool> LoadingPanelEvent;
    public static void CallLoadingPanelEvent(bool active) { LoadingPanelEvent?.Invoke(active); }

    public static event Action<string> UnlockAchievementEvent;
    public static void CallUnlockAchievementEvent(string id) { UnlockAchievementEvent?.Invoke(id); }
    public static event Action<string> IncrementAchievementEvent;
    public static void CallIncrementAchievementEvent(string id) { IncrementAchievementEvent?.Invoke(id); }

    public static event Action<ProductID> PurchaseProductEvent;
    public static void CallPurchaseProductEvent(ProductID id) { PurchaseProductEvent?.Invoke(id); }
    public static event Action<ProductID> AlreadyPurchasedProductEvent;
    public static void CallAlreadyPurchasedProductEvent(ProductID id) { AlreadyPurchasedProductEvent?.Invoke(id); }
    public static Action OpenMarketEvent;
    public static void CallOpenMarketEvent() { OpenMarketEvent?.Invoke(); }

    public static Action StartGameEvent;
    public static void CallStartGameEvent() { StartGameEvent?.Invoke(); }

    //public static Action<MiniGameIndex> OpenMiniGameCanvasEvent;
    //public static void CallOpenMiniGameCanvasEvent(MiniGameIndex idx) { OpenMiniGameCanvasEvent?.Invoke(idx); }
}
