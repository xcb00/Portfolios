using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomDataSO : ScriptableObject
{
    [HideInInspector] public string guid;
    public GameObject roomPrefab;
    public RoomType roomType;

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (string.IsNullOrEmpty(guid))
        {
            guid = GUID.Generate().ToString();
            EditorUtility.SetDirty(this);
        }
    }
#endif
}
