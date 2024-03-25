using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractorBubble : MonoBehaviour
{
    [SerializeField] Transform player;
    bool active = false;

    private void OnEnable()
    {
        EventHandler.ActiveInteractorEvent += ActiveBubble;
    }
    private void OnDisable()
    {
        EventHandler.ActiveInteractorEvent -= ActiveBubble;
    }

    void ActiveBubble(bool active)
    {
        this.active = active;
        if (active)
            StartCoroutine(Following());
        else
            StopAllCoroutines();

        transform.GetChild(0).gameObject.SetActive(active);
    }

    IEnumerator Following()
    {
        while (active)
        {
            transform.position = Camera.main.WorldToScreenPoint(player.position);
            yield return null;
        }
    }
}
