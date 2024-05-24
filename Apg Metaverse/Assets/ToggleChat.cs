using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class ToggleChat : MonoBehaviour
{
    public TextMeshPro sampleText; // textmeshpro text bestand
    private bool isTextVisible = true; 
    private bool isButtonPressed = false; // bekijk of de knop wordt ingedrukt

    // verbind alles in de unity inspector
    public InputHelpers.Button toggleButton = InputHelpers.Button.PrimaryButton;
    public XRController rightController;

    void Start()
    {
        if (sampleText == null)
        {
            Debug.LogError("TextMeshPro text component is not assigned.");
        }
    }

    void Update()
    {
        if (rightController)
        {
            bool buttonPressed = IsButtonPressed(rightController, toggleButton);

            // bekijk of de knop wordt ingedrukt
            if (buttonPressed && !isButtonPressed)
            {
                ToggleVisibility();
            }

            // uodate de staat van de knop
            isButtonPressed = buttonPressed;
        }
    }

    private bool IsButtonPressed(XRController controller, InputHelpers.Button button)
    {
        InputHelpers.IsPressed(controller.inputDevice, button, out bool isPressed);
        return isPressed;
    }

    public void ToggleVisibility()
    {
        isTextVisible = !isTextVisible;
        sampleText.gameObject.SetActive(isTextVisible);
        Debug.Log($"Text visibility toggled. Now visible: {isTextVisible}");
    }
}
