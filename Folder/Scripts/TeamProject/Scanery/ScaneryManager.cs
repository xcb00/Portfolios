using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaneryManager : MonoBehaviour
{
    [SerializeField] GameObject[] treePrefabs;
    [SerializeField] Vector2 spawnRange;
    [SerializeField] int count = 0;
    [SerializeField] List<float> canCreate = new List<float>();
    float xPos = 0.0f;

    private void Start()
    {
        CreateScanery();
    }

    void CreateScanery()
    {
        xPos = spawnRange.x;

        while (xPos <= spawnRange.y)
        {
            if (count-- > 0)
            {
                float delta = Random.Range(4, 10) * 0.5f;
                if (delta > 3.5f) canCreate.Add(xPos);
                xPos += delta;
                CreatePrefab(xPos);
            }
            else
                break;
        }

        if (count > 0)
        {
            for(int i=0;i<canCreate.Count;i++)
            {
                CreatePrefab(canCreate[i]+2.0f);
            }
        }
    }

    void CreatePrefab(float xPos)
    {
        GameObject obj = Instantiate(treePrefabs[Random.Range(0, treePrefabs.Length)], new Vector3(xPos, 0.5f, 2.0f), Quaternion.identity);
        obj.transform.SetParent(transform);
    }
}
