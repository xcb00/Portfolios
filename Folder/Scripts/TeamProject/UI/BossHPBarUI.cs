using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHPBarUI : MonoBehaviour
{
    [SerializeField] Image HPImg;
    float maxHP = 0f;
    private void OnEnable()
    {
        EventHandler.StageClearEvent += Inactive;
        EventHandler.BossRespawnEvent += Active;
        EventHandler.BossGetDamageEvent += GetDamage;
    }

    private void OnDisable()
    {
        EventHandler.StageClearEvent -= Inactive;
        EventHandler.BossRespawnEvent -= Active;
        EventHandler.BossGetDamageEvent -= GetDamage;
    }

    void Inactive()
    {
        maxHP = 0f;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    void Active(float bossHP)
    {
        maxHP = bossHP;
        HPImg.fillAmount = 1f;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    void GetDamage(float hp)
    {
        HPImg.fillAmount = hp / maxHP;
    }
}
