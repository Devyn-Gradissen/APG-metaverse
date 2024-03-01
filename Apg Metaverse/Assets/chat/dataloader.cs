using UnityEngine;
using System.Collections;
using UnityEngine.Networking; // Import UnityWebRequest
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class dataloader : MonoBehaviour
{
    public string[] chat;

    IEnumerator Start()
    {
        UnityWebRequest ChatDataRequest = UnityWebRequest.Get("http://localhost/apg/chatdata.php"); //connectie met de data
        yield return ChatDataRequest.SendWebRequest(); // wacht totdat data is gedownload
        
        if (ChatDataRequest.result == UnityWebRequest.Result.Success)
        {
            string ChatDataString = ChatDataRequest.downloadHandler.text; //maak de data klaar om te zien
            Debug.Log(ChatDataString); // laat de data zien
            
            chat = ChatDataString.Split(';');
            Debug.Log(GetDataValue(chat[0], "chat")); // Replace "your_index_here" with the actual index you want to retrieve
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
