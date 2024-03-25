using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitProperty : UnitMovement
{
    [SerializeField] protected PlayerUnitType type;
    protected float unitDamage = 0.0f;
    protected float unitDelay = 0.0f;
    protected float unitSpeed = 0.0f;
    protected float unitHP = 0.0f;
    protected bool attackUnit = false;
    protected bool moveForward = false;
}
