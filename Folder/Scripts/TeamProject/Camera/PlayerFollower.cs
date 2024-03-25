using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    public float spd;
    public Transform target;
    public float dist = 8.0f;
    bool following = false;

    private void Start()
    {
        transform.position = target.position;
    }

    private void Update()
    {
        if (!following && Vector3.Distance(target.position, transform.position) > dist)
        {
            following = true;
            StartCoroutine(Following());
        }
    }

    IEnumerator Following()
    {
        while (following)
        {
            float _dist = target.position.x - transform.position.x;
            if (Mathf.Abs(_dist) < dist)
                following = false;
            else
            {
                transform.Translate((_dist > 0f ? Vector3.right : Vector3.left) * Time.deltaTime * target.GetComponent<TmpPlayer>().playerSpeed);
            }
            yield return null;
        }
    }
}
