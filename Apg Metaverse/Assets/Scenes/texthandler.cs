using UnityEngine;
using UnityEngine.XR;
using TMPro;

public class VRTextManager : MonoBehaviour
{
    public TextMeshPro instructionText;

    void Start()
    {
        // Check if the application is running in VR mode
        if (XRSettings.isDeviceActive)
        {
            // The application is running in VR mode, so enable the TextMeshPro component
            instructionText.gameObject.SetActive(true);
        }
        else
        {
            // The application is not running in VR mode, so disable the TextMeshPro component
            instructionText.gameObject.SetActive(false);
        }
    }
}