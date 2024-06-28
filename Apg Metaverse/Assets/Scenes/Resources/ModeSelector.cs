using UnityEngine;
using System.Collections;

public class ModeSelector : MonoBehaviour
{
    public GameObject player;
    public GameObject xrRig;

    void Start()
    {
        if (!IsLocalPlayer()) return; // Ensure this script only runs for the local player

        StartCoroutine(InitializeBasedOnPlatform());
    }

    bool IsLocalPlayer()
    {
        // Replace with your multiplayer framework's check
        return true; // Placeholder - Update with actual check
    }

    IEnumerator InitializeBasedOnPlatform()
    {
        yield return new WaitForEndOfFrame(); // Delay initialization to ensure all objects are loaded

        #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (UnityEngine.XR.XRSettings.enabled)
        {
            Debug.Log("VR is enabled. Using XR Rig.");
            xrRig.SetActive(true);
            player.SetActive(false);
        }
        else
        {
            Debug.Log("VR is not enabled. Using Player.");
            player.SetActive(true);
            xrRig.SetActive(false);
        }
        #else
        Debug.Log("Non-Windows platform. Using XR Rig.");
        xrRig.SetActive(true);
        player.SetActive(false);
        #endif
    }
}
