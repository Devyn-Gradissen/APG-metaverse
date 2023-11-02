using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class loginscript : MonoBehaviour
{
    // UI elementen voor gebruiker input.
    public InputField firstnameField; // Inputfield voor je voornaam.
    public InputField initialsField; // Inputfield voor de initieelen.
    public InputField lastnameField; // Inputfield voor je achternaam.
    public InputField passwordField; // Inputfield voor het wachtwoord, het veld is beschermd.
    public Button submitButton; //Button om in te loggen. Na inlog stuurt het je terug naar main menu scene.
    public Button exitButton; // Button om uit de login scene te gaan en terug te gaan naar de main menu scene zonder inloggen.
    public Text errorMessage; // Error message for when you incorrectly fill in the login credentials.

    // Trigger het loginprocess.
    public void CallLogin()
    {
        StartCoroutine(LoginPlayer());
    }

    // Coroutine voor de login.
    IEnumerator LoginPlayer()    
    {
        // Bereid ingevulde data voor om naar database/server te sturen voor controleren.
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("voornaam", firstnameField.text));
        formData.Add(new MultipartFormDataSection("initialen", initialsField.text));
        formData.Add(new MultipartFormDataSection("achternaam", lastnameField.text));
        formData.Add(new MultipartFormDataSection("password", passwordField.text));

        // Stuur een login request naar de database/server.
        UnityWebRequest www = UnityWebRequest.Post("http://localhost/apg_metaverse/login.php", formData);
        yield return www.SendWebRequest();

        // Controleer of er een succesvol resultaat is of een foutmelding.
        if (www.result == UnityWebRequest.Result.Success)
        {
            string responseText = www.downloadHandler.text;
            if (responseText[0] == '0')
            {
                // Succesvolle login, stuur gebruiker terug naar main menu scene.
                DBmanager.firstname = firstnameField.text;
                DBmanager.initials = initialsField.text;
                DBmanager.lastname = lastnameField.text;
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
            else
            {
                // foutmelding melden.
                Debug.Log("User login failed. Error #" + responseText);
                errorMessage.gameObject.SetActive(true);
                errorMessage.text = "Login mislukt: onjuiste gegevens."; // foutmelding voor mislukte inlog.
                
            }
        }
        else
        {
            // foutmelding (netwerk/database) melden.
            Debug.Log("Network error: " + www.error);
            errorMessage.gameObject.SetActive(true);
            errorMessage.text = "Login mislukt, netwerk fout opgetreden."; // foutmelding voor internetproblemen.

        }
    }

    // Zet de submit button aan als alle velden zijn ingevuld.
    public void VerifyInputs()
    {
        submitButton.interactable = (firstnameField.text.Length >= 2 && initialsField.text.Length >0 && lastnameField.text.Length >= 2 && passwordField.text.Length >= 8);
    }

    public void GoBackNow()
    {
        // bye bye
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}