using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBmanager : MonoBehaviour
{
    public static string firstname;
    public static string initials;
    public static string lastname;

    // Controleer of een gebruiker is ingelogd.
    public static bool LoggedIn { get { return firstname != null && initials != null && lastname != null; } }

    // Gebruiker uitloggen en terug sturen naar de main menu scene.
    public static void LogOut()
    {
        firstname = null;
        initials = null;
        lastname = null;
    }
}  