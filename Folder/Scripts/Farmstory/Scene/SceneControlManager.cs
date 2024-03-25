using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControlManager : Singleton<SceneControlManager>
{
    bool isFading = false;
    [SerializeField] CanvasGroup fadeUI = null;
    [SerializeField] CanvasGroup systemUI = null;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Fade(float alpha) { StartCoroutine(Fading(alpha, true)); }

    public void ChangeScene()
    {
        ChangeScene(GameDatas.playerData[0]);
    }

    public void ChangeScene(PlayerData playerData)
    {
        if (!isFading)
        {
            systemUI.blocksRaycasts = true;
            StartCoroutine(ChangingScene(playerData));
        }
    }

    IEnumerator Fading(float alpha, bool isFirst = false)
    {
        isFading = true;
        fadeUI.blocksRaycasts = true;
        systemUI.blocksRaycasts = !systemUI.blocksRaycasts;

        float fadeSpd = Mathf.Abs(fadeUI.alpha - alpha) / Settings.Instance.fadeTime;

        while(!Mathf.Approximately(fadeUI.alpha, alpha))
        {
            fadeUI.alpha = Mathf.MoveTowards(fadeUI.alpha, alpha, fadeSpd * Time.deltaTime);           
            yield return null;
        }

        fadeUI.blocksRaycasts = false;
        systemUI.blocksRaycasts = !systemUI.blocksRaycasts;
        isFading = false;
    }

    IEnumerator ChangingScene(PlayerData playerData)
    {
        // ���� ��ȯ�� �� ���� �� �ð��� ����
        GameDatas.pause = true;

        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        yield return StartCoroutine(Fading(1f, true));

        // ���� ���� �ִ� Prefab, Tile ���� ����

        // �÷��̾��� ��ġ�� ���� ��ġ�� �̵�

        EventHandler.CallBeforeSceneUnloadEvent();

        // ���� �� ��ε�
        yield return SceneManager.UnloadSceneAsync(EnumCaching.ToString(GameDatas.currentScene));

        // currentScene�� ����
        DataManager.Instance.ChangePlayerData(playerData, true);

        // ���� �� �ε�
        yield return SceneManager.LoadSceneAsync(EnumCaching.ToString(GameDatas.currentScene), LoadSceneMode.Additive);

        // �ε��� ���� Ÿ�� ����
        EventHandler.CallAfterSceneLoadEvent();

        if ((int)GameDatas.currentScene >= 5 && (int)GameDatas.currentScene <= 7)
            EventHandler.CallCreateMineEvent();

        if (GameDatas.yearChange)
            EventHandler.CallOpenMenuEvent(MenuIndex.bank);

        // �÷��̾� ��ġ ����
        // �ε��� ���� ������ ����
        EventHandler.CallAfterSceneLoadBeforeFadeInEvent();

        EventHandler.CallGoldChangeEvent();

        EventHandler.CallNightEvent();

        systemUI.alpha = 1.0f;

        yield return StartCoroutine(Fading(0f));

        if (GameDatas.HourMinuteGold.x < 6)
        {
            GameDatas.pause = true;
            EventHandler.CallSleepPanaltyEvent();
        }
        else GameDatas.pause = GameDatas.yearChange;

        GameDatas.LobbyScene = false;

        EventHandler.CallAFterSceneLoadFadeInEvent();
    }

    public void LoadLobby()
    {
        StartCoroutine(LoadingLobby());
    }

    IEnumerator LoadingLobby()
    {
        yield return StartCoroutine(Fading(1f, true));
        yield return SceneManager.LoadSceneAsync(EnumCaching.ToString(SceneName.Lobby), LoadSceneMode.Additive);
        GameDatas.yearChange = false;
        EventHandler.CallLoadLobby();
        GameDatas.currentScene = SceneName.Lobby;
        yield return StartCoroutine(Fading(0f));
    }
}
