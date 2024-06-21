using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class roommanager : MonoBehaviourPunCallbacks
{
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
}
