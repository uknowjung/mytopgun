using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Health : MonoBehaviourPunCallbacks, IPunObservable
{
    public float health = 1f;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            health = (float)stream.ReceiveNext();
        }
    }

    void Update()
    {
        //if (health <= 0f)
        //{
        //    if (transform.position.y > 100)
        //    {
        //        transform.Translate(0, -100 * Time.deltaTime, 0);
        //    }
        //}
    }

    void OnParticleCollision()
    {
        if (health > 0)
        {
            health -= 0.2f;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Floor"))
        {
            health = -0.2f;
        }
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            health = -0.2f;
        }
    }
}
