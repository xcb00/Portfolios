using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPoint : MonoBehaviour
{
    [SerializeField] SceneName currentScene;
    [SerializeField] InteractionType interactionType;

    Collider2D collider = null;

    private void OnEnable()
    {
        collider = GetComponent<Collider2D>();
        EventHandler.AfterSceneLoadBeforeFadeInEvent += SetActive;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadBeforeFadeInEvent -= SetActive;
    }

    void SetActive()
    {
        collider.enabled = currentScene == GameDatas.currentScene;
    }

    public void isActive()
    {
        if (currentScene == GameDatas.currentScene)
            EventHandler.CallSetInteractionTypeEvent(interactionType, currentScene);
    }
}
