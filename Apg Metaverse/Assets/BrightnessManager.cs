using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class BrightnessManager : MonoBehaviour
{
    private string phpURL = "http://localhost/apg/Get_Brightness.php"; // URL van PHP script

    void Start()
    {
        StartCoroutine(FetchHelderheid()); 
    }

    IEnumerator FetchHelderheid()
    {
        string username = DBmanager.username;

        // maak een form voor de POST request
        WWWForm form = new WWWForm();
        form.AddField("Username", username);

        // stuur de POST request naar het PHP script
        using (UnityWebRequest www = UnityWebRequest.Post(phpURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Network error: " + www.error);
            }
            else
            {
                // Haal de heldlerheid value op van de respons
                string helderheidString = www.downloadHandler.text;
                Debug.Log("Raw HelderheidValue received: " + helderheidString);

                // Check of de respons aangeeft of er geen gegevens zijn gevonden
                if (helderheidString.Contains("No record found for the username"))
                {
                    Screen.brightness = 1.0f;
                    Debug.Log("No record found. Screen brightness set to default value: 1.0");
                }
                else
                {
                    // probeer de helderheidvalue te parseren naar een float
                    if (float.TryParse(helderheidString, out float helderheidValue))
                    {
                        Screen.brightness = helderheidValue;

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