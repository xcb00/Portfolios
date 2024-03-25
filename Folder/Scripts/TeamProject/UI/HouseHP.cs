using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseHP : MonoBehaviour, IDamage
{
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] Image hpImg;
    [SerializeField] float decraseTime = 0.5f;
    [SerializeField] float maxHP = 100f;
    float hp;
    float targetHP;
    Coroutine decrease;
    private void Start()
    {
        InitHP();
    }
    private void OnEnable()
    {
        EventHandler.StageClearEvent += StageClear;
    }
    private void OnDisable()
    {
        EventHandler.StageClearEvent -= StageClear;
    }

    void StageClear()
    {
        InitHP();
    }

    void InitHP()
    {
        hp = maxHP;
        targetHP = maxHP;
        hpImg.fillAmount = hp / maxHP;
    }

    public void GetDamage(float dmg, int possibility = 0)
    {
        targetHP -= dmg;
        if (decrease != null) StopCoroutine(decrease);
        StartCoroutine(DecreasingHP());
    }
    IEnumerator DecreasingHP()
    {
        float t = 0.0f;
        float from = hp;
        while (t<decraseTime)
        {
            t += Time.deltaTime;
            if (t > decraseTime) t = decraseTime;
            hp = Mathf.Lerp(from, targetHP, t / decraseTime);
            hpImg.fillAmount = hp / maxHP;
            yield return null;
        }
        if(hp<=0.0f)
        {
            SceneControlManager.Instance.NextLevel(SceneChange.GameOver);
            GameDatas.pause = true;
        }
    }
}
