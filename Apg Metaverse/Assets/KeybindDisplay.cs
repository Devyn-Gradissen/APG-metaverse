using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class KeybindDisplay : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(FetchKeybinds());
    }

    IEnumerator FetchKeybinds()
    {
        // Modify the URL to match your PHP script
        string url = "http://localhost/apg/Get_Keybinds.php";

        // Get the username from DBManager
        string username = DBmanager.username;

        // Create a form with the username
        WWWForm form = new WWWForm();
        form.AddField("Username", username);

        // Send the request
        using (WWW www = new WWW(url, form))
        {
            yield return www;

            if (www.error != null)
            {
                Debug.Log("Error fetching keybinds: " + www.error);
            }
            else 
            {
                // Parse the response
                string[] keybinds = www.text.Split('\n');

                if (keybinds.Length == 6) // Ensure all keybinds are present
                {
                    // Print each keybind value separately to the debug log
                    Debug.Log("Move Forward Bind: " + keybinds[0]);
                    Debug.Log("Move Left Bind: " + keybinds[1]);
                    Debug.Log("Move Backward Bind: " + keybinds[2]);
                    Debug.Log("Move Right Bind: " + keybinds[3]);
                    Debug.Log("Push To Talk Bind: " + keybinds[4]);
                    Debug.Log("Mute User Voice Bind: " + keybinds[5]);
                }
                else
                {
                    Debug.Log("Error: Incomplete keybind data received");
                }
            }
        }
    }
}