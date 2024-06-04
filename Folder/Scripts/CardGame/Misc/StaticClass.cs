using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

#region Enum Caching
public static class EnumCaching
{
    static Dictionary<Enum, string> EnumDic = new Dictionary<Enum, string>();
    public static string ToString(Enum key)
    {
        if (EnumDic.TryGetValue(key, out var value)) return value;
        EnumDic[key] = key.ToString();
        return EnumDic[key];
    }
}
#endregion

#region SceneChange
public static class SceneChange
{
    public static async void ChangeScene(SceneName current, SceneName next, Action beforeFadeIn, Action afterFadeIn, Action beforeFadeOut, Action afterFadeOut, bool delay = false)
    {
        beforeFadeIn?.Invoke();

        SceneManager.LoadScene("Load", LoadSceneMode.Additive);

        await Delay();

        await SceneFader.Inst.Fade(0.1f, 0.0f, 1.0f);

        afterFadeIn?.Invoke();

        if(current != SceneName.None) 
            SceneManager.UnloadSceneAsync(EnumCaching.ToString(current));

        await Delay();

        SceneManager.LoadScene(EnumCaching.ToString(next), LoadSceneMode.Additive);

        beforeFadeOut?.Invoke();

        await SceneFader.Inst.Fade(0.1f, 1.0f, 0.0f, delay);

        SceneManager.UnloadSceneAsync("Load");

        afterFadeOut?.Invoke();
    }

    static async Task Delay() => await Task.Delay(100);

    /*public static void CheckManagerScene()
    {
        for(int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name.Equals(EnumCaching.ToString(SceneName.Manager)))
                return;
        }
        SceneManager.LoadScene(EnumCaching.ToString(SceneName.Manager), LoadSceneMode.Additive);
    }*/
}
#endregion

#region Player Data
[Serializable]
public class PlayerData
{
    public int gold;
    public Vector3Int singleRecord;
    public Vector3Int multiRecord;

    public PlayerData()
    {
        gold = 500;
        singleRecord = Vector3Int.zero;
        multiRecord = Vector3Int.zero;
    }

    public PlayerData(PlayerDataJson json)
    {
        gold = json.gold;
        singleRecord = json.singleRecord;
        multiRecord = json.multiRecord;
    }
}

public struct PlayerDataJson
{
    public int gold;
    public Vector3Int singleRecord;
    public Vector3Int multiRecord;
    public PlayerDataJson(PlayerData data)
    {
        gold = data.gold;
        singleRecord = data.singleRecord;
        multiRecord = data.multiRecord;
    }
}
#endregion