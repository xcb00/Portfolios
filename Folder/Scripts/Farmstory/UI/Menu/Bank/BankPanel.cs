using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BankPanel : MenuPanel
{
    [SerializeField] TextMeshProUGUI moneyText = null;
    [SerializeField] TextMeshProUGUI loanTxt = null;
    [SerializeField] TextMeshProUGUI repaymentTxt = null;
    int num = 0;

    void UpdateLoanData()
    {
        num = 0;
        moneyText.SetText(num.ToString("N0"));
        loanTxt.SetText(GameDatas.loan.x.ToString("N0"));
        repaymentTxt.SetText(GameDatas.loan.y.ToString("N0"));
    }

    public override void ActivePanel()
    {
        UpdateLoanData();
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
        else if (num < 100000000)
        {
            if (num < 1 && n == 0) num = 0;
            else num = num * 10 + n;
        }

        moneyText.SetText(num.ToString("N0"));
    }

    public void OnLoan()
    {
        // 대출 한도 체크
        // int가 나타낼 수 있는 최대값이 제한되어 있기 때문에 제한
        if (GameDatas.loan.x + num > 1000000000)
            EventHandler.CallPrintSystemMassageEvent("대출 한도를 초과합니다");
        else if (GameDatas.HourMinuteGold.z + num > 1000000000)
            EventHandler.CallPrintSystemMassageEvent("보유 골드 한도를 초과합니다");
        else
        {
            GameDatas.loan.x += num;
            Utility.GoldChange(num);
            DataManager.Instance.SaveTimeData();
        }
        UpdateLoanData();
    }

    public void OnRepayment()
    {
        if (GameDatas.HourMinuteGold.z < num)
            EventHandler.CallPrintSystemMassageEvent("골드가 부족합니다");
        else
        {
            if (GameDatas.loan.x - num <= 0)
                EventHandler.CallGameClearEvent();

            GameDatas.loan.x -= num;
            GameDatas.loan.y = GameDatas.loan.y - num < 0 ? 0 : GameDatas.loan.y - num;
            Utility.GoldChange(-num);
            DataManager.Instance.SaveTimeData();
        }

        /*if (GameDatas.yearChange)
        {
            if (GameDatas.loan.y > 0) 
            {
                Debug.Log("Game Over");
                EventHandler.CallGameOverEvent();
            }
            else 
                GameDatas.loan.y = Mathf.RoundToInt(GameDatas.loan.x * Settings.Instance.loanInterest * 0.01f);
            GameDatas.yearChange = false;
        }*/

        UpdateLoanData();
    }
}
