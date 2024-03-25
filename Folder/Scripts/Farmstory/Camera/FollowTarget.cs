using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] SceneName[] notMovingCam;
    public float spd = 1.0f;
    [SerializeField] Transform target;
    Vector2 size = Vector2.zero;
    Vector3 dir = Vector3.zero;

    bool movingCam = true;
    float x = 0.0f;
    float y = 0.0f;

    public Vector2 MovingArea => size;
    Vector2 camMovingArea = new Vector2(12f, 5f);

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += CheckMovingCam;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= CheckMovingCam;
    }

    void CheckMovingCam()
    {
        movingCam = true;
        for (int i = 0; i < notMovingCam.Length; i++)
        {
            if (notMovingCam[i] == GameDatas.currentScene)
            {
                movingCam = false;
                break;
            }            
        }
    }

    void UpdateCamArea()
    {
        size = camMovingArea;
    }

    public void Init()
    {
        StopAllCoroutines();
        UpdateCamArea();
        x = 0.0f;
        y = 0.0f;
        // -18
        if (Mathf.Abs(target.position.x) > size.x)
            x = (Mathf.Abs(target.position.x) - size.x) * (target.position.x > 0f ? 1 : -1);

        if (Mathf.Abs(target.position.y) > size.y)
            y = (Mathf.Abs(target.position.y) - size.y) * (target.position.y > 0f ? 1 : -1);

        transform.position = new Vector3(x, y, 0f);
        //Debug.Log(transform.position);

        StartCoroutine(Following()); 
    }

    IEnumerator Following() // update문과 동일
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) UpdateCamArea();
            if (movingCam)
            {
                x = 0.0f;
                y = 0.0f;
                if (Mathf.Abs(target.position.x - transform.position.x) > size.x)
                    x = target.position.x - transform.position.x > 0f ? 1 : -1;

                if (Mathf.Abs(target.position.y - transform.position.y) > size.y)
                    y = target.position.y - transform.position.y > 0f ? 1 : -1;

                dir.x = x;
                dir.y = y;

                transform.Translate(dir * Time.deltaTime * spd);
            }

            yield return null;
        }
    }
}
