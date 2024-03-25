using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackUnit : PlayerUnit
{
    public Transform banner = null;


    private void OnEnable()
    {
        EventHandler.BannerChangeEvent += BannerChange;
        attackUnit = true;
    }

    private void OnDisable()
    {
        EventHandler.BannerChangeEvent -= BannerChange;
    }

    public void SetTarget(Transform trans) { banner = trans; }

    void BannerChange(bool moveForward)
    {
        myState = UnitStateType.none;
        arrive = false;
        if (!this.moveForward)
            this.moveForward = moveForward;
        ChangeState(UnitStateType.move);
    }

    protected override void MoveState()
    {
        transform.GetChild(0).transform.rotation = Quaternion.identity;
        if (moveForward) MoveForward();
        else MoveTargetPosition();
    }

    void MoveTargetPosition()
    {
        float dist = GetDistance(banner.position.x, out bool isRight) + ((int)type == 2 ? Random.Range(-0.25f, 1.5f) : Random.Range(-1.5f, 0.25f));

        
        if (!arrive)
        {
            Vector3 dir = isRight ? Vector3.right : Vector3.left;

            if (stateRoutine != null) StopCoroutine(stateRoutine);

            if (Mathf.Abs(dist) < 2.5f)
            {
                arrive = true;
                ChangeState(UnitStateType.idle);
            }
            else
                stateRoutine = StartCoroutine(MovingPosition(dist, dir, () => ChangeState(UnitStateType.idle), true));
        }
        else
        {

            transform.GetChild(0).transform.rotation = Quaternion.identity;
            stateRoutine = StartCoroutine(MovingPosition(0.7f, Vector3.right, () => ChangeState(UnitStateType.idle), true));
        }
    } 

    void MoveForward()
    {
        base.MoveState();
        if (stateRoutine != null) StopCoroutine(stateRoutine);
        stateRoutine = StartCoroutine(MovingForward());
    }
}
