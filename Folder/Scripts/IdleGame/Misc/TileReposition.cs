using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileReposition : MonoBehaviour
{
    Vector3 origin = Vector3.zero;

    private void Awake() => origin = transform.position;

    private void OnEnable() => EventHandler.ResetPositionEvent += ResetPosition;
    private void OnDisable() => EventHandler.ResetPositionEvent -= ResetPosition;

    void ResetPosition()=>transform.position= origin;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (GameManager.inst.EndGame)
            return;
        if (((1 << (int)LayerName.CameraArea) & (1 << collision.gameObject.layer)) != 0)
        {
            Vector3 diff = GameManager.inst.GetCameraPosition - transform.position;
            Vector2 dir = GameManager.inst.GetCameraDirection();
            dir.x = dir.x < 0 ? -1f : 1f;
            dir.y = dir.y < 0 ? -1f : 1f;

            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
                transform.Translate(Vector3.right * dir.x * 32f);
            else
                transform.Translate(Vector3.up * dir.y * 32f);
        }
    }
}
