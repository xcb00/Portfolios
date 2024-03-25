using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingImage : MonoBehaviour
{
    [SerializeField] Sprite[] playerSprite;
    [SerializeField] float animationTime = 0.3f;
    [SerializeField] Image playerImage;
    Coroutine routine = null;

    private void OnEnable()
    {
        routine = StartCoroutine(MovingPlayer());
    }

    private void OnDisable()
    {
        if(routine!=null)
        {
            StopCoroutine(routine);
            routine = null;
        }    
    }    

    IEnumerator MovingPlayer()
    {
        int count = 0;
        float t = 0.0f;
        while (gameObject.activeSelf)
        {
            while (t < animationTime)
            {
                t += Time.deltaTime;
                yield return null;
            }
            t = 0.0f;
            count = ++count % playerSprite.Length;
            playerImage.sprite = playerSprite[count];
        }
    }
}
