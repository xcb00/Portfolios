using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamRatio : MonoBehaviour
{
    [SerializeField] Vector2Int targetScreenSize;
    [SerializeField] Vector2Int targetScreenRatio;

    private void Awake()
    {
        Camera cam = GetComponent<Camera>();

        Rect rect = cam.rect;

        float sH = (float)Screen.width / Screen.height / ((float)targetScreenRatio.x / targetScreenRatio.y);
        float sW = 1f / sH;

        Vector2 adjust = Vector2.zero;

        if (sH < 1)
        {
            rect.height = sH;
            rect.y = (1f - sH) / 2f;

            Settings.Instance.canvasRatio = new Vector3(0f, (targetScreenSize.y / sH - targetScreenSize.y) * 0.5f, 0f);
        }
        else
        {
            rect.width = sW;
            rect.x = (1f - sW) / 2f;

            Settings.Instance.canvasRatio = new Vector3((targetScreenSize.x / sW - targetScreenSize.x) * 0.5f, 0f, 1f);
        }
        cam.rect = rect;
    }
    private void OnPrecull() => GL.Clear(true, true, Color.black);
}
