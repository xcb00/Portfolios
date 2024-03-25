using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControlManager : Singleton<SceneControlManager>
{
    [SerializeField] Transform poolManager;
    bool isFading = false;
    [SerializeField] CanvasGroup fadeUI = null;
    Coroutine next = null;

    /// <summary>
    /// ������ ó�� �����ϸ� �����ϴ� �Լ�
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        // Main ������ �̵��� �� ���� ���� �� �ð��� ����
        //EventHandler.CallPauseTimeEvent(true);
        GameDatas.pause = true;

        // GameDatas�� �ʱ�ȭ��Ŵ
        ResourceManager.Instance.InitGameDatas();

        // ���� ȭ���� Fade In
        StartGame();
    }

    public void StartGame()
    {
        StartCoroutine(StartingGame());
    }

    public void NextLevel(SceneChange sc)
    {
        if (GameDatas.stageLevel >= GameDatas.maxStage) sc = SceneChange.GameClear;

        EventHandler.CallPanelTextEvent(sc);
        switch (sc)
        {
            case SceneChange.GameClear:
            case SceneChange.GameOver:
                Fade(1f);
                break;
            case SceneChange.StageClear:
                if (next == null)
                    next = StartCoroutine(MovingNextLevel());
                break;
        }
    }

    public void Fade(float alpha) { StartCoroutine(Fading(alpha)); }

    IEnumerator Fading(float alpha)
    {
        isFading = true;


        float fadeSpeed = Mathf.Abs(fadeUI.alpha - alpha) / Settings.fadeTime;
        while(!Mathf.Approximately(fadeUI.alpha, alpha))
        {
            fadeUI.alpha = Mathf.MoveTowards(fadeUI.alpha, alpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        isFading = false;
        //EventHandler.CallPauseTimeEvent(false);
    }

    IEnumerator StartingGame()
    {
        while (true)
        {
            if (Input.anyKeyDown) break;
            yield return null;
        }

        yield return StartCoroutine(Fading(0f));
        GameDatas.pause = false;
    }

    IEnumerator MovingNextLevel()
    {
        GameDatas.pause = true;

        yield return StartCoroutine(Fading(1f));
        EventHandler.CallStopRespawnEvent(true);
        EventHandler.CallPoolClearevent(poolManager);
        EventHandler.CallStageClearEvent();

        while (true)
        {
            if (Input.anyKeyDown) break;
            yield return null;
        }

        yield return StartCoroutine(Fading(0f));
        GameDatas.stageLevel++;
        EventHandler.CallStageStartEvent();
        EventHandler.CallStopRespawnEvent(false);

        next = null;
    }

    /*public void ChangeScene(SceneName scene)
    {
        StartCoroutine(ChangingScene(scene));
    }

    IEnumerator ChangingScene(SceneName scene)
    {
        // Fade Out �� ������ �Լ�

        yield return StartCoroutine(Fading(1f));

        // Fade Out �� ������ �Լ�

        yield return SceneManager.UnloadSceneAsync(EnumCaching.ToString(SceneName.Lobby));

        yield return SceneManager.LoadSceneAsync(EnumCaching.ToString(scene), LoadSceneMode.Additive);

        // Fade In �� ������ �Լ�

        yield return StartCoroutine(Fading(0f));

        // Fade In �� ������ �Լ�
        EventHandler.CallPauseTimeEvent(false);
    }*/
}
