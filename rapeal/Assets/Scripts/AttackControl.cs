using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AttackControl : MonoBehaviourPun
{
    private bool attack_bool = false;

    public ParticleSystem ps;

    private Tracking tracking;
    public int attack;

    void Start()
    {
        tracking = GetComponent<Tracking>();
    }

    void Update()
    {
        attack = tracking.attackMode;

        if (attack == 1 && photonView.IsMine)
        {
            if (!attack_bool)
            {
                attack_bool = true;
                photonView.RPC("RPC_Shoot", RpcTarget.All);
            }
        }
        else
        {
            if (attack_bool)
            {
                attack_bool = false;
                photonView.RPC("RPC_Stop", RpcTarget.All);
            }
        }
        //if (!attack && !ps1.isPlaying && !ps2.isPlaying && !ps3.isPlaying && !ps4.isPlaying) { attack = true; }
        //else { attack = false; }
    }

    void LateUpdate()
    {
        //if (attack == true) { RPC_Shoot(); }
        //else { RPC_Stop(); }
    }

    [PunRPC]
    void RPC_Shoot()
    {
        //ps1.Play();
        //ps2.Play();
        //ps3.Play();
        ps.Play();
    }

    [PunRPC]
    void RPC_Stop()
    {
        //ps1.Stop();
        //ps2.Stop();
        //ps3.Stop();
        ps.Stop();
    }
}
