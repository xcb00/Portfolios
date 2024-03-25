using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyTarget : MonoBehaviour
{
    float movingSpd = 0.3f;
    Coroutine moving = null;
    WaitForSeconds wfs = new WaitForSeconds(1.0f);
    bool[] visitCP = new bool[9];
    Vector3 targetPos = Vector3.zero;
    CharacterDirection currentDir = CharacterDirection.zero;
    CharacterDirection nextDir = CharacterDirection.zero;
    Vector2 cpEdge = new Vector2(8.5f, 5.5f);

    private void OnEnable()
    {
        EventHandler.LoadLobby += LoadLobby;
        EventHandler.AfterSceneLoadEvent += LoadScene;
    }

    void SetTargetPosition()
    {
        Vector3 vec = Utility.CharacterDirectionToVector3(currentDir);
        targetPos.x += vec.x * cpEdge.x;
        targetPos.y += vec.y * cpEdge.y;
    }

    void LoadLobby()
    {
        for (int i = 0; i < 9; i++)
            visitCP[i] = false;
        currentDir = (CharacterDirection)Random.Range(0, 4);

        StartMoving();
    }

    void LoadScene()
    {
        if (moving != null)
        {
            StopCoroutine(moving);
            moving = null;
        }

        EventHandler.LoadLobby -= LoadLobby;
        EventHandler.AfterSceneLoadEvent -= LoadScene;

        Destroy(gameObject);
    }

    IEnumerator Moving()
    {
        while (GameDatas.currentScene==SceneName.Lobby)
        {
            float sqrDist = (transform.position - targetPos).sqrMagnitude;
            if (sqrDist < 0.001f)
            {
                transform.position = targetPos;
                currentDir = nextDir;
                SetTargetPosition();
                nextDir = CharacterDirection.zero;
            }

            transform.Translate(Utility.CharacterDirectionToVector3(currentDir) * Time.deltaTime * movingSpd);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << (int)LayerName.LobbyCheckPoint) & (1 << collision.gameObject.layer)) != 0)
        {
            int idx;
            nextDir = collision.GetComponent<LobbyCheckPoint>().GetNextDirection(out idx);
            StartMoving();
            if (idx < 0)
                return;
            else
                visitCP[idx] = true;

            foreach (bool b in visitCP)
                if (!b)
                    return;

            Debug.LogWarning("업적달성 : 모든 체크포인트 방문");
        }
    }

    void StartMoving()
    {
        if (moving != null)
        {
            StopCoroutine(moving);
            moving = null;
        }

        moving = StartCoroutine(Moving());
    }
}
