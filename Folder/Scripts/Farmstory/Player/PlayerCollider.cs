using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCollider : MonoBehaviour
{
    public UnityEvent InactiveJoystick;
    [SerializeField] LayerMask item;
    [SerializeField] LayerMask enemy;
    [SerializeField] LayerMask teleport;
        
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((item & 1 << collision.gameObject.layer) != 0)
        {
            collision.GetComponentInParent<ItemPrefab>().OnReturn();
        }

        if ((teleport & 1 << collision.gameObject.layer) != 0)
            SceneControlManager.Instance.ChangeScene(collision.GetComponent<SceneChanger>().data);

        // InteractorManager를 만들어서 관리
        if (((1 << (int)LayerName.InteractionPoint) & (1 << collision.gameObject.layer)) != 0)
            collision.GetComponent<InteractionPoint>().isActive();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << (int)LayerName.InteractionPoint) & (1 << collision.gameObject.layer)) != 0)
            EventHandler.CallActiveInteractor(false);

    }
}
