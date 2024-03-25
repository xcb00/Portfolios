using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPanel : Panel
{
    public bool activeSelf { get; private set; }
    void Start()
    {
        activeSelf = false;
        Init();
    }

    public void ActiveExit(bool active)
    {
        activeSelf = active;
        if(active)
            base.ActivePanel();
        else
            base.InactivePanel();
    }

    public override void ActivePanel()
    {
        activeSelf = true;
        base.ActivePanel();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public override void InactivePanel()
    {
        activeSelf = false;
        base.InactivePanel();
    }
}
