using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] GameObject[] upgradeUnitPanel;
    [SerializeField] float workerSpeedIncrement = 0.1f;
    [SerializeField] float farmerDamageIncrement = 0.4f;
    [SerializeField] float minerDamageIncrement = 5f;
    [SerializeField] float warriorDamageIncrement = 1.5f;
    [SerializeField] float warriorHPIncrement = 3f;
    [SerializeField] float magicianDamageIncrement = 2f;
    [SerializeField] float magicianDelayDecrease = 0.1f;

    int[] upgradeLevels;// = new int[8];
    [SerializeField]Text[] upgradePrices;
    private void Awake()
    {
        UnitSO unitSO = Resources.Load<UnitSO>("UnitDetailList");
        GameDatas.playerUnitDetailList = new List<PlayerUnitDetails>();
        GameDatas.comUnitDetailList = new List<ComUnitDetails>();
        GameDatas.bossSkillDetailsList = new List<BossSkillDetails>();

        upgradeLevels = new int[8];
        for (int i = 0; i < 8; i++) upgradeLevels[i] = 0;

        foreach (PlayerUnitDetails details in unitSO.playerUnitDetailList)
            GameDatas.playerUnitDetailList.Add(details.DeepCopy());

        foreach (ComUnitDetails details in unitSO.comUnitDetailList)
            GameDatas.comUnitDetailList.Add(details.DeepCopy());

        foreach (BossSkillDetails details in unitSO.bossSkillDetailsList)
            GameDatas.bossSkillDetailsList.Add(details.DeepCopy());
    }

    public void UpgradePanelActive(int index)
    {
        upgradePrices[index * 2].text = (100 * Mathf.Pow(2, upgradeLevels[index * 2])).ToString();
        upgradePrices[index * 2 + 1].text = (100 * Mathf.Pow(2, upgradeLevels[index * 2 + 1])).ToString();

        for (int i = 0; i < upgradeUnitPanel.Length; i++)
            upgradeUnitPanel[i].SetActive((index == i));
    }

    public void UpgradeUnit(int index)
    {
        for (int i = 0; i < upgradeUnitPanel.Length; i++)
            upgradeUnitPanel[i].SetActive(false);
        switch (index)
        {
            case 0:
                FarmerSpeed();
                break;
            case 1:
                FarmerDamage();
                break;
            case 2:
                MinerSpeed();
                break;
            case 3:
                MinerDamage();
                break;
            case 4:
                WarriorDamage();
                break;
            case 5:
                WarriorHP();
                break;
            case 6:
                MagicianDamage();
                break;
            case 7:
                MagicianDelay();
                break;
            default: break;
        }

        for (int i = 0; i < upgradeUnitPanel.Length; i++)
            upgradeUnitPanel[i].SetActive(false);
    }

    bool CanUpgrade(int index)
    {
        if (ResourceManager.Instance.Gold < 100 * Mathf.Pow(2, upgradeLevels[index]))
        {
            EventHandler.CallPrintSystemMassageEvent("Gold가 부족합니다");
            return false;
        }
        else if(upgradeLevels[index]>=5)
        {
            EventHandler.CallPrintSystemMassageEvent("최대 레벨입니다");
            return false;
        }
        else
        {
            ResourceManager.Instance.AddGold(Mathf.RoundToInt(-100 * Mathf.Pow(2, upgradeLevels[index]++)));
            return true;
        }
    }

    void FarmerSpeed()
    {
        if(CanUpgrade(0))
            GameDatas.playerUnitDetailList[(int)PlayerUnitType.farmer].speed += workerSpeedIncrement;
    }

    void FarmerDamage()
    {
        if (CanUpgrade(1))
        {
            GameDatas.playerUnitDetailList[(int)PlayerUnitType.farmer].damage += farmerDamageIncrement;
            ResourceManager.Instance.CheckMaxPopulation();
        }
    }

    void MinerSpeed()
    {
        if(CanUpgrade(2))
            GameDatas.playerUnitDetailList[(int)PlayerUnitType.miner].speed += workerSpeedIncrement;
    }

    void MinerDamage()
    {
        if (CanUpgrade(3))
            GameDatas.playerUnitDetailList[(int)PlayerUnitType.miner].damage += minerDamageIncrement;
    }

    void WarriorDamage()
    {
        if (CanUpgrade(4))
            GameDatas.playerUnitDetailList[(int)PlayerUnitType.warrior].damage += warriorDamageIncrement;
    }

    void WarriorHP()
    {
        if (CanUpgrade(5))
            GameDatas.playerUnitDetailList[(int)PlayerUnitType.warrior].hp += warriorHPIncrement;
    }

    void MagicianDamage()
    {
        if(CanUpgrade(6))
            GameDatas.playerUnitDetailList[(int)PlayerUnitType.magician].damage += magicianDamageIncrement;
    }

    void MagicianDelay()
    {
        if(CanUpgrade(7))
            GameDatas.playerUnitDetailList[(int)PlayerUnitType.magician].attackSpeed += magicianDelayDecrease;
    }
}
