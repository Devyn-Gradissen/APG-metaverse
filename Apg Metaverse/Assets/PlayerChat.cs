using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerChat : MonoBehaviourPun
{
    public Canvas playerChatCanvas;

    void Start()
    {
        if (photonView.IsMine)
        {
            // Enable the local player's Canvas
            playerChatCanvas.gameObject.SetActive(true);
        }
        else
        {
            // Disable other players' Canvases
            playerChatCanvas.gameObject.SetActive(false);
        }
    } 
}