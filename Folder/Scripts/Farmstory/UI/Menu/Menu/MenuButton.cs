using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    RectTransform button;
    MenuGroup menu;
    Vector2 selectBtn = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        button = transform.GetChild(0).GetComponent<RectTransform>();
        menu = transform.GetComponentInParent<MenuGroup>();
        selectBtn = new Vector2(0f, 30f);
    }

    public void MenuSelect(bool select)
    {
        if (select) button.offsetMax = selectBtn;
        else button.offsetMax = Vector2.zero;
    }
}
