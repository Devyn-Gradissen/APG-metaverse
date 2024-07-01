using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAvatarValues : MonoBehaviour
{
    // Default values for character attributes
    public string defaultGender = "male";
    public string defaultHair = "0:0:0"; // black
    public string defaultPants = "255:255:255"; // white
    public string defaultEyes = "0:0:0"; // black
    public string defaultShirt = "255:255:255"; // White
    public string defaultSkin = "255:224:189"; // Light skin tone
    public string defaultUsername = "defaultUser"; //temporary, will de DBmanager.username regardless

    // used for calling upon in SaveCharacterData
    public string GetDefaultGender() => defaultGender;
    public string GetDefaultHair() => defaultHair;
    public string GetDefaultPants() => defaultPants;
    public string GetDefaultEyes() => defaultEyes;
    public string GetDefaultShirt() => defaultShirt;
    public string GetDefaultSkin() => defaultSkin;
    public string GetDefaultUsername() => defaultUsername; 
}