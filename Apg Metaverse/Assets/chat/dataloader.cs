using UnityEngine;
using System.Collections;

public class dataloader : MonoBehaviour
{
    public string[] chat;

    IEnumerator Start()
    {
        WWW ChatData = new WWW("http://localhost/school%20leerjaar%203/chatdata/ChatData.php"); //connectie met de data
        yield return ChatData; // wacht totdat data is gedownload
        
        string ChatDataString = ChatData.text; //maak de data klaar om te zien
        print(ChatDataString); // laat de data zien
        
        chat = ChatDataString.Split(';');
        print(GetDataValue(chat[0], "chat")); // Replace "your_index_here" with the actual index you want to retrieve
    }

    string GetDataValue(string data, string index)
    {
        string value = data.Substring(data.IndexOf(index));
        return value;
    }
}
