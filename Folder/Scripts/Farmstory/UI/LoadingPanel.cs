using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPanel : Panel
{
    [SerializeField] GameObject loadingImg;
    private void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        EventHandler.LoadingPanelEvent += Active;
    }
    private void OnDisable()
    {
        EventHandler.LoadingPanelEvent -= Active;
    }

    void Active(bool active)
    {
        loadingImg.SetActive(active);
        if (active)
            base.ActivePanel();
        else
            base.InactivePanel();
    }
}
