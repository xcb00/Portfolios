using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BankPopup : Panel
{
    [SerializeField] TextMeshProUGUI moneyTxt = null;
    [SerializeField] TextMeshProUGUI titleTxt = null;
    bool isEarn = false;
    int num = 0;

    void Start()
    {
        Init();
    }

    public override void ActivePanel()
    {
        num = 0;
        moneyTxt.SetText("0");
        base.ActivePanel();
    }

    public void KeyFunc(int n)
    {
        if (n == -2) num = 0;
        else if (n == -1)
        {
            if (num < 10) num = 0;
            else num /= 10;
        }
        else if(num<100000000)
        {
            if (num < 1 && n == 0) num = 0;
            else num = num * 10 + n;
        }

        moneyTxt.SetText(num.ToString("N0"));
    }

    public void OpenPopup(bool earn)
    {
        isEarn = earn;
        titleTxt.SetText(earn ? "대출" : "상환");
        ActivePanel();
    }

    public void Confirm()
    {        
        int money = num * (isEarn ? 1 : -1);
        //Utility.GoldChange(ref money);

        if (money > 0)
            Debug.Log("Gold 최대치 초과 : " + money);

        // Loan Data 처리
        // loan.x -= money / loan.x += money
        // loan.y -= money
        

        InactivePanel();
        DataManager.Instance.SaveTimeData();
    }
}
