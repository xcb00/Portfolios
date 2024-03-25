using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Panel
{
    // Player.cs > ActivePlayer() > myJoystick.stickDirection = GameDatas.playerData[0].direction;
    // PlayerCursor.cs > TurningCursor() > cursorDirection = myJoystick.stickDirection;
    // Player.cs > Update
    // Player.cs > CheckDirection()
    [SerializeField] Joystick joystick;
    [SerializeField] KeyArrowInput arrowkey;



    private void Start()
    {
        Init();
    }

    public bool isMove => Settings.Instance.useJoystick ? joystick.isMove : arrowkey.isMove;
    public CharacterDirection inputDirection => Settings.Instance.useJoystick ? joystick.inputDir : arrowkey.inputDir;
    public void SetInputDirection(CharacterDirection dir)
    {
        joystick.inputDir = dir;
        arrowkey.inputDir = dir;
    }

    void OnEnable()
    {
        EventHandler.ChangeSettingEvent += ChangeSetting;
        EventHandler.AfterSceneLoadBeforeFadeInEvent += ActivePanel;
        EventHandler.BeforeSceneUnloadFadeOutEvent += InactivePanel;
    }
    void OnDisable()
    {
        EventHandler.ChangeSettingEvent -= ChangeSetting;
        EventHandler.AfterSceneLoadBeforeFadeInEvent -= ActivePanel;
        EventHandler.BeforeSceneUnloadFadeOutEvent -= InactivePanel;
    }

    void ChangeSetting()
    {
        joystick.gameObject.SetActive(Settings.Instance.useJoystick);
        arrowkey.gameObject.SetActive(!Settings.Instance.useJoystick);
    }

}
