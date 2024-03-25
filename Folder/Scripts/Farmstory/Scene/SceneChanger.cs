using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] SceneName currentScene;
    public PlayerData data;// { get; private set; }
    Collider2D collider = null;
    //public bool isActive() => currentScene == GameDatas.currentScene;

    private void OnEnable()
    {
        collider = GetComponent<Collider2D>();
        EventHandler.AfterSceneLoadBeforeFadeInEvent += ActiveChanger;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadBeforeFadeInEvent -= ActiveChanger;
    }

    void ActiveChanger() { collider.enabled=currentScene == GameDatas.currentScene; }

}
