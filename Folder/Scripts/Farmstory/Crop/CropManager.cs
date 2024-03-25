using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropManager : Singleton<CropManager>
{
    //List<CropPrefab> removeCropPrefab = new List<CropPrefab>();

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadBeforeFadeInEvent += LoadCrop;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadBeforeFadeInEvent -= LoadCrop;
    }

    void LoadCrop()
    {
        if (GameDatas.currentScene!=SceneName.Farm || GameDatas.mapTileList[(int)TilemapType.diggable - 1].Count < 1)
            return;
        GameDatas.cropPrefabList.Clear();
        List<CropPrefabJson> dataList = DataManager.Instance.LoadDataToJson<CropPrefabJson>(JsonDataName.CropPrefab, GameDatas.currentScene);
        if (dataList.Count > 0)
        {
            foreach (CropPrefabJson data in dataList)
            {
                // Vector2Int�� ����ü�̱� ������ Find�� ����� ��� null���� ��ȯ�Ǵ� ���� �ƴ� Vector2Int.zero���� ��ȯ��                    
                if (GameDatas.removeCropCoordinateList.FindIndex(x => x == data.coordinate) < 0)
                    SpawnCrop(data.coordinate, data.seedCode, false);
            }
        }
        DataManager.Instance.SaveCropPrefabData();
    }

    // CropTile�� �ε��� �� �����ؾ� ��
    public bool SpawnCrop(Vector2Int coordinate, int seedCode, bool useSeed = true)
    {
        GameObject crop = PoolManager.Instance.DequeueObject(PoolPrefabName.crop);
        if (!crop.GetComponent<CropPrefab>().SpawnCropPrefab(GameDatas.cropDetailsList.Find(x => x.seedCode == seedCode), coordinate)) return false;

        GameDatas.cropPrefabList.Add(crop.GetComponent<CropPrefab>());
        crop.SetActive(true);

        if(useSeed)
            EventHandler.CallUseSeedEvent();
        return true;
    }

    public bool Harvest(Vector2Int coordinate)
    {
        GameDatas.cropPrefabList.Find(x => x.coordinate == coordinate).Harvest();
        return true;
    }
}
