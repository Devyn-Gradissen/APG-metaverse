using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject Player;
    public Transform spawnPoint;

    void Start()
    {
        Debug.Log("Connecting....");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to Master server");
        JoinLobby();
    }

    void JoinLobby()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("Joining Lobby...");
            PhotonNetwork.JoinLobby();
        }
        else
        {
            Debug.LogError("Not connected or not ready to join lobby");
        }
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("Joined Lobby");
        JoinOrCreateRoom();
    }

    void JoinOrCreateRoom()
    {
        if (PhotonNetwork.InLobby)
        {
            Debug.Log("Joining or Creating Room...");
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 10;
            PhotonNetwork.JoinOrCreateRoom("test", roomOptions, TypedLobby.Default);
        }
        else
        {
            Debug.LogError("Not in lobby, cannot join or create room");
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Room!");
        GameObject player = PhotonNetwork.Instantiate(Player.name, spawnPoint.position, Quaternion.identity);
        player.GetComponent<PlayerSetup>().ISLocalPlayer();
        // Ensure that the CameraController script is properly set up
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

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.LogError("Disconnected from server: " + cause.ToString());
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.LogError("Failed to join room: " + message);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.LogError("Failed to create room: " + message);
    }
}
