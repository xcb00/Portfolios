using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAnimation : MonoBehaviour
{
    Animator animator = null;
    RectTransform rect = null;

    bool open = false;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        EventHandler.OpenFishPanelEvent += OpenFishPanel;
        //OpenFishPanel(true);
    }
    private void OnDisable()
    {
        EventHandler.OpenFishPanelEvent -= OpenFishPanel;
    }

    void OpenFishPanel(bool open, FishType type)
    {
        if (open)
        {
            int ran = Random.Range(0, 4);
            int degree = (ran % 4 + ran / 4) * 90;
            degree = degree >= 360 ? degree - 360 : degree;
            int xScale = ran % 2 == 0 ? 1 : -1;
            int yScale = ran < 8 ? 1 : -1;

            rect.anchoredPosition = new Vector2(Mathf.Cos(ran * 22.5f), Mathf.Sin(ran * 22.5f)) * Random.Range(1, 28) * 3f;

            ran = Random.Range(12, 24);

            rect.Rotate(Vector3.forward * degree);
            rect.localScale = new Vector3(xScale, yScale, 1f);
            animator.speed = ran * 0.05f;
            animator.SetBool("moveCircle", ran > 20);
            animator.SetTrigger("active");
        }
        else
        {
            StartCoroutine(Utility.DelayCall(0.5f, () => InactiveAnimation()));
        }
    }

    void InactiveAnimation()
    {
        rect.anchoredPosition = Vector3.zero;
        rect.rotation = Quaternion.identity;
        animator.SetTrigger("inactive");
    }
}
