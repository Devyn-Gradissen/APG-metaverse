using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class roommanager : MonoBehaviourPunCallbacks
{
    public GameObject player;

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

    PhotonNetwork.JoinOrCreateRoom( "test",  null,  null);

    Debug.Log("we're connectred and in a room");
  }

  public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("we're connected and in a room!");

        // GameObject player = PhotonNetwork.Instantiate(player.name);
    }
}
