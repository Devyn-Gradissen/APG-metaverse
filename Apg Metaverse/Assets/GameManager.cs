using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

     ArrayList credentials;  
     
    public Text welcomeText;

    public string username; // User identifier (deze houd je gebruikersnaam vast )

    // Manages message history size
    public int maxMessages = 25; // Message history limit (Determines the maximum number of messages in the chat history)

    // UI elements
    public GameObject chatPanel, textObject; // Visual chat elements (References to the chat panel and text objects in the Unity UI)
    public InputField chatBox; // Input field for chat messages (The input field where the player enters messages)

    public Color playerMessage, info; // Message color options (Colors for different types of messages)

    // Private list to store messages
    [SerializeField]
    List<Message> messageList = new List<Message>(); // Message storage (A list to store chat messages)

    // Start is called before the first frame update
    void Start()
    {
   
        welcomeText.text = "Welcome, " + username + "!";
    }

    // Update is called once per frame
    void Update()
    {
        if (chatBox.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // Send player's message and clear input
                SendMessageToChat(username + ": " + chatBox.text, Message.MessageType.playerMessage);
                chatBox.text = "";
            }
        }
        else
        {
            if (!chatBox.isFocused && Input.GetKeyDown(KeyCode.Return))
                chatBox.ActivateInputField(); // Activate chat input (When no text is entered, pressing Enter activates the chat input field)
        }

       // if (!chatBox.isFocused)
       // {
       //     if (Input.GetKeyDown(KeyCode.Space))
       //         SendMessageToChat("Button Test", Message.MessageType.info); // Send test message to chatbox (Sending a test message when the chat is not focused)
         //   Debug.Log("Space"); // Log "Space" to the console
// }
    }

    // Send a message to the chat
    public void SendMessageToChat(string text, Message.MessageType messageType)
    {
        // Ensure message history doesn't exceed the maximum limit for the chatbox
        if (messageList.Count >= maxMessages)
        {
            // Remove the oldest message from the chatbox
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }

        // Prepare and display the new message in the chatbox
        Message newMessage = new Message();

        newMessage.text = text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);

        newMessage.textObject = newText.GetComponent<Text>();

        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = MessageTypeColor(messageType); // Set message color

        messageList.Add(newMessage);
    }

    Color MessageTypeColor(Message.MessageType messageType)
    {
        Color color = info; // Default message color

        switch (messageType)
        {
            case Message.MessageType.playerMessage:
                color = playerMessage; // Set player's message color
                break;
        }

        return color; // Return the selected color
    }
}

[System.Serializable]
public class Message
{
    public string text;
    public Text textObject;
    public MessageType messageType;

    public enum MessageType
    {
        playerMessage, // Player's message type
        info // General information message type
    }
}
