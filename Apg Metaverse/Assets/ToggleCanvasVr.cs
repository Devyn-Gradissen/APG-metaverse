using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ToggleCanvasVr : MonoBehaviour
{
    public CanvasGroup canvasGroup; // Reference to your CanvasGroup
    private bool isCanvasVisible = false;

    private InputAction toggleAction;

    void Awake()
    {
        // Set up the action for toggling the canvas
        toggleAction = new InputAction("ToggleCanvas", binding: "<XRController>{RightHand}/primaryButton");
        toggleAction.performed += ctx => ToggleCanvasVisibility();
        toggleAction.Enable();

        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup is not assigned.");
        }
        else
        {
            // Hide the canvas at the start
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            Debug.Log("Canvas is now hidden at start.");
        }
    }

    void OnDestroy()
    {
        toggleAction.Disable();
        toggleAction.performed -= ctx => ToggleCanvasVisibility();
    }

    public void ToggleCanvasVisibility()
    {
        isCanvasVisible = !isCanvasVisible;
        canvasGroup.alpha = isCanvasVisible ? 1 : 0;
        canvasGroup.interactable = isCanvasVisible;
        canvasGroup.blocksRaycasts = isCanvasVisible;
        Debug.Log($"Canvas visibility toggled. Now visible: {isCanvasVisible}");
    }
}
