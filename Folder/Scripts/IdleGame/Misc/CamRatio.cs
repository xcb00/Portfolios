using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamRatio : MonoBehaviour
{
    [SerializeField] StaticVariables variables;
    [SerializeField] CanvasScaler cameraCanvas;
    [SerializeField] CanvasScaler overlayCanvas;
    Vector2Int targetScreenRatio = new Vector2Int(9, 16);
    Vector2Int targetScreenSize = new Vector2Int(1080, 1920);

    private void Awake()
    {
        Camera cam = GetComponent<Camera>();

        Rect rect = cam.rect;

        float sH = (float)Screen.width / Screen.height / ((float)targetScreenRatio.x / targetScreenRatio.y);
        float sW = 1f / sH;

        Vector3 adjust = Vector3.zero;

        if (sH < 1)
        {
            rect.height = sH;
            rect.y = (1f - sH) / 2f;
            adjust = new Vector3(0f, (targetScreenSize.y / sH - targetScreenSize.y) * 0.5f, 0f);
            variables.dragAdjust = (float)targetScreenSize.x / (float)Screen.width;
        }
        else
        {
            rect.width = sW;
            rect.x = (1f - sW) / 2f;
            adjust = new Vector3((targetScreenSize.x / sW - targetScreenSize.x) * 0.5f, 0f, 1f);
            variables.dragAdjust = (float)targetScreenSize.y / (float)Screen.height;
        }
        cam.rect = rect;

        cameraCanvas.matchWidthOrHeight = overlayCanvas.matchWidthOrHeight = adjust.z;
        overlayCanvas.transform.GetChild(0).GetComponent<RectTransform>().offsetMax = -adjust;
        overlayCanvas.transform.GetChild(0).GetComponent<RectTransform>().offsetMin = adjust;
    }
    private void OnPrecull() => GL.Clear(true, true, Color.white);

}
