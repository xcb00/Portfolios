using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CineControl : MonoBehaviour
{
    [SerializeField] Transform player;
    //[SerializeField] Transform lobby;
    [SerializeField] SceneConfiner[] confiners;
    [SerializeField] CinemachineConfiner2D confiner;
    [SerializeField] PolygonCollider2D[] boundary;
    [SerializeField] CinemachineVirtualCamera vCam;
    bool _switch = true;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadBeforeFadeInEvent += SetBoundingShape;
        EventHandler.LoadLobby += SetBoundingShape;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadBeforeFadeInEvent -= SetBoundingShape;
        EventHandler.LoadLobby -= SetBoundingShape;
    }

    void SetBoundingShape()
    {
        if(vCam.Follow!=player && GameDatas.currentScene!=SceneName.Lobby)
            vCam.Follow = player;

        // PolygonCollider의 points 좌표를 변경해도 Cinemachine의 confiner 영역은 변하지 않음
        // Bounding Shape 2D가 변경되면 Cinemachine의 confiner 영역이 변경됨
        // >> PolygonCollider를 2개를 교차해 Bounding Shape 2D를 변경

        _switch = !_switch;
        int idx = _switch ? 0 : 1;

        // 바꿀 PolygonCollider의 Points 변경
        SetConfiner(idx);

        // 씬의 크기에 맞게 변경된 PolygonCollider로 Bounding Shape 2D를 변경
        confiner.m_BoundingShape2D = boundary[idx];
    }

    void SetConfiner(int _switch)
    {
        int idx = -1;

        for (int i = 0; i < confiners.Length; i++)
        {
            if (confiners[i].scene == GameDatas.currentScene)
            {
                idx = i;
                break;
            }
        }

        if (idx < 0)
        {
            Debug.LogError($"CineControl Script Error : {EnumCaching.ToString(GameDatas.currentScene)}의 Confiner가 설정되어있지 않습니다");
            return;
        }

        if (confiners[idx].points.Length != 4)
        {
            Debug.LogError($"CineControl Script Error : {EnumCaching.ToString(GameDatas.currentScene)}의 Confiner Point가 잘못 설정되어있습니다");
            return;
        }

        boundary[_switch].points = confiners[idx].points;
    }

}
