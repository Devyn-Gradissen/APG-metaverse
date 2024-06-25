using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class roommanager : MonoBehaviourPunCallbacks
{
    public GameObject Player;
    public Transform spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting....");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PhotonNetwork.JoinOrCreateRoom("test", null, null);
        Debug.Log("We're connected and in a room");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We're connected and in a room!");
        GameObject player = PhotonNetwork.Instantiate(Player.name, spawnPoint.position, Quaternion.identity);

        PlayerCameraController1 cameraController = player.GetComponent<PlayerCameraController1>();
        if (cameraController != null && cameraController.playerCamera == null)
        {
            cameraController.playerCamera = player.GetComponentInChildren<Camera>();
        }

        PlayerChat playerChat = player.GetComponent<PlayerChat>();
        if (playerChat != null && playerChat.playerChatCanvas == null)
        {
            playerChat.playerChatCanvas = player.GetComponentInChildren<Canvas>();
        }
    }
}