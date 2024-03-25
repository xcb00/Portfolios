using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooldown : MonoBehaviour
{
    Image img = null;
    private void OnEnable()
    {
        img = GetComponent<Image>();
    }

    public void UseSkill() { img.fillAmount = 0.0f; }

    public void CoolDown(float time)
    {
        StartCoroutine(CoolingDown(time));
    }

    // 해당 스크립트를 가지고 있는 게임 오브젝트가 비활성화 될 경우 코루틴은 정지됨
    protected virtual IEnumerator CoolingDown(float time)
    {
        float t = 0.0f;
        while (t<time)
        {
            t += Time.deltaTime;
            img.fillAmount = t / time;
            yield return null;
        }
        img.fillAmount = 1f;
    }

    public bool CanUse { get { return Mathf.Approximately(img.fillAmount, 1f); } }
}
