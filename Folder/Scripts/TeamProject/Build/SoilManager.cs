using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilManager : Singleton<SoilManager>
{
    public LayerMask cantBuild;
    public BuildPreview[] buildingPreview = null;
    public GameObject soil = null;
    public bool previewActive = false;
    public float zPos = 2.0f;
    public void FollowCursor(RaycastHit hit)
    {
        float x = Mathf.Round(hit.point.x);
        //buildingPreview[0].SetPosition(new Vector3(x, 0.5f, zPos));
    }

    public void ActivePreview()
    {
        previewActive = true;
        buildingPreview[0].gameObject.SetActive(true);
    }

    public void InativePreview()
    {
        previewActive = false;
        buildingPreview[0].gameObject.SetActive(false);
    }

    public bool Build(RaycastHit hit)
    {
        float x = Mathf.Round(hit.point.x);
        Collider[] colliders = Physics.OverlapSphere(new Vector3(x, 0.5f, zPos), 1f, cantBuild);
        if (colliders.Length < 1)
        {
            GameObject tmp = Instantiate(soil, new Vector3(x, 0.5f, zPos), Quaternion.identity, transform);            
            GameDatas.soilQueue.Enqueue(tmp.transform);
            ResourceManager.Instance.AddMaxPopulation(false);
            return true;
        }
        else
        {
            EventHandler.CallPrintSystemMassageEvent("현재 위치에 건물을 지을 수 없습니다");
            InativePreview();
            return false; 
        }
    }

}
