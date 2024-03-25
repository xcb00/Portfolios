using UnityEngine;
public class Settings :Singleton<Settings>
{
    // const
    #region LoadingImage.cs
    [Header("LoadingImage.cs"), Space(10)]
    public float fadeTime = 0.5f;
    #endregion

    #region DoubleClick.cs
    [Header("DoubleClick.cs"), Space(10)]
    public float doubleClickTime = 0.3f;
    #endregion

    #region TimeManager.cs
    [Header("TimeManager.cs"), Space(10)]
    public float secondsPerMinute = 300f;
    public int monthDays = 45;
    #endregion

    #region PlayerActionButton.cs
    [Header("PlayerActionButton.cs"), Space(10)]
    public float toolButtonDistance = 180.0f;
    public float toolButtonTimeToMove = 0.1f;
    #endregion

    #region NightMask.cs
    [Header("NightMask.cs"), Space(10)]
    public float nightAlpha = 0.7f;
    public float nigthFadeTime = 0.0f;
    #endregion

    #region Scanery.cs
    [Header("Scanery.cs"), Space(10)]
    public float scaneryFadeTime = 0.1f;
    public float scaneryAlpha = 0.3f;
    #endregion

    #region InventoryManager.cs
    [Header("InventoryManager.cs"), Space(10)]
    public int maxQuantity = 4;
    public InventoryCapacity[] inventoryCapacity;
    #endregion

    public int shopItemCount;

    // 바뀔 수 있는 값
    #region Player.cs
    [Header("Player.cs"), Space(10)]
    public float playerTimeToMove = 0.4f;
    public float playerCursorTurnTime = 0.3f;
    public bool activeCursor = true;
    #endregion

    // 세팅에서 바꿀 수 있는 값
    #region Joystick.cs
    [Header("Joystick.cs"), Space(10)]
    // 100~250
    public float padSize = 100.0f;
    public float stickScale = 0.5f;
    public float minDistanceToMove = 0.75f;
    public float stickRange = 0.5f;
    #endregion

    public Vector3 canvasRatio = Vector2.zero;

    public int volume;

    public bool activeToolMenu;
    public bool interactingAnimation;
    public bool useJoystick;

    // 대출 이자
    // 1년 동안 최소한으로 상환해야하는 이자
    // 상환하지 못할 경우 Game Over
    public int startLoan = 50000000;
    public float loanInterest = 2.0f;

}
