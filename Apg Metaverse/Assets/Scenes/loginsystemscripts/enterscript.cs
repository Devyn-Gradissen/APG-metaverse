using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class enterscript : MonoBehaviour
{
    public Text playerDisplay;
    private void Awake()
    {
        // Controleer of gebruiker is ingelogd, zo niet stuur terug naar main menu scene.
        //if (DBmanager.firstname == null && DBmanager.lastname == null)
        if (DBmanager.username == null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        // toon username ingelogde gebruiker aan 
        playerDisplay.text = DBmanager.username;
        // Toon gebruikers voornaam & initielen & achternaam aan.
        //playerDisplay.text = DBmanager.firstname + " " + DBmanager.initials + " " + DBmanager.lastname;   

    }

    // Trigger the data save process.
    public void CallSaveData()   
    {
        StartCoroutine(SavePlayerData());
    }

    // Coroutine om spelerwijzigingen op te slaan op de server.
    IEnumerator SavePlayerData()
    {
        // Bereid data voor om te verzenden.
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("username", DBmanager.username));
        //formData.Add(new MultipartFormDataSection("voornaam", DBmanager.firstname));
        //formData.Add(new MultipartFormDataSection("initialen", DBmanager.initials));
        //formData.Add(new MultipartFormDataSection("achternaam", DBmanager.lastname));

        // Stuur data naar de database/server.
        UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/savedata.php", formData);
        yield return www.SendWebRequest();

        // Controleer of er een succesvolle reactie is van de database/server of een netwerk fout is opgetreden.
        if (www.result == UnityWebRequest.Result.Success)
        {
            string responseText = www.downloadHandler.text;
            if (responseText == "0")
            {
                Debug.Log("Result Saved");
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

        // Log de gebruiker uit en stuur ze terug naar de main menu scene. 
        DBmanager.LogOut();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}