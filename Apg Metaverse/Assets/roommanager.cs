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
            roomOptions.MaxPlayers = 100;
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

        // Ensure that the player is only instantiated once per player
        if (Player == null)
        {
            Debug.LogError("Player prefab is not assigned!");
            return;
        }

        // PhotonNetwork.Instantiate will instantiate the object across the network
        GameObject player = PhotonNetwork.Instantiate(Player.name, spawnPoint.position, Quaternion.identity);
        Debug.Log("Player instantiated at position: " + spawnPoint.position);

        // Ensure that the CameraController script is properly set up
        PlayerCameraController1 cameraController = player.GetComponent<PlayerCameraController1>();
        if (cameraController != null && cameraController.playerCamera == null)
        {
            cameraController.playerCamera = player.GetComponentInChildren<Camera>();
        }

        // Log camera settings
        if (cameraController != null && cameraController.playerCamera != null)
        {
            Debug.Log("Camera assigned to player");
        }
        else
        {
            Debug.LogError("Camera not assigned to player");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("New player entered room: " + newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log("Player left room: " + otherPlayer.NickName);
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

    void Update()
    {
        Debug.Log("IsConnected: " + PhotonNetwork.IsConnected);
        Debug.Log("IsInRoom: " + PhotonNetwork.InRoom);
    }
}
