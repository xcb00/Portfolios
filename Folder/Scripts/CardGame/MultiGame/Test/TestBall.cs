using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBall : MonoBehaviourPun // MonoBehaviourPun�� ��ӹ��� ��� PhotonView�� ��� ���� ����
{
    // PhotonNetwork.IsMasterClient : ���� Ŭ���̾�Ʈ�� ����(ȣ��Ʈ)�ΰ�
    // photonView.IsMine : ���� ������Ʈ�� ���ÿ��� ������ ������Ʈ�ΰ�
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
