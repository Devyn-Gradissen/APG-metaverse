using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    public string SampleSceneVR = "SampleSceneVR";
    public string SampleScene = "SampleScene";
    public Button vrButton;
    public Button desktopButton;

    private void Start()
    {
        bool isVR = UnityEngine.XR.XRSettings.enabled;

        if (isVR)
        {
            if (desktopButton != null)
            {
                desktopButton.interactable = false;
                desktopButton.GetComponent<Image>().color = Color.gray;
            }
        }
        else
        {
            if (vrButton != null)
            {
                vrButton.interactable = false;
                vrButton.GetComponent<Image>().color = Color.gray;
            }
        }
    }

    public void ChangeScene()
    {
        bool isVR = UnityEngine.XR.XRSettings.enabled;

        if (isVR)
        {
            SceneManager.LoadScene(SampleSceneVR);
        }
        else
        {
            SceneManager.LoadScene(SampleScene);
        }
    }
}

public class VRButtonHandler : MonoBehaviour, IPointerClickHandler
{
    public SceneChanger sceneChanger;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (sceneChanger != null)
        {
            sceneChanger.ChangeScene();
        }
    }
}

public class DesktopButtonHandler : MonoBehaviour, IPointerClickHandler
{
    public SceneChanger sceneChanger;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (sceneChanger != null)
        {
            sceneChanger.ChangeScene();
        }
    }
}
