using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    ArrayList credentials;  

    public string username;
    public int maxMessages = 100;
    public GameObject chatPanel, textObject;
    public InputField chatBox;
    public Color playerMessage, info;

    [SerializeField]
    List<Message> messageList = new List<Message>();

    public PlayerController playerController; // Reference to the PlayerController script

    void Start()
    {
        // Call both Start functions from each script
        Start1();
        Start2();
    }

    void Update()
    {
        // Call both Update functions from each script
        Update1();
        Update2();
    }

    // First Start function
    void Start1()
    {
        playerController.SetGameManager(this); // Set reference to GameManager in PlayerController
    }

    // Second Start function
    void Start2()
    {
        // Welcome text code or any other start-related code from the second script can be placed here
        // Example: welcomeText.text = "Welcome, " + username + "!";
    }

    // First Update function
    void Update1()
    {
        if (chatBox.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat(username + ": " + chatBox.text, Message.MessageType.playerMessage);
                chatBox.text = "";

                // Toggle player movement
                playerController.ToggleMovement(false);
            }
        }
        else
        {
            if (!chatBox.isFocused && Input.GetKeyDown(KeyCode.Return))
                chatBox.ActivateInputField(); // Activate chat input

            // Ensure movement is enabled when chat is not focused
            playerController.ToggleMovement(true);
        }
    }

    // Second Update function
    void Update2()
    {
        // Add any additional update functionality from the second script here
    }

    // Send a message to the chat
    public void SendMessageToChat(string text, Message.MessageType messageType)
    {
        if (messageList.Count >= maxMessages)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }

        Message newMessage = new Message();

        newMessage.text = text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);

        newMessage.textObject = newText.GetComponent<Text>();

        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = MessageTypeColor(messageType);

        messageList.Add(newMessage);
    }

    Color MessageTypeColor(Message.MessageType messageType)
    {
        Color color = info;

        switch (messageType)
        {
            case Message.MessageType.playerMessage:
                color = playerMessage;
                break;
        }

        return color;
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
        playerMessage,
        info
    }
}