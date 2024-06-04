using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BettingPanel : MonoBehaviour
{
    [SerializeField] UnityEvent StartPlayingEvent;
    [SerializeField] UnityEvent DrawPlayerCard;
    public bool canPlaying = false;

    public void CanPlaying(bool b) => canPlaying = b;

    public void StartPlaying()
    {
        if (!canPlaying)
            return;

        gameObject.SetActive(false);
        StartPlayingEvent?.Invoke();
        DrawPlayerCard?.Invoke();
    }
}
