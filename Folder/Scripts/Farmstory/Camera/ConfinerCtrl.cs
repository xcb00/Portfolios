using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfinerCtrl : MonoBehaviour
{
    [SerializeField] SceneConfiner[] confiners;
    PolygonCollider2D collider = null;

    private void OnEnable()
    {
        //collider = GetComponent<PolygonCollider2D>();
        //EventHandler.AfterSceneLoadEvent += SetConfiner;
    }

    private void OnDisable()
    {
        //EventHandler.AfterSceneLoadEvent -= SetConfiner;
    }

    void SetConfiner()
    {
        int idx = -1;

        for(int i = 0; i < confiners.Length; i++)
        {
            if (confiners[i].scene == GameDatas.currentScene)
            {
                idx = i;
                break;
            }
        }

        if(idx<0)
        {
            Debug.LogError($"ConfinerCtrl Script Error : {EnumCaching.ToString(GameDatas.currentScene)}�� Confiner�� �����Ǿ����� �ʽ��ϴ�");
            return;
        }

        if (confiners[idx].points.Length != 4)
        {
            Debug.LogError($"ConfinerCtrl Script Error : {EnumCaching.ToString(GameDatas.currentScene)}�� Confiner Point�� �߸� �����Ǿ��ֽ��ϴ�");
            return;
        }

        collider.points = confiners[idx].points;

        /*for (int i = 0; i < 4; i++)
        {
            Debug.Log($"Before : {collider.points[i]} / {confiners[idx].points[i]}");
            collider.points[i] = confiners[idx].points[i];
            Debug.Log($"After : {collider.points[i]} / {confiners[idx].points[i]}");
        }*/
    }
}
