using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    private const string chatdatauploaderURL = "http://localhost/apg/ChatDataUpload.php"; // URL of your PHP script
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
                SendMessageToChat(DBmanager.username + ": " + chatBox.text, Message.MessageType.playerMessage);
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

        // Prepare message for display in chat
        Message newMessage = new Message();
        newMessage.text = text;

        // Instantiate new text object in chat panel
        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newMessage.textObject = newText.GetComponent<Text>();
        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = MessageTypeColor(messageType);

        // Add message to messageList
        messageList.Add(newMessage);

        // Upload message to PHP
        StartCoroutine(UploadMessageToPHP(newMessage.text)); // Pass newMessage.text to the coroutine
    }

    // Upload message to PHP script
    IEnumerator UploadMessageToPHP(string message)
    {
        // Prepare form data
        WWWForm form = new WWWForm();
        form.AddField("sender", DBmanager.username); // Add sender username
        form.AddField("message", message); // Add message text

        // Send POST request to PHP script
        using (UnityWebRequest www = UnityWebRequest.Post(chatdatauploaderURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to upload message: " + www.error);
            }
            else
            {
                Debug.Log("Message uploaded successfully");
            }
        }
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