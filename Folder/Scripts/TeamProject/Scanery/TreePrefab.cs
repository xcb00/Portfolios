using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreePrefab : MonoBehaviour
{
    Animator animator;
    SpriteRenderer destoryMark;
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        destoryMark = GetComponentInChildren<SpriteRenderer>();
    }

    public void DestoryMarkInteraction()
    {
        destoryMark.enabled = !destoryMark.enabled;
        //destoryMark.gameObject.SetActive(!destoryMark.gameObject.activeSelf);
    }

    public void FallDownTreeEvent()
    {
        Debug.Log("Disable");
    }

    public void HitTree()
    {

    }
}
