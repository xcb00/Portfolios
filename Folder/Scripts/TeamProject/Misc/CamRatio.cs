using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamRatio : MonoBehaviour
{
    [SerializeField] CanvasScaler gameCanvas;
    [SerializeField] CanvasScaler systemCanvas;
    [SerializeField] RectTransform gameCanvasUI;

    private void Awake()
    {
        Camera cam = GetComponent<Camera>();
         Rect rect = cam.rect;
        float sH = ((float)Screen.width / Screen.height) / ((float)16 / 9);
        float sW = 1f / sH;
        if (sH < 1)
        {
            rect.height = sH;
            rect.y = (1f - sH) / 2f;
            gameCanvas.matchWidthOrHeight = 0f;
            systemCanvas.matchWidthOrHeight = 0f;
        }
        else
        {
            rect.width = sW;
            rect.x = (1f - sW) / 2f;
            gameCanvas.matchWidthOrHeight = 1f;
            systemCanvas.matchWidthOrHeight = 1f;
            Vector2 adjust = new Vector2((1920f / sW - 1920f) * 0.5f, 0f);
            gameCanvasUI.offsetMax = -adjust;
            systemCanvas.GetComponent<RectTransform>().offsetMax = -adjust;
            gameCanvasUI.offsetMin = adjust;
            systemCanvas.GetComponent<RectTransform>().offsetMin = adjust;
        }
        cam.rect = rect;
    }

    private void OnPreCull()
    {
        GL.Clear(true, true, Color.black);
    }
}
