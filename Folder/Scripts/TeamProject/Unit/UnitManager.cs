using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitManager : MonoBehaviour
{
    [SerializeField] Transform crystalVein = null;
    [SerializeField] Transform banner = null;
    [SerializeField] Cooldown[] coolDown = null;
    [SerializeField] Text[] prices = null;
    Vector3 spawnPosition;

    private void Start()
    {
        spawnPosition = transform.GetChild(0).position;
        for (int i = 0; i < coolDown.Length; i++)
            prices[i].text = $"${GameDatas.playerUnitDetailList[i].price.ToString()}";
    }

    public void CreateUnit(int index)
    {
        //int price = GameDatas.playerUnitDetailList.Find(x => x.type == (PlayerUnitType)index).price;
        int price = GameDatas.playerUnitDetailList[index].price;
        if (!coolDown[index].CanUse)
            EventHandler.CallPrintSystemMassageEvent("���� ������ ������ �� �����ϴ�");
        else if (price > ResourceManager.Instance.Gold)
            EventHandler.CallPrintSystemMassageEvent("Gold�� �����մϴ�");
        else if (ResourceManager.Instance.CurrentPopulation >= ResourceManager.Instance.MaxPopulation)
            EventHandler.CallPrintSystemMassageEvent("�α����� �����մϴ�");
        else
        {
            ResourceManager.Instance.AddGold(-price);
            GameObject obj = PoolManager.Instance.DeququeObject((PoolType)(index + (int)ComUnitType.count));
            coolDown[index].CoolDown(GameDatas.playerUnitDetailList[index].respownTime);

            if ((PlayerUnitType)index == PlayerUnitType.farmer) ResourceManager.Instance.AddMaxPopulation(true);

            if (index == 1) obj.GetComponent<MinerUnit>().SetTarget(crystalVein);
            else if (index > 1) obj.GetComponent<AttackUnit>().SetTarget(banner);

            obj.GetComponent<PlayerUnit>().SpawnUnit(spawnPosition, (PoolType)(index + (int)ComUnitType.count));


        }
    }
}