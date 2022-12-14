using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class CameraSetup : MonoBehaviourPun
{
    private CinemachineVirtualCamera followCam;

    void Start()
    {
        followCam = FindObjectOfType<CinemachineVirtualCamera>();
        if (photonView.IsMine)
        {
            followCam.Follow = transform;
        }
        else
        {
            followCam.enabled = false;
        }
    }
}
