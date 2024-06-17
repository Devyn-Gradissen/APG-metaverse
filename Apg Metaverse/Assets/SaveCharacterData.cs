using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;
using TMPro;

public class SaveCharacterData : MonoBehaviour
{
    // URL for PHP script to save/update avatar.
    private string characterURL = "http://localhost/apg/AvatarUploader.php";

    //no gender script yet, and username is DBmanager.username.
    public SelectHairColor HairColorScript;
    public SelectPantsColor PantsColorScript;
    public SelectEyeColor EyeColorScript;
    public SelectShirtColor ShirtColorScript;
    public SelectSkinColor SkinColorScript;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AssignAvatarValues());
    }

    // Coroutine to assign avatar values
    private IEnumerator AssignAvatarValues()
    {
        yield return new WaitForEndOfFrame(); // Wait for all scripts to be initialized if needed

        string gender_avatar = "male";
        string hair_avatar = $"{HairColorScript.redAmount}|{HairColorScript.blueAmount}|{HairColorScript.greenAmount}";
        string pants_avatar = $"{PantsColorScript.redAmount}|{PantsColorScript.blueAmount}|{PantsColorScript.greenAmount}";
        string eyes_avatar = $"{EyeColorScript.redAmount}|{EyeColorScript.blueAmount}|{EyeColorScript.greenAmount}";
        string shirt_avatar = $"{ShirtColorScript.redAmount}|{ShirtColorScript.blueAmount}|{ShirtColorScript.greenAmount}";
        string skin_avatar = $"{SkinColorScript.redAmount}|{SkinColorScript.blueAmount}|{SkinColorScript.greenAmount}";
        string username_avatar = DBmanager.username;

        Debug.Log($"Gender: {gender_avatar}, Hair: {hair_avatar}, Pants: {pants_avatar}, Eyes: {eyes_avatar}, Shirt: {shirt_avatar}, Skin: {skin_avatar}");
    }

    // Public method to save avatar data to the server
    public void SaveAvatarToServer()
    {
        string gender_avatar = "male"; // You may adjust this based on your requirements
        string hair_avatar = $"{HairColorScript.redAmount}|{HairColorScript.blueAmount}|{HairColorScript.greenAmount}";
        string pants_avatar = $"{PantsColorScript.redAmount}|{PantsColorScript.blueAmount}|{PantsColorScript.greenAmount}";
        string eyes_avatar = $"{EyeColorScript.redAmount}|{EyeColorScript.blueAmount}|{EyeColorScript.greenAmount}";
        string shirt_avatar = $"{ShirtColorScript.redAmount}|{ShirtColorScript.blueAmount}|{ShirtColorScript.greenAmount}";
        string skin_avatar = $"{SkinColorScript.redAmount}|{SkinColorScript.blueAmount}|{SkinColorScript.greenAmount}";
        string username_avatar = DBmanager.username;

        StartCoroutine(SaveAvatarData(gender_avatar, hair_avatar, pants_avatar, eyes_avatar, shirt_avatar, skin_avatar, username_avatar));
    }

    // Coroutine to send avatar data to the server
    private IEnumerator SaveAvatarData(string gender_avatar, string hair_avatar, string pants_avatar, string eyes_avatar, string shirt_avatar, string skin_avatar, string username_avatar)
    {
        Debug.Log($"gender_avatar: {gender_avatar}, hair_avatar: {hair_avatar}, pants_avatar: {pants_avatar}, eyes_avatar: {eyes_avatar}, shirt_avatar: {shirt_avatar}, skin_avatar: {skin_avatar}, username_avatar: {username_avatar}");
        WWWForm form = new WWWForm();
        form.AddField("gender_avatar", gender_avatar);
        form.AddField("hair_avatar", hair_avatar);
        form.AddField("pants_avatar", pants_avatar);
        form.AddField("eyes_avatar", eyes_avatar);
        form.AddField("shirt_avatar", shirt_avatar);
        form.AddField("skin_avatar", skin_avatar);
        form.AddField("username_avatar", username_avatar);

        using (UnityWebRequest www = UnityWebRequest.Post(characterURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                Debug.Log("Response: " + www.downloadHandler.text);
            }
        }
    }
}