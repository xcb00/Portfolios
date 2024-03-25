using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerPreview : BuildPreview
{
    [SerializeField] GameObject build;
    public override void Build(RaycastHit hit)
    {
        base.Build(hit);
        InactivePreview();
        if (colliders.Length>0)
        {
            EventHandler.CallPrintSystemMassageEvent("���� ��ġ�� ���� �� �����ϴ�");
            return;
        }
        build.transform.position = new Vector3(xPos, 0.5f, zPos);
        EventHandler.CallBannerchangeEvent(false);
    }
}
