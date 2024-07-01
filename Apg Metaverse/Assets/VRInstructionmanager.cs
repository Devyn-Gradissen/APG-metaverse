using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR;

public class VRInstructionManager : MonoBehaviour
{
    public List<TextMeshPro> instructionTexts; // List of TextMeshPro to display instructions
    private List<string> instructions; // List of instructions
    private int currentInstructionIndex = 0; // Track the current instruction index
    private bool instructionCompleted = false; // Track if the current instruction is completed

    void Start()
    {
        // Initialize the instructions
        instructions = new List<string>
        {
            "Beweeg de joystick van de rechter controller om om je heen te kijken",
            "Klik de B knop. De B knop gebruik je om te klikken op bijvoorbeeld een knop in een lift",
            "Beweeg je linker joystick om te lopen"
        };

        // Initialize TextMeshPro objects
        InitializeInstructions();
    }

    void Update()
    {
        // Check VR input for the current instruction
        if (!instructionCompleted && currentInstructionIndex < instructions.Count)
        {
            if (IsCurrentInstructionCompleted())
            {
                CompleteInstruction();
            }
        }
    }

    void InitializeInstructions()
    {
        for (int i = 0; i < instructionTexts.Count; i++)
        {
            if (i < instructions.Count)
            {
                instructionTexts[i].text = instructions[i];
                instructionTexts[i].color = Color.white;
                instructionTexts[i].gameObject.SetActive(i == currentInstructionIndex); // Show only the current instruction
            }
            else
            {
                instructionTexts[i].gameObject.SetActive(false);
            }
        }
    }

    bool IsCurrentInstructionCompleted()
    {
        switch (currentInstructionIndex)
        {
            case 0:
                // Check for right joystick movement
                return CheckRightJoystickMovement();
            case 1:
                // Check for B button press
                return Input.GetKeyDown(KeyCode.JoystickButton1); // B button is typically button 1
            case 2:
                // Check for left joystick movement
                return CheckLeftJoystickMovement();
            default:
                return false;
        }
    }

    bool CheckRightJoystickMovement()
    {
        Vector2 joystickValue;
        if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out joystickValue))
        {
            return joystickValue != Vector2.zero;
        }
        return false;
    }

    bool CheckLeftJoystickMovement()
    {
        Vector2 joystickValue;
        if (InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out joystickValue))
        {
            return joystickValue != Vector2.zero;
        }
        return false;
    }

    void CompleteInstruction()
    {
        // Mark the instruction as completed
        instructionCompleted = true;

        // Change text color to green
        instructionTexts[currentInstructionIndex].color = Color.green;

        // Hide current instruction and show next after 2 seconds
        Invoke("NextInstruction", 2.0f);
    }

    void NextInstruction()
    {
        instructionTexts[currentInstructionIndex].gameObject.SetActive(false); // Hide current instruction

        currentInstructionIndex++;
        instructionCompleted = false; // Reset the completion flag

        if (currentInstructionIndex < instructions.Count)
        {
            instructionTexts[currentInstructionIndex].gameObject.SetActive(true); // Show next instruction
        }
        else
        {
            // All instructions are complete
            foreach (var text in instructionTexts)
            {
                text.gameObject.SetActive(false);
            }
        }
    }
}
