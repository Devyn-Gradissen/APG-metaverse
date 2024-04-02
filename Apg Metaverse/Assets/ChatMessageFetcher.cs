using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ChatMessageFetcher : MonoBehaviour
{
    public string chatDataFetcherURL = "http://localhost/apg/ChatData.php"; // php link
    public GameObject chatPanel, textObject;
    public Color playerMessage, info;
    public float fetchInterval = 1f; // tijd om chatberichten te ophalen

    private List<string> fetchedMessages = new List<string>();
    private HashSet<int> displayedLogs = new HashSet<int>(); // HashSet om logs te onthouden die al getoond zijn.

    void Start()
    {
        StartCoroutine(FetchChatMessagesPeriodically());
    }

    IEnumerator FetchChatMessagesPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(fetchInterval);
            StartCoroutine(FetchChatMessages());
        }
    }

    IEnumerator FetchChatMessages()
    {
        UnityWebRequest www = UnityWebRequest.Get(chatDataFetcherURL);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch chat messages: " + www.error);
        }
        else
        {
            string[] messages = www.downloadHandler.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            fetchedMessages.Clear(); //wis oude chat en refresh na nieuwe logs
            displayedLogs.Clear();

            foreach (string message in messages)
            {
                fetchedMessages.Add(message);
            }

            DisplayFetchedMessages();
        }
    }

    void DisplayFetchedMessages()
    {
        foreach (Transform child in chatPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (string message in fetchedMessages)
        {
            // Parse the message to extract the log count, sender, and message content
            string[] parts = message.Split(':');
            if (parts.Length >= 3)
            {
                int logCount;
                if (int.TryParse(parts[0], out logCount))
                {
                    displayedLogs.Add(logCount);

                    string sender = parts[1]; //alle onderdelen samenvoegen behalven de log die dient als enkel een id in functie
                    string messageContent = string.Join(":", parts, 2, parts.Length - 2);

                    // new text object in chatpanel voor elk nieuw bericht
                    GameObject newText = Instantiate(textObject, chatPanel.transform);
                    Text newTextComponent = newText.GetComponent<Text>();
                    newTextComponent.text = sender + ": " + messageContent; 
                    newTextComponent.color = info; 
                }
            }
        }
    }
}