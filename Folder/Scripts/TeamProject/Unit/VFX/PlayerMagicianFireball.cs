using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMagicianFireball : MonoBehaviour
{
    public float rayPointX=-3.0f;
    Transform rayPoint;
    ParticleSystem ps = null;
    TrailRenderer tr = null;
    Vector3 position;
    Coroutine co = null;
    float range = 0.0f;

    private void OnEnable()
    {
        if(ps==null) ps = GetComponentInChildren<ParticleSystem>();
        if(tr==null) tr = GetComponentInChildren<TrailRenderer>();
        range = GameDatas.playerUnitDetailList[(int)PlayerUnitType.magician].range;
        rayPoint = transform.GetChild(0);
        //rayPoint.localPosition = new Vector3(rayPointX, 0f, 0f);
        tr.Clear();
        tr.time = Settings.ballTrailTime / Settings.ballSpeed;
        position = transform.position;
        StartCoroutine(MovingFireball());
    }

    IEnumerator MovingFireball()
    {
        float t = Settings.ballTime;
        ps.Play();
        while (gameObject.activeSelf)
        {
            t -= Time.deltaTime;

            if (t < 0.0f) Inactive();

            if (Physics.Raycast(rayPoint.position, Vector3.right, out RaycastHit hit, Time.deltaTime * Settings.ballSpeed, 1 << (int)LayerName.ComUnit))
            {
                hit.transform.GetComponentInParent<IDamage>().GetDamage(GameDatas.playerUnitDetailList[(int)PlayerUnitType.magician].damage);
                Inactive();
            }

            transform.Translate(transform.right * Time.deltaTime * Settings.ballSpeed);
            yield return null;
        }
    }

    void Inactive()
    {
        transform.position = position;
        StopAllCoroutines();
        PoolManager.Instance.EnqueueObject(PoolType.PlayerFireball, gameObject);
    }
}
