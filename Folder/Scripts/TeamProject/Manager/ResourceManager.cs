using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ResourceManager : Singleton<ResourceManager>
{
    [SerializeField] Transform _minimap;
    public Transform MinimapTrans { get => _minimap; }
    [SerializeField] int defaultPopulation = 6;
    int gold = 0;
    int currentPopulation = 0;
    int maxPopulation = 0;
    int farmerNum = 0;// { get; set; }
    int soilNum = 0;
    StringBuilder sb = new StringBuilder();
    
    [SerializeField] Text goldTxt;
    [SerializeField] Text populationTxt;
    [SerializeField] ItemSO itemSO;


    public int Gold { get { return gold; } }
    public int CurrentPopulation { get { return currentPopulation; }}
    public int MaxPopulation { get { return maxPopulation; }}
    public void AddGold(int g) { gold += g; goldTxt.text = gold.ToString("N0"); }
    public void AddCurrentPopulation(bool addPopulation){ currentPopulation += (addPopulation?1:-1); UpdatePopulation(); }
    public void AddMaxPopulation(int p) { maxPopulation += p; UpdatePopulation(); }
    public void CheckMaxPopulation()
    {
        int farmerCover = Mathf.FloorToInt(farmerNum * GameDatas.playerUnitDetailList[(int)PlayerUnitType.farmer].damage);
        int soilCover = soilNum * 2;
        if (farmerCover < soilCover) maxPopulation = farmerCover + defaultPopulation;
        else maxPopulation = soilCover + defaultPopulation;
        UpdatePopulation();
    }

    public void AddMaxPopulation(bool isFarmer) 
    {
        if (isFarmer) farmerNum++;
        else soilNum++;
        CheckMaxPopulation();
    }

    public void MinusMaxPopulation() { farmerNum--; CheckMaxPopulation(); }

    public void InitGameDatas(int addPopulation = 0)
    {
        //AddGold(300);

        AddGold(500);
        soilNum = 0;
        farmerNum = 0;
        currentPopulation = 0;
        defaultPopulation += addPopulation;
        CheckMaxPopulation();
        /*if (addPopulation == 0) { AddMaxPopulation(defaultPopulation); }
        else { defaultPopulation += addPopulation; }*/

        GameDatas.itemList = new List<Item>();
        foreach (Item item in itemSO.itemList)
            GameDatas.itemList.Add(item.ItemCopy());
        GameDatas.enhanceProbability = itemSO.enhanceProbability;
    }

    void UpdatePopulation()
    {
        sb.Clear();
        sb.Append(currentPopulation.ToString());
        sb.Append("/");
        sb.Append(maxPopulation.ToString());
        populationTxt.text = sb.ToString();
    }
}
