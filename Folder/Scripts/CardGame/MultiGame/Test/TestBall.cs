using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBall : MonoBehaviourPun // MonoBehaviourPun을 상속받을 경우 PhotonView에 즉시 접근 가능
{
    // PhotonNetwork.IsMasterClient : 현재 클라이언트가 방장(호스트)인가
    // photonView.IsMine : 현재 오브젝트가 로컬에서 생성된 오브젝트인가
    public bool IsMasterClientLocal => PhotonNetwork.IsMasterClient && photonView.IsMine;

    Vector2 dir = Vector2.right;
    float spd = 10f;
    float randomRefection = 0.15f;

    private void FixedUpdate()
    {
        if (!IsMasterClientLocal) return;

        var dist = spd * Time.deltaTime;
        var hit = Physics2D.Raycast(transform.position, dir, dist);

        if (hit.collider != null)
        {
            dir = Vector2.Reflect(dir, hit.normal);
            dir += Random.insideUnitCircle * randomRefection;
        }

        transform.position = (Vector2)transform.position + dir * dist;
    }
}
