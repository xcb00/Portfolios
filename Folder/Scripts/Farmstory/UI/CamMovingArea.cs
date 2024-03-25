using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovingArea : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] float hori;
    [SerializeField, Range(0f, 1f)] float verti;
    [SerializeField] RectTransform camArea;
    Vector2 area = Vector2.zero;
    Vector2 origin = Vector2.zero;
    Vector2 imgSize = Vector2.zero;
    private void Start()
    {
        ShowArea();
    }
    // 세팅창에서 카메라가 Show를 할 때
    void ShowArea()
    {
        //origin = Settings.Instance.camMovingAreaScale * 2f;
        transform.localScale = origin;
        GetComponent<SpriteRenderer>().enabled = true;
        imgSize = camArea.sizeDelta;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //origin = Settings.Instance.camMovingAreaScale * 2f;
            area.x = origin.x * verti;
            area.y = origin.y * hori;
            //Debug.Log(area);
            transform.localScale = area;
            //camArea.sizeDelta = imgSize * new Vector2(verti, hori);
            //camArea.localScale = new Vector2(verti, hori);
        }
    }

    void InActive()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
