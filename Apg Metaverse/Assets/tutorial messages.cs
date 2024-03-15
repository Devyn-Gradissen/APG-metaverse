using UnityEngine;
using TMPro;

public class TutorialMessage : MonoBehaviour
{
    public TMP_Text lookAroundText; // Text for looking around
    public TMP_Text forwardText; // Text for moving forward
    public TMP_Text backwardText; // Text for moving backward
    public TMP_Text leftText; // Text for moving left
    public TMP_Text rightText; // Text for moving right
    public TMP_Text finalText; // Final message
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

    private float activityTimer = 0f;

    void Start()
    {
        if (lookAroundText != null)
        {
            lookAroundText.enabled = false;
            Invoke("ShowLookAroundMessage", 2f); // Show look around message after 2 seconds
        }
        else
        {
            Debug.LogError("Look around text component not assigned in the Unity editor!");
        }

        if (forwardText != null)
        {
            forwardText.enabled = false;
        }
        else
        {
            Debug.LogError("Forward text component not assigned in the Unity editor!");
        }

        if (backwardText != null)
        {
            backwardText.enabled = false;
        }
        else
        {
            Debug.LogError("Backward text component not assigned in the Unity editor!");
        }

        if (leftText != null)
        {
            leftText.enabled = false;
        }
        else
        {
            Debug.LogError("Left text component not assigned in the Unity editor!");
        }

        if (rightText != null)
        {
            rightText.enabled = false;
        }
        else
        {
            Debug.LogError("Right text component not assigned in the Unity editor!");
        }

        if (finalText != null)
        {
            finalText.enabled = false;
        }
        else
        {
            Debug.LogError("Final text component not assigned in the Unity editor!");
        }
    }

    void ShowLookAroundMessage()
    {
        if (lookAroundText != null)
        {
            lookAroundText.enabled = true;
        }
    }

    void Update()
    {
        bool playerMoved = Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0 || Input.GetKey(forwardKey) || Input.GetKey(backwardKey) || Input.GetKey(leftKey) || Input.GetKey(rightKey);
        
        if (playerMoved)
        {
            activityTimer += Time.deltaTime;
        }
        else
        {
            activityTimer = 0f; // Reset timer when player stops moving or looking around
        }

        // If activity exceeds 4 seconds and final message is shown, hide it
        if (activityTimer > 3f && finalText.enabled)
        {
            finalText.enabled = false;
        }

        if (lookAroundText.enabled && Input.GetAxis("Mouse X") != 0 && Input.GetAxis("Mouse Y") != 0)
        {
            if (lookAroundText != null)
            {
                // Change text color to green
                lookAroundText.color = Color.green;
                
                // Hide look around message with a delay of 1 second
                Invoke("HideLookAroundMessage", 2f);
            }
        }

        if (forwardText.enabled && Input.GetKeyDown(forwardKey))
        {
            if (forwardText != null)
            {
                // Change text color to green
                forwardText.color = Color.green;
                
                // Hide forward message with a delay of 1 second
                Invoke("HideForwardMessage", 1.5f);
            }
        }

        if (backwardText.enabled && Input.GetKeyDown(backwardKey))
        {
            if (backwardText != null)
            {
                // Change text color to green
                backwardText.color = Color.green;
                
                // Hide backward message with a delay of 1 second
                Invoke("HideBackwardMessage", 1.5f);
            }
        }

        if (leftText.enabled && Input.GetKeyDown(leftKey))
        {
            if (leftText != null)
            {
                // Change text color to green
                leftText.color = Color.green;
                
                // Hide left message with a delay of 1 second
                Invoke("HideLeftMessage", 1.5f);
            }
        }

        if (rightText.enabled && Input.GetKeyDown(rightKey))
        {
            if (rightText != null)
            {
                // Change text color to green
                rightText.color = Color.green;
                
                // Hide right message with a delay of 1 second
                Invoke("HideRightMessage", 1.5f);
            }
        }
    }

    void HideLookAroundMessage()
    {
        if (lookAroundText != null)
        {
            lookAroundText.enabled = false;
            forwardText.enabled = true;
        }
    }

    void HideForwardMessage()
    {
        if (forwardText != null)
        {
            forwardText.enabled = false;
            backwardText.enabled = true;
        }
    }

    void HideBackwardMessage()
    {
        if (backwardText != null)
        {
            backwardText.enabled = false;
            leftText.enabled = true;
        }
    }

    void HideLeftMessage()
    {
        if (leftText != null)
        {
            leftText.enabled = false;
            rightText.enabled = true;
        }
    }

    void HideRightMessage()
    {
        if (rightText != null)
        {
            rightText.enabled = false;
            finalText.enabled = true;
        }
    }
}
