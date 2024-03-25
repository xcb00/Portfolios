using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPreview : MonoBehaviour
{
    [SerializeField] protected float zPos;
    [SerializeField] LayerMask cantBuild;
    protected Collider[] colliders;
    protected float xPos;

    //public void SetPosition(Vector3 pos) { transform.position = pos; }

    public void ActivePreview()
    {
        gameObject.SetActive(true);
    }

    public void InactivePreview()
    {
        gameObject.SetActive(false);
    }

    public void FollowCursor(RaycastHit hit)
    {
        float x = Mathf.Round(hit.point.x);
        transform.position = new Vector3(x, 0.5f, zPos);
    }

    public virtual void Build(RaycastHit hit)
    {
        xPos = Mathf.Round(hit.point.x);
        colliders = Physics.OverlapSphere(new Vector3(xPos, 0.5f, zPos), 1f, cantBuild);
    }
}
