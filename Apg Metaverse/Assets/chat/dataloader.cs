using UnityEngine;
using System.Collections;
using UnityEngine.Networking; // Import UnityWebRequest
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class dataloader : MonoBehaviour
{
    public GameManager gameManager; // Reference to the GameManager script
    public string[] chat;

    IEnumerator Start()
    {
        UnityWebRequest ChatDataRequest = UnityWebRequest.Get("http://localhost/apg/chatdata.php"); // Connect to the data source
        yield return ChatDataRequest.SendWebRequest(); // Wait until data is downloaded

        if (ChatDataRequest.result == UnityWebRequest.Result.Success)
        {
            string ChatDataString = ChatDataRequest.downloadHandler.text; // Get the downloaded data
            Debug.Log(ChatDataString); // Log the data

            chat = ChatDataString.Split(';'); // Split the data into individual chat messages

            // Iterate through the chat messages and pass them to the GameManager
            foreach (string messageData in chat)
            {
                string messageText = GetDataValue(messageData, "chat"); // Extract the chat message
                gameManager.SendMessageToChat(messageText, Message.MessageType.info); // Pass the message to GameManager
            }
        }
        else
        {
            Debug.LogError("Error downloading chat data: " + ChatDataRequest.error);
        }
    }

    string GetDataValue(string data, string index)
    {
        string value = data.Substring(data.IndexOf(index));
        return value;
    }
}