using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ChatMessageFetcher : MonoBehaviour
{
    public string chatDataFetcherURL = "http://localhost/apg/ChatDataFetch.php"; // URL of your PHP script to fetch chat messages
    public GameObject chatPanel, textObject;
    public Color playerMessage, info;
    public float fetchInterval = 5f; // Interval for fetching chat messages (in seconds)

    private List<string> fetchedMessages = new List<string>();
    private HashSet<int> displayedLogs = new HashSet<int>(); // HashSet to store logs already displayed

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

            // Clear previously fetched messages and displayed logs
            fetchedMessages.Clear();
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
        // Clear chat panel before displaying new messages
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

                    // Join all parts except the first one (log count) to reconstruct the message content
                    string sender = parts[1];
                    string messageContent = string.Join(":", parts, 2, parts.Length - 2);

                    // Instantiate new text object in chat panel for each fetched message
                    GameObject newText = Instantiate(textObject, chatPanel.transform);
                    Text newTextComponent = newText.GetComponent<Text>();
                    newTextComponent.text = sender + ": " + messageContent; // Sender name + message content
                    newTextComponent.color = info; // Assuming all fetched messages are of 'info' type
                }
            }
        }
    }
}