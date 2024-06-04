using System;

public static class EventHandler
{
    #region Fade Event
    public static Action FadeInEvent;
    public static void CallFadeInEvent() => FadeInEvent?.Invoke();
    public static Action FadeOutEvent;
    public static void CallFadeOutEvent() => FadeOutEvent?.Invoke();

    public static Action<bool> FadeDelayEvent;
    public static void CallFadeDelayEvent(bool startFade) => FadeDelayEvent?.Invoke(startFade);
    #endregion

    #region Start Game Event
    public static Action StartSingleGameEvent;
    public static void CallStartSingleGameEvent() => StartSingleGameEvent?.Invoke();
    #endregion

    public static Action CheckPlayerEvent;
    public static void CallCheckPlayerEvent() => CheckPlayerEvent?.Invoke();

    public static Action<string> ShowMoneyMessageEvent;
    public static void CallShowMoenyMessageEvent(string msg) => ShowMoneyMessageEvent?.Invoke(msg);

    public static Action LoadingPanelInactiveEvent;
    public static void CallLoadingPanelInactiveEvent() => LoadingPanelInactiveEvent?.Invoke();
}
