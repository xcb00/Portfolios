using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilPreview : BuildPreview
{
    [SerializeField] int price = 50;
    public override void Build(RaycastHit hit)
    {
        if (ResourceManager.Instance.Gold < price)
        {
            EventHandler.CallPrintSystemMassageEvent("Gold�� �����մϴ�");
            return;
        }


        base.Build(hit);
        if (colliders.Length < 1)
        {
            ResourceManager.Instance.AddGold(-price);
            GameObject tmp = PoolManager.Instance.DeququeObject(PoolType.Soil);
            tmp.transform.position = new Vector3(xPos, 0.5f, zPos);
            tmp.transform.rotation = Quaternion.identity;
            tmp.SetActive(true);
            GameDatas.soilQueue.Enqueue(tmp.transform);
            ResourceManager.Instance.AddMaxPopulation(false);
        }
        else
        {
            EventHandler.CallPrintSystemMassageEvent("���� ��ġ�� �ǹ��� ���� �� �����ϴ�");
            InactivePreview();
        }
    }
}
