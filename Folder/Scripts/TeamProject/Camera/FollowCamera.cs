using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target = null;
    Vector3 _currentVelocity = Vector3.zero;
    private void LateUpdate()
    {
            Vector3 newPos = new Vector3(target.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, newPos, ref _currentVelocity, 0.3f);
    }
}
