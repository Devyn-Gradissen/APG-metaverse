using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class signedintoapg : MonoBehaviour
{
    public Text playerDisplay;

    private void Awake()
    {
        // Check if a user is logged in; if not, return to the main menu.
        if (DBmanager.firstname == null && DBmanager.initials == null && DBmanager.lastname == null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        // Display the player's name and score.
        playerDisplay.text = DBmanager.firstname + " " + DBmanager.initials + " " + DBmanager.lastname;
    }

    // Trigger the data save process.
    public void CallSaveData()
    {
        StartCoroutine(SavePlayerData());
    }

    // Coroutine to save player data to a remote server. 
    IEnumerator SavePlayerData()
    {
        // Prepare the data to be sent.
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("voornaam", DBmanager.firstname));
        formData.Add(new MultipartFormDataSection("initialen", DBmanager.initials));
        formData.Add(new MultipartFormDataSection("achternaam", DBmanager.lastname));

        // Send the data to the server.
        UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/savedata.php", formData);
        yield return www.SendWebRequest();

        // Check for a successful response or network error.
        if (www.result == UnityWebRequest.Result.Success)
        {
            string responseText = www.downloadHandler.text;
            if (responseText == "0")
            {
                Debug.Log("Changes Saved");
            }
            else
            {
                Debug.Log("Save failed. Error #" + responseText);
            }
        }
        else
        {
            Debug.Log("Network error: " + www.error);
        }

        // Log out the user and return to the main menu.
        DBmanager.LogOut();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}