using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCheckPoint : MonoBehaviour
{
    [SerializeField] int cpIdx;
    [SerializeField] CharacterDirection[] nextDirs;

    private void OnEnable()
    {
        EventHandler.LoadLobby += LoadLobby;
        EventHandler.AfterSceneLoadEvent += LoadScene;
    }

    void LoadScene()
    {
        EventHandler.LoadLobby -= LoadLobby;
        EventHandler.AfterSceneLoadEvent -= LoadScene;
        Destroy(gameObject);
    }

    void LoadLobby()
    {
        transform.GetComponent<CircleCollider2D>().isTrigger = true;
    }

    public CharacterDirection GetNextDirection(out int cpIdx)
    {
        if (nextDirs.Length < 1)
        {
            cpIdx = -1;
            Debug.LogError($"{transform.name}'s LobbyCheckPoint.cs Error : nextDirs is null");
            return CharacterDirection.zero;
        }
        else
        {
            cpIdx = this.cpIdx;
            return nextDirs[Random.Range(0, nextDirs.Length)];
        }
    }
}
