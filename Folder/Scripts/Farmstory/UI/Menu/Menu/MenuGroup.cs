using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGroup : Panel
{
    [SerializeField] MenuBinding[] menu;

    int index;

    private void Start()
    {
        index = -1;
        Init();
    }

    private void OnEnable()
    {
        EventHandler.OpenMenuEvent += OpenMenu;
    }

    private void OnDisable()
    {
        EventHandler.OpenMenuEvent -= OpenMenu;
    }

    void OpenMenu(MenuIndex idx)
    {
        switch (idx)
        {
            case MenuIndex.bank:
                {
                    if (GameDatas.loan.y > 0)
                    {
                        MenuChange(2);
                        base.ActivePanel();
                    }
                    else
                    {
                        GameDatas.yearChange = false;
                        GameDatas.loan.y = Mathf.RoundToInt(GameDatas.loan.x * Settings.Instance.loanInterest * 0.01f);
                    }
                }
                break;
            case MenuIndex.inventory:
                MenuChange(0);
                base.ActivePanel();
                break;
        }
    }

    public override void ActivePanel()
    {
        MenuChange(0);
        base.ActivePanel();
    }

    public override void InactivePanel()
    {

        if (GameDatas.yearChange)
        {
            if (GameDatas.loan.y > 0)
            {
                Debug.Log("Game Over");
                EventHandler.CallGameOverEvent();
            }
            else
            {
                GameDatas.yearChange = false;
                GameDatas.loan.y = Mathf.RoundToInt(GameDatas.loan.x * Settings.Instance.loanInterest * 0.01f);
            }
        }

        base.InactivePanel();
        InactiveMenu(index);
        index = -1;
    }

    public void MenuChange(int i)
    {
        if (i == index) return;

        if (i > index) { ActiveMenu(i); InactiveMenu(index); }
        else { InactiveMenu(index); ActiveMenu(i); }

        index = i;
    }

    void ActiveMenu(int idx) 
    {
        if (idx < menu.Length)
        {
            menu[idx].panel.ActivePanel();
            menu[idx].button.MenuSelect(true);
        }
    }

    void InactiveMenu(int idx)
    {
        if (idx >= 0)
        {
            menu[idx].panel.InactivePanel();
            menu[idx].button.MenuSelect(false);
        }
    }
}
