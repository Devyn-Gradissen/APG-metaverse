using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCameraController1 : MonoBehaviourPun
{
    public Camera playerCamera;
    void Start()
    {
        if (photonView.IsMine)
        {
            // Enable the local player's camera
            playerCamera.gameObject.SetActive(true);
        }
        else
        {
            // Disable other players' cameras
            playerCamera.gameObject.SetActive(false);
        }
    } 
}