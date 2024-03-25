using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasRatio : MonoBehaviour
{
    private void Start()
    {
        GetComponent<CanvasScaler>().matchWidthOrHeight = Settings.Instance.canvasRatio.z;
        Vector2 adjust = new Vector2(Settings.Instance.canvasRatio.x, Settings.Instance.canvasRatio.y);
        transform.GetChild(0).GetComponent<RectTransform>().offsetMax = -adjust;
        transform.GetChild(0).GetComponent<RectTransform>().offsetMin = adjust;
    }
}
