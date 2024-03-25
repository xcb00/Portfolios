using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterProperty : MonoBehaviour
{
    // Variable
    protected CharacterState myState = CharacterState.none;
    protected CharacterDirection myDirection = CharacterDirection.down;
    public CharacterDirection MyDirection { get { return myDirection; } }
    protected bool slow = false;

    // Component
    Animator _animator;
    protected Animator myAnimator
    {
        get
        {
            if (_animator == null)
            {
                _animator = GetComponentInParent<Animator>();
                if (_animator == null)
                    _animator = GetComponentInChildren<Animator>();
            }
            return _animator;
        }
    }

    Collider2D _collider;
    protected Collider2D myCollider
    {
        get
        {
            if (_collider == null)
            {
                _collider = GetComponentInParent<Collider2D>();
                if (_collider == null)
                    _collider = GetComponentInChildren<Collider2D>();
            }
            return _collider;
        }
    }

}
