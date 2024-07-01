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
        UnityWebRequest ChatDataRequest = UnityWebRequest.Get("http://localhost/apg/chatdata.php"); // Connectie met data chatlog
        yield return ChatDataRequest.SendWebRequest(); // Wacht tot de data is gedownload

        if (ChatDataRequest.result == UnityWebRequest.Result.Success)
        {
            string ChatDataString = ChatDataRequest.downloadHandler.text; // haal gedownloade data
            Debug.Log(ChatDataString); // Log de data

            chat = ChatDataString.Split(';'); // Split de data naar individuele chatberichten

            // iteratie per chatbericht en stuur ze naar gamemanager
            foreach (string messageData in chat)
            {
                string messageText = GetDataValue(messageData, "chat"); // haal chatbericht op
                gameManager.SendMessageToChat(messageText, Message.MessageType.info); // stuur bericht naar de GameManager
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