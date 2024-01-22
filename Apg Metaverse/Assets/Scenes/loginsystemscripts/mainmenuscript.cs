using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class mainmenuscript : MonoBehaviour
{
    // Variabels voor de UI in unity.
    public Button registerButton;
    public Button loginButton;
    public Button playButton; 
    public Button exitButton; 
    public Text playerDisplay; 

    // Start functie roept meteen op
    private void Start()
    {
        // Controleer of gebruiker is ingelogd.
        if (DBmanager.LoggedIn)
        {
            playerDisplay.text = " Ingelogd als: " + DBmanager.username + "\n Welkom bij de APG virtuele omgeving.";
            //playerDisplay.text = " Ingelogd als: " + DBmanager.firstname + " " + DBmanager.initials + " " + DBmanager.lastname + "\n Welkom bij de APG virtuele omgeving.";
        }

        registerButton.interactable = !DBmanager.LoggedIn;
        loginButton.interactable = !DBmanager.LoggedIn;
        playButton.interactable = DBmanager.LoggedIn;
    }

    // Sturen naar registratie.
    public void GoToRegister()
    {
        SceneManager.LoadScene(1); 
    }

    // Sturen naar de login.
    public void GoToLogin()
    {
        SceneManager.LoadScene(2); 
    }

    // Sturen naar de virtuele omgeving.
    public void GoToAPG()
    {
        SceneManager.LoadScene(3); 
    }

    // Method to exit the application
    public void ExitApp()
    {
        Debug.Log("Gebruiker has exit app");
        Application.Quit();
    }
}