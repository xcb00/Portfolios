using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioClip bgm = null;
    AudioSource player = null;
    
    void Start()
    {
        player = GetComponent<AudioSource>();
        player.clip = bgm;
    }

    private void OnEnable()
    {
        EventHandler.ChangeSettingEvent += SetVolume;
    }
    private void OnDisable()
    {
        EventHandler.ChangeSettingEvent -= SetVolume;
    }

    void SetVolume()
    {
        if (Mathf.Approximately(Settings.Instance.volume, player.volume)) return;

        /*if (Mathf.Approximately(Settings.Instance.volume, 0f))
        {
            if (player.isPlaying) player.Stop();
        }
        else
        {
            player.volume = Settings.Instance.volume * 0.1f;
            if (!player.isPlaying) player.Play();
        }*/

        StartCoroutine(ChangingVolume());
    }

    IEnumerator ChangingVolume()
    {
        float from = player.volume;
        float to = Settings.Instance.volume * 0.1f;

        float t = 0.0f;
        float time = 0.2f;

        while (t < time)
        {
            t += Time.deltaTime;
            if (t > time) t = time;
            player.volume = Mathf.Lerp(from, to, t / time);
            yield return null;
        }
        player.volume = to;

        if (Mathf.Approximately(to, 0.0f))
        { if (player.isPlaying) player.Stop(); }
        else
        { if (!player.isPlaying) player.Play(); }
         
    }
}
