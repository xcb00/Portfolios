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

    // �ش� ��ũ��Ʈ�� ������ �ִ� ���� ������Ʈ�� ��Ȱ��ȭ �� ��� �ڷ�ƾ�� ������
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
