using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banner : MonoBehaviour
{
    public Transform origin;
    private void OnEnable()
    {
        EventHandler.StageClearEvent += StageClear;
    }
    private void OnDisable()
    {
        EventHandler.StageClearEvent -= StageClear;
    }

    void StageClear()
    {
        transform.position = origin.position;
    }
}
