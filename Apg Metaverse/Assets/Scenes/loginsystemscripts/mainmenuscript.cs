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
    public Button customize_your_character;
    public Button tutorial;
    public Button instellingen;
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
        customize_your_character.interactable = DBmanager.LoggedIn;
        tutorial.interactable = DBmanager.LoggedIn;
        instellingen.interactable = DBmanager.LoggedIn;
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

    public void GoToCustomize()
    {
        SceneManager.LoadScene(4); 
    }

    public void GoToTutorial()
    {
        SceneManager.LoadScene(5); 
    }
    public void GoToInstellingen()
    {
        SceneManager.LoadScene(6); 
    }

    // Method to exit the application
    public void ExitApp()
    {
        Debug.Log("Gebruiker has exit app");
        Application.Quit();
    }
}