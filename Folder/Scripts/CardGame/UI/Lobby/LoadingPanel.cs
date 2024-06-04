using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    [SerializeField] Transform loadingImg;

    private void OnEnable() => StartCoroutine(Loading());
    private void OnDisable() => StopAllCoroutines();

    IEnumerator Loading()
    {
        loadingImg.transform.localRotation = Quaternion.identity;
        while (true)
        {
            loadingImg.Rotate(Vector3.forward * Time.deltaTime * 100f);
            yield return null;
        }
    }
    
    
}
