using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDetector : MonoBehaviour
{
    Enemy parent;

    private void Start()
    {
        parent = GetComponentInParent<Enemy>();   
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << (int)LayerName.Player) & (1 << collision.gameObject.layer)) != 0)
            parent.EnterPlayer(true, collision.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << (int)LayerName.Player) & (1 << collision.gameObject.layer)) != 0)
            parent.EnterPlayer(false);
    }
}
