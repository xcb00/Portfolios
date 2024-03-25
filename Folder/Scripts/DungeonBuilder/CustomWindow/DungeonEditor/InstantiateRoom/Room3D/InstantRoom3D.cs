using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class InstantRoom3D : MonoBehaviour
{
    [HideInInspector] public List<Room3DDoorClass> doorwayList;
    [HideInInspector] public List<Vector3> spawnPosition;
    bool[] doorwayOrientations = new bool[4];
    bool alreadyDoorwayBlock = false;

    private void Awake()
    {
        for (int i = 0; i < doorwayOrientations.Length; i++)
            doorwayOrientations[i] = false;
    }

    public void Initialise(List<Room3DDoorClass> doorwayList, List<Vector3> spawnPosition)
    {
        this.doorwayList = doorwayList;
        this.spawnPosition = spawnPosition;
    }

    public void DoorwayConnect(Orientation orientation)
    {
        if (orientation == Orientation.none)
            return;

        doorwayOrientations[(int)orientation] = true;
    }

    void BlockOffUnusedDoorways()
    {
        for(int i=0;i<doorwayOrientations.Length;i++)
        {
            Room3DDoorClass doorData = doorwayList.Find(x => x.orientation == (Orientation)i);
            if (doorData == null)
                return;

            if (doorwayOrientations[i])
            {
                SpawnDoorPrefab(doorData);
                continue;
            }

            BlockDoorway(doorData);
        }
        alreadyDoorwayBlock = true;
    }

    void SpawnDoorPrefab(Room3DDoorClass doorData)
    {
        GameObject doorPrefab = Instantiate(doorData.doorPrefab, Vector3.zero, Quaternion.identity, transform);
        doorPrefab.transform.localPosition = doorData.doorPosition;
        doorPrefab.transform.Rotate(-Vector3.up * 90f * (int)doorData.orientation);
        doorPrefab.GetComponent<DoorPrefab3D>().SpawnDoorPrefab(doorData.orientation);
    }

    void BlockDoorway(Room3DDoorClass doorData)
    {
        GameObject wallPrefab = Instantiate(doorData.WallPrefab, Vector3.zero, Quaternion.identity, transform);
        wallPrefab.transform.localPosition = doorData.doorPosition;
    }

    public void EnableRoom()
    {
        if (!alreadyDoorwayBlock)
            BlockOffUnusedDoorways();
        gameObject.SetActive(true);
    }

    public void DisableRoom() => gameObject.SetActive(false);

    public Vector3 GetSpawnPosition(Orientation orientation)
    {
        if (orientation == Orientation.none)
            return spawnPosition[Random.Range(0, spawnPosition.Count)];
        else
        {
            orientation = (Orientation)(((int)orientation + 2) % 4);

            Vector3 adjust = DungeonBuilder.Inst.OrientationToVector3(orientation);
            return doorwayList.Find(x => x.orientation == orientation).doorPosition - adjust;
        }
    }
}
