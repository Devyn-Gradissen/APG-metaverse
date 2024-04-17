using UnityEngine;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class OculusVRChatController : MonoBehaviour
{
    public GameObject chatCanvas;

    // Adjust this button based on your Oculus Touch controller setup
    public OVRInput.Button toggleChatButton = OVRInput.Button.One;

    private bool chatVisible = false;

    void Update()
    {
        // Check for button press
        if (OVRInput.GetDown(toggleChatButton))
        {
            // Toggle chat visibility
            chatVisible = !chatVisible;
            chatCanvas.SetActive(chatVisible);
        }
    }
}
