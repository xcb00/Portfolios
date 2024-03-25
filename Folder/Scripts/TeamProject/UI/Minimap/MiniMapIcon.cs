using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapIcon : IconProperty
{
   
    public void SetTarget(Transform target, Color color)
    {
        myTarget = target;
        myImage.enabled = false;
        myImage.color = color;
        IconPosition();
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (myTarget != null)
        {
            IconPosition();
        }
    }

    void IconPosition()
    {
        Vector3 pos = Camera.allCameras[Camera.allCameras.Length - 1].WorldToViewportPoint(myTarget.position);
        if (!myImage.enabled && (pos.x <= 1f || pos.x>=0f)) myImage.enabled = true;
        if (myImage.enabled && (pos.x > 1f||pos.x<0f)) myImage.enabled = false;
        pos.x *= 1200.0f;
        pos.y *= 350.0f;
        myRect.anchoredPosition = pos;
    }
}
