using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public float dragSpeed = 1.0f;
    public float pinchSpeed = 1.0f;
    float pinch = 4.0f;
    public Vector2 pinchRange = new Vector2(1.0f, 8.0f);
    Camera cam = null;
    private void OnEnable()
    {
        if (cam == null)
        {
            cam = GetComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = pinch;
        }

        cam.transform.localPosition = new Vector3(DungeonBuilder.Inst.currentRoom.x, DungeonBuilder.Inst.currentRoom.y, 0f);
    }

    public void PinchEvent(float delta)
    {
        Debug.Log(delta);
        pinch += delta * pinchSpeed * Time.deltaTime;
        cam.orthographicSize = Mathf.Clamp(pinch, pinchRange.x, pinchRange.y);
    }

    public void DragEvent(Vector2 delta)
    {
        Vector3 _delta = Vector3.zero;
        _delta.x = delta.x;
        _delta.y = delta.y;
        transform.localPosition -= _delta * (dragSpeed * (pinch / pinchRange.y)) * Time.deltaTime;
    }
}
