using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class RegistManager : MonoBehaviour
{
    public CanvasGroup register;
    public Toggle policy, gpgs;
    public Button regist;

    private void OnDisable() => StopAllCoroutines();

    private void Awake()
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("Regist")))
            SceneChange.ChangeScene(SceneName.Regist, SceneName.Lobby, null, ()=> GameManager.Inst.StartGame(), null, null, true);
        else
            register.alpha = 1.0f;
    }

    public void CheckToggle()
    {
        regist.interactable = policy.isOn && gpgs.isOn;
    }

    public void RegistButton() => SceneChange.ChangeScene(SceneName.Regist, SceneName.Lobby, null, () => GameManager.Inst.StartGame(), null, () => PlayerPrefs.SetString("Regist", "SAVE"), true);

    public void PolicyButton() => GameManager.Inst.ShowPolicy();
}
