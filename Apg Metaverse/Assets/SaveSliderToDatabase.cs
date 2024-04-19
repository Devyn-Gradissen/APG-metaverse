using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class SaveSliderToDatabase : MonoBehaviour
{
    public Button saveButton;
    public Button resetButton;
    public Slider Helderheid;
    public Slider Volume;
    public Toggle PushToTalkToggle;

    private const string phpScriptURL = "http://localhost/apg/save_slider.php"; // URL PHP script 

    public TextMeshProUGUI output;

    private void Start()
    {
        saveButton.interactable = DBmanager.LoggedIn;
        resetButton.interactable = false; // Initially disable the reset button

        // Listener op buttons zetten als events
        saveButton.onClick.AddListener(SaveDataToDatabase);
        resetButton.onClick.AddListener(ResetDataToDatabase);

        // Check if the user has settings in the database
        StartCoroutine(CheckUserSettings(DBmanager.username));

        // Start periodic check for user settings
        StartCoroutine(CheckUserSettingsPeriodically(DBmanager.username));
    }

    // Coroutine to periodically check if the user has settings in the database
    IEnumerator CheckUserSettingsPeriodically(string username)
    {
        while (true)
        {
            yield return new WaitForSeconds(10); // Check every 10 seconds
            yield return CheckUserSettings(username);
        }
    }

    // data instellingen opslaan
    public void SaveDataToDatabase()
    {
        // Check of de gebruiker bestaat
        string username = DBmanager.username;
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("No username found. Settings cannot be saved.");
            return;
        }

        // Haal de gekozen dropdown optie tekst uit de output field
        string kleurenblindValue = output.text;

        StartCoroutine(SendDataToDatabase(username, kleurenblindValue));

        // Check settings status after saving
        StartCoroutine(CheckUserSettings(username));
    }

    // Coroutine to check if the user has settings in the database
    IEnumerator CheckUserSettings(string username)
    {
        // PHP script URL for checking user settings
        string checkSettingsURL = "http://localhost/apg/check_settings.php";

        // Form data with username
        WWWForm form = new WWWForm();
        form.AddField("Username", username);

        using (UnityWebRequest www = UnityWebRequest.Post(checkSettingsURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to check user settings: " + www.error);
            }
            else
            {
                // Check the response from the server
                string response = www.downloadHandler.text;
                if (response == "1")
                {
                    // User has settings, enable the reset button
                    resetButton.interactable = true;
                }
                else
                {
                    // User does not have settings, disable the reset button
                    resetButton.interactable = false;
                }
            }
        }
    }

    //data naar database sturen
    IEnumerator SendDataToDatabase(string username, string kleurenblindValue)
    {
        Debug.Log("Helderheid Value: " + Helderheid.value.ToString());
        Debug.Log("Volume Value: " + Volume.value.ToString());
        Debug.Log("Kleurenblind Value: " + kleurenblindValue); // Gebruik gekozen dropdown optie text

        // Determine PushToTalk value based on toggle state
        string pushToTalkValue = PushToTalkToggle.isOn ? "0" : "1";

        Debug.Log("PushToTalk Value: " + pushToTalkValue);

        // Voeg gebruikersnaam en andere opties om data te vormen
        WWWForm form = new WWWForm();
        form.AddField("Username", username);
        form.AddField("HelderheidValue", Helderheid.value.ToString());
        form.AddField("VolumeValue", Volume.value.ToString());
        form.AddField("KleurenblindValue", kleurenblindValue); // Gebruik geselecteerde dropdown text
        form.AddField("PushToTalkValue", pushToTalkValue); // Voeg PushToTalk value toe

        using (UnityWebRequest www = UnityWebRequest.Post(phpScriptURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to send data to database: " + www.error);
            }
            else
            {
                Debug.Log("Data saved successfully");
            }
        }
    }

    // data resetten
    public void ResetDataToDatabase()
    {
        // Check of gebruiker bestaat
        string username = DBmanager.username;
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("No username found. Settings cannot be Reset.");
            return;
        }

        StartCoroutine(SendResetRequestToDatabase(username));
        Debug.Log("Requesting a reset of settings...");
    }

    //reset request naar database sturen  
    IEnumerator SendResetRequestToDatabase(string username)
    {
        string resetScriptURL = "http://localhost/apg/reset_settings.php"; // Php URL voor reset van instellingen

        // Sorteer met de username
        WWWForm form = new WWWForm();
        form.AddField("Username", username);

        using (UnityWebRequest www = UnityWebRequest.Post(resetScriptURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to reset settings: " + www.error);
            }
            else
            {
                Debug.Log("Settings reset successfully");
            }
        }
    }

    // Handelt dropdown selectie veranderingen
    public void HandleInputData(int val)
    {
        if (val == 0)
        {
            output.text = "UIT";
        }
        else if (val == 1)
        {
            output.text = "Trichromatopsie";
        }
        else if (val == 2)
        {
            output.text = "Dichromatopsie";
        }
        else if (val == 3)
        {
            output.text = "Monochromatopsie";
        }
        else if (val == 4)
        {
            output.text = "Achromatopsie";
        }
    }
}
