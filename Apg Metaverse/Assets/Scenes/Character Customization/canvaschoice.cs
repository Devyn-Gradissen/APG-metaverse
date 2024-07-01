using UnityEngine;
using UnityEngine.XR;

public class CanvasController : MonoBehaviour
{
    public GameObject vrCanvas;
    public GameObject desktopCanvas;

    void Start()
    {
        if (XRSettings.enabled)
        {
            // VR is enabled, enable VR canvas
            vrCanvas.SetActive(true);
            desktopCanvas.SetActive(false);
        }
        else
        {
            // VR is not enabled, enable desktop canvas
            vrCanvas.SetActive(false);
            desktopCanvas.SetActive(true);
        }
    }
}
