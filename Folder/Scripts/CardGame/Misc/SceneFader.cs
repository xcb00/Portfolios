using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneFader : Singleton<SceneFader>
{
    CanvasGroup canvas;
    bool delay = false;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable() => EventHandler.FadeDelayEvent += FadeDelayEvent;
    private void OnDisable() => EventHandler.FadeDelayEvent -= FadeDelayEvent;

    void FadeDelayEvent(bool startDelay) => delay = startDelay;


    public async Task Fade(float time, float startAlpha, float targetAlpha, bool delay = false)
    {
        if (canvas == null)
            canvas = GetComponent<CanvasGroup>();

        canvas.blocksRaycasts = transform;

        float t = 0.0f;

        this.delay = delay;

        while (this.delay)
            await Task.Yield();

        while (t < time)
        {
            t += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / time);
            await Task.Yield();
        }
        canvas.alpha = targetAlpha;
        canvas.blocksRaycasts = false;
    }
}
