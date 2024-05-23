using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class BrightnessManager : MonoBehaviour
{
    private string phpURL = "http://localhost/apg/Get_Brightness.php"; // URL to your PHP script

    void Start()
    {
        StartCoroutine(FetchHelderheid());
    }

    IEnumerator FetchHelderheid()
    {
        string username = DBmanager.username;

        // Create a form for the POST request
        WWWForm form = new WWWForm();
        form.AddField("Username", username);

        // Send the POST request to the PHP script
        using (UnityWebRequest www = UnityWebRequest.Post(phpURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Network error: " + www.error);
            }
            else
            {
                // Extract the HelderheidValue from the response
                string helderheidString = www.downloadHandler.text;
                Debug.Log("Raw HelderheidValue received: " + helderheidString);

                // Check if the response indicates no record was found
                if (helderheidString.Contains("No record found for the username"))
                {
                    // Set to default value (100)
                    Screen.brightness = 1.0f;
                    Debug.Log("No record found. Screen brightness set to default value: 1.0");
                }
                else
                {
                    // Attempt to parse the HelderheidValue to float
                    if (float.TryParse(helderheidString, out float helderheidValue))
                    {
                        // Set screen brightness to the received HelderheidValue (without normalization)
                        Screen.brightness = helderheidValue;

                        // Log the updated brightness
                        Debug.Log("Screen brightness updated: " + Screen.brightness);
                    }
                    else
                    {
                        Debug.LogError("Failed to parse HelderheidValue: " + helderheidString);
                    }
                }
            }
        }
    }
}