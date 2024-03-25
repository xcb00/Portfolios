using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample3DPlayer : MonoBehaviour
{
    public float speed = 1.0f;
    Vector3 dir = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        dir.x = Input.GetAxis("Horizontal");
        dir.z = Input.GetAxis("Vertical");

        transform.Translate(Time.deltaTime * speed * dir);
    }

    public void MovePosition(Vector3 position) => transform.position = position;
}
