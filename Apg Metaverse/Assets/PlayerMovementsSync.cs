using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovementSync : MonoBehaviourPun, IPunObservable
{
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            networkPosition = transform.position;
            networkRotation = transform.rotation;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // Local player movement and rotation logic
            // Example: This can be replaced with your own movement logic
            float move = Input.GetAxis("Vertical") * Time.deltaTime * 5f;
            float strafe = Input.GetAxis("Horizontal") * Time.deltaTime * 5f;
            transform.Translate(strafe, 0, move);
        }
        else
        {
            // Interpolate movement for smoother remote player movements
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send the current position and rotation to other players
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Receive the position and rotation from other players
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}