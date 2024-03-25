using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperty : CharacterMovement
{
    protected bool isFishing = false;

    #region Components
    //[SerializeField] protected Joystick myJoystick;
    [SerializeField] protected InputManager myInput;
    [SerializeField] protected PlayerActionButton myAction;

    #endregion
    SpriteRenderer _renderer;
    [SerializeField]protected int seedCode = 10001;

    protected SpriteRenderer myRenderer
    {
        get
        {
            if (_renderer == null)
            {
                _renderer = GetComponentInParent<SpriteRenderer>();
                if (_renderer == null)
                    _renderer = GetComponentInChildren<SpriteRenderer>();
            }
            return _renderer;
        }
    }
}
