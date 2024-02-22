using UnityEngine;

public class MovementController : MonoBehaviour
{
    public GameObject player;
    public GameObject xrRig;

    void Start()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        // Check if the application is running in VR
        if (UnityEngine.XR.XRSettings.enabled)
        {
            // If VR is enabled, use XR Rig
            xrRig.SetActive(true);
            player.SetActive(false);
        }
        else
        {
            // If VR is not enabled, use PlayerMovementTest
            player.SetActive(true);
            xrRig.SetActive(false);
        }
#else
        // If not running on Windows, always use XR Rig
        xrRig.SetActive(true);
        player.SetActive(false);
#endif
    }
}
