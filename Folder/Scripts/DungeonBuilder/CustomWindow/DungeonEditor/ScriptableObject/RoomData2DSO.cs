using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomData2D_", menuName = "Scriptable Objects/Room Data/Dungeon Room Data 2D")]
public class RoomData2DSO : RoomDataSO
{
    public List<Room2DDoorClass> doorways;
    public List<Vector2Int> spawnPosition;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();


        if (doorways == null || doorways.Count < 4)
            Debug.LogWarning($"{this.name}'s doorways.Length is {doorways.Count}");
        if (spawnPosition == null || spawnPosition.Count < 1)
            Debug.LogWarning($"{this.name}'s spawPosition is null or empty");
    }
#endif
}
