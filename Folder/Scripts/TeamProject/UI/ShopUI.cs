using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    float screenAdjust = 30f;
    [SerializeField] RectTransform itemInfo;
    [SerializeField] Image activeMark;
    [SerializeField] GameObject beforeBuy;
    [SerializeField] GameObject afterBuy;
    [SerializeField] Image[] notBoughts;
    [Space(12), Header("ItemInfo")]
    [SerializeField] Text curDmg;
    [SerializeField] Text curAsp;
    [SerializeField] Text itemDmg;
    [SerializeField] Text itemAsp;
    [SerializeField] Text enhanceDmg;
    [SerializeField] Text enhanceAsp;
    [SerializeField] Text Itemprice;

    int currentItem;
    int equipItem;
    int itemCnt = 4;


    bool[] activeItem;
    int[] enhanceLevel;
    Vector3 screenHight;

    private void Awake()
    {
        activeItem = new bool[itemCnt];
        enhanceLevel = new int[itemCnt];
        activeItem[0] = true;
        for (int i = 1; i < activeItem.Length; i++) activeItem[i] = false;
        for (int i = 1; i < enhanceLevel.Length; i++) enhanceLevel[i] = 0;
        equipItem = 0;
        screenHight = new Vector3(0f, Screen.height, 0f);
        //AfterBuy();
    }

    private void OnEnable()
    {
        AfterBuy();
        //activeMark.rectTransform.position = Screen.width * Camera.main.ScreenToViewportPoint(notBoughts[equipItem].rectTransform.position) + screenHight* screenAdjust;
        activeMark.rectTransform.localPosition = Vector3.right * (-270.0f + 180.0f * equipItem);
    }

    private void OnDisable()
    {
        InActiveItemInfo();
    }

    public void ActiveItemInfo(int index)
    {
        itemInfo.gameObject.SetActive(true);
        currentItem = index;
        itemInfo.localPosition = new Vector3(-270.0f + 180.0f * index, screenAdjust, 0.0f);
        //itemInfo.position = Screen.width * Camera.main.ScreenToViewportPoint(notBoughts[index].rectTransform.position) + screenHight * screenAdjust;

        WriteItemInfo();

        //beforeBuy.SetActive(!activeItem[index]);
        //afterBuy.SetActive(activeItem[index]);

        if(!activeItem[index])
        {
            beforeBuy.SetActive(true);
            afterBuy.SetActive(false);
            Itemprice.text = GameDatas.itemList[currentItem].price.ToString();
        }
        else
        {
            beforeBuy.SetActive(false);
            afterBuy.SetActive(true);
        }
    }

    void WriteItemInfo()
    {
        curDmg.text = RoundFloat(GameDatas.itemList[equipItem].damage).ToString("F2");
        curAsp.text = RoundFloat(GameDatas.itemList[equipItem].attackSpeed).ToString("F2");
        itemDmg.text = RoundFloat(GameDatas.itemList[currentItem].damage).ToString("F2");
        itemAsp.text = RoundFloat(GameDatas.itemList[currentItem].attackSpeed).ToString("F2");/*
        enhanceDmg.text = RoundFloat(GameDatas.itemList[currentItem].damageEnhance).ToString("F2");
        enhanceAsp.text = (-RoundFloat(GameDatas.itemList[currentItem].attackSpeedEnhance)).ToString("F2");*/
    }

    float RoundFloat(float f) => Mathf.Round(f * 10f) * 0.1f;

    void InActiveItemInfo()
    {
        itemInfo.gameObject.SetActive(false);
    }

    void AfterBuy()
    {
        for (int i = 0; i < activeItem.Length; i++)
            notBoughts[i].gameObject.SetActive(!activeItem[i]);
    }

    public void BuyButton()
    {
        Debug.Log("Buy Item");
        int p = GameDatas.itemList[currentItem].price;
        if (ResourceManager.Instance.Gold >= p)
        {
            ResourceManager.Instance.AddGold(-p);
            activeItem[currentItem] = true;
            AfterBuy();
        }
        else
            EventHandler.CallPrintSystemMassageEvent("Gold가 부족합니다");
        InActiveItemInfo();
    }

    public void EquipButton()
    {
        Debug.Log("Equip Item");
        EventHandler.CallPlayerEquipmentEvent(currentItem);
        equipItem = currentItem;
        activeMark.rectTransform.localPosition = Vector3.right * (-270.0f + 180.0f * equipItem);
        //activeMark.rectTransform.position = Screen.width * Camera.main.ScreenToViewportPoint(notBoughts[equipItem].rectTransform.position) + screenHight * screenAdjust;
        InActiveItemInfo();
    }

    public void EnhanceButton()
    {
        Debug.Log("Enhance Item");
        if (enhanceLevel[currentItem] >= GameDatas.enhanceProbability.Length)
        {
            EventHandler.CallPrintSystemMassageEvent("강화할 수 없습니다");
            return;
        }
        else
        {
            if (ResourceManager.Instance.Gold >= 100)
            {
                ResourceManager.Instance.AddGold(-100);
                if (Enhance(enhanceLevel[currentItem]))
                {
                    enhanceLevel[currentItem]++;
                    GameDatas.itemList[currentItem].damage += GameDatas.itemList[currentItem].damageEnhance;
                    GameDatas.itemList[currentItem].attackSpeed -= GameDatas.itemList[currentItem].attackSpeedEnhance;
                    
                    WriteItemInfo();

                    EventHandler.CallPrintSystemMassageEvent("강화 성공");
                }
                else
                    EventHandler.CallPrintSystemMassageEvent("강화 실패");
            }
            else
            {
                EventHandler.CallPrintSystemMassageEvent("Gold가 부족합니다");
                return;
            }
        }
    }

    bool Enhance(int level)
    {
        
        if (Random.Range(0, 100) < GameDatas.enhanceProbability[level])
            return true;
        else return false;
    }
}
