using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample2DPlayer : MonoBehaviour
{
    public float speed = 1.0f;
    Vector2 dir = Vector2.zero;

    // Update is called once per frame
    void Update()
    {
        dir.x = Input.GetAxis("Horizontal");
        dir.y = Input.GetAxis("Vertical");

        transform.Translate(Time.deltaTime * speed * dir);
    }

    public void MovePosition(Vector3 position) => transform.position = position;
}
