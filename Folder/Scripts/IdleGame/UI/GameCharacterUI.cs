using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCharacterUI : MonoBehaviour
{
    public Vector3 adjust = Vector3.zero;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Slider hpBar;
    string levelFormat = "Lv{0}";
    RectTransform rect = null;

    public void InitInfo(int maxHP, int level = -1)
    {
        if(rect==null)
            rect = GetComponent<RectTransform>();

        //LevelUp(level);
        if(level > 0)
            levelText.SetText(System.String.Format(levelFormat, level));
        hpBar.minValue = 0;
        hpBar.maxValue = maxHP;
        hpBar.value = maxHP;
    }

    public void OnDamage(int currentHP) => hpBar.value = currentHP;

    //public void LevelUp(int level) => levelText.SetText(System.String.Format(levelFormat, level));
    public void MoveCharacter(Vector3 position) => rect.position = Camera.main.WorldToScreenPoint(position) + adjust;

    public void ActiveObject(bool active) => gameObject.SetActive(active);
}