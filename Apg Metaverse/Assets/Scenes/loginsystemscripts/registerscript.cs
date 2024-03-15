using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;
using TMPro;

public class registerscript : MonoBehaviour
{
    // UI elementen voor de input en interactie.
    public TMP_InputField user_nameField;
    public TMP_InputField user_passwordField;
    public TMP_InputField user_locationField;
    public TMP_InputField user_birthdateField;
    public Button submitButton; 
    public Button exitButton; 
    public Text errorMessage;
    public Text locationerrorMessage;
    public Text successMessage;
    //public InputField usernameField;
    //public InputField firstnameField;
    //public InputField initialsField;
    //public InputField lastnameField; 
    //public InputField birthdateField; 
    //public InputField locationField; 
    //public InputField sectorField;
    //public InputField passwordField; 

    //Url voor php script voor registratie.
    private string registerURL = "http://localhost/apg/register.php";

    // Start het registratieprocess.
    public void CallRegister()
    {
        StartCoroutine(Register());
    }

    // De coroutine voor het registratieprocess
    IEnumerator Register()
    {
        // Maak een lijst van form data voor de HTTP POST request.
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>
        {
            //new MultipartFormDataSection("voornaam", firstnameField.text),
            //new MultipartFormDataSection("initialen", initialsField.text), 
            //new MultipartFormDataSection("achternaam", lastnameField.text), 
            new MultipartFormDataSection("geboortedatum", user_birthdateField.text), 
            new MultipartFormDataSection("locatie", user_locationField.text),
            //new MultipartFormDataSection("afdeling", sectorField.text),
            new MultipartFormDataSection("username", user_nameField.text),
            new MultipartFormDataSection("password", user_passwordField.text) 
        };

        // maak een HTTP POST request met UnityWebRequest.
        UnityWebRequest www = UnityWebRequest.Post(registerURL, formData);
        yield return www.SendWebRequest(); 

        // Controleer of de request succesvol is.
        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("User created successfully."); 
            successMessage.gameObject.SetActive(true); 
            successMessage.text = "Registratie is succesvol.";
            
            // Wait for 5 seconds before loading the main menu scene
            yield return new WaitForSeconds(2f);

            SceneManager.LoadScene(0); // Load the main menu scene
        }
        else
        {
            Debug.Log("User creation failed. Error #" + www.responseCode + ": " + www.error); 
            successMessage.gameObject.SetActive(false); 
            errorMessage.gameObject.SetActive(true);
            errorMessage.text = "Registratie mislukt, gebruiker is al geregistreerd."; 
        }
    }

    // verifieer de input fields om te controleren of de submit button geactiveerd mag zijn.
    public void VerifyInputs()
    {
        bool isPasswordValid = IsPasswordValid(user_passwordField.text);
        bool isLocationValid = IsLocationValid(user_locationField.text);
        bool isAgeValid = IsAgeValidFormat(user_birthdateField.text);


        // Schakel de submit button aan als alle velden zijn ingevuld.
        //submitButton.interactable = (firstnameField.text.Length >= 2 && lastnameField.text.Length >= 2 && sectorField.text.Length >= 2 && isPasswordValid && isLocationValid && isAgeValid);
        submitButton.interactable = (user_nameField.text.Length >= 5 && user_nameField.text.Length <= 30 && isPasswordValid && isLocationValid && isAgeValid);
    }

    // Check if the password meets the specified criteria.
    bool IsPasswordValid(string password)
    {
        // Check if password length is between 8 and 50 characters.
        if (password.Length < 8 || password.Length > 50)
        {
            return false;
        }

        // Check if there are at least 8 characters or 2 numbers with a total of minimum 12 characters.
        int letterCount = 0;
        int digitCount = 0;
        foreach (char c in password)
        {
            if (char.IsLetter(c))
            {
                letterCount++;
            }
            else if (char.IsDigit(c))
            {
                digitCount++;
            }
        }

        return (letterCount >= 8 || (letterCount + digitCount >= 12 && digitCount >= 2));
    }

    bool IsLocationValid(string location)
    {
        // Apg heeft 2 locaties in nederland: Amsterdam en Heerlen. Andere opties invullen weerhoudt je van registratie.
        if (string.Equals(location, "Heerlen") || string.Equals(location, "Amsterdam"))
        {
            locationerrorMessage.gameObject.SetActive(false);
            return true;
        }
        // Controleer of veld ingevuld is voor foutmelding
        else if (!string.IsNullOrEmpty(location))
        {
            locationerrorMessage.gameObject.SetActive(true);
            locationerrorMessage.text = "invoerbare Apg groeifabriek locaties zijn enkel Amsterdam & Heerlen.";
        }

        return false;
    }

    // Functie om leeftijd te verifieren.
    bool IsAgeValidFormat(string ageInput)
    {
        // Split de input met spaties om dag, maand en jaar onderdelen te detecteren.
        string[] dateParts = ageInput.Split(' ');

        // Controleer of er drie onderdelen zijn gevuld (dag, maand, jaar)
        if (dateParts.Length != 3)
        {
            return false;
        }

        // valideer de dag, maand en het jaar.
        if (!int.TryParse(dateParts[0], out int day) || !int.TryParse(dateParts[1], out int month) || !int.TryParse(dateParts[2], out int year))
        {
            return false;
        }

        // controleer of dag, maand en jaar in redelijke aantallen zijn verspreid.
        if (day < 1 || day > 31 || month < 1 || month > 12 || year < 1900 || year > DateTime.Now.Year - 18)
        {
            return false;
        }

        // controleer aantal dagen per maand
        if ((month == 4 || month == 6 || month == 9 || month == 11) && day > 30)
        {
            return false; // April, juni, september en november hebben 30 dagen
        }
            else if (month == 2)
            {
                // controleer shrikkeljaar
                if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0))
                {
                    // schrikkeljaar februari
                    if (day > 29)
                    {
                        return false;
                    }
                }
                else
                {
                    // normaal jaar februari
                    if (day > 28)
                    {
                        return false;
                    }
                }
            }
            return true;
    }

    public void GoBackNow()
    {
        // bye bye
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}