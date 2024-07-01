using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class SaveKeybindToDatabase : MonoBehaviour
{
    public Button saveButton;
    public Button resetButton;
    public TMP_InputField MoveForwardBind;
    public TMP_InputField MoveLeftBind;
    public TMP_InputField MoveBackwardsBind;
    public TMP_InputField MoveRightBind;
    public TMP_InputField PushToTalkBind;
    public TMP_InputField MuteUserVoiceBind;

    private Dictionary<string, KeyCode> keybinds = new Dictionary<string, KeyCode>();

    void Start()
    {
        saveButton.onClick.AddListener(SaveKeybinds);
        resetButton.onClick.AddListener(ResetKeybinds);
        LoadKeybinds();
        saveButton.interactable = DBmanager.LoggedIn;
        resetButton.interactable = DBmanager.LoggedIn;
    }

    // Get the keybinds
    void Update()
    {
        if (Input.GetKeyDown(keybinds["MoveForwardBind"]))
        { 
        
        }

        if (Input.GetKeyDown(keybinds["MoveLeftBind"]))
        { 
        
        }

        if (Input.GetKeyDown(keybinds["MoveBackwardsBind"]))
        { 
        
        }

        if (Input.GetKeyDown(keybinds["MoveRightBind"]))
        { 
        
        }

        if (Input.GetKeyDown(keybinds["PushToTalkBind"]))
        { 
        
        }

        if (Input.GetKeyDown(keybinds["MuteUserVoiceBind"]))
        { 
        
        }
    }

    void LoadKeybinds()
    {
        // Load keybinds from database or PlayerPrefs
        MoveForwardBind.text = PlayerPrefs.GetString("MoveForwardBind", "W");
        MoveLeftBind.text = PlayerPrefs.GetString("MoveLeftBind", "A");
        MoveBackwardsBind.text = PlayerPrefs.GetString("MoveBackwardsBind", "S");
        MoveRightBind.text = PlayerPrefs.GetString("MoveRightBind", "D");
        PushToTalkBind.text = PlayerPrefs.GetString("PushToTalkBind", "N");
        MuteUserVoiceBind.text = PlayerPrefs.GetString("MuteUserVoiceBind", "M");

        keybinds["MoveForwardBind"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), MoveForwardBind.text);
        keybinds["MoveLeftBind"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), MoveLeftBind.text);
        keybinds["MoveBackwardsBind"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), MoveBackwardsBind.text);
        keybinds["MoveRightBind"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), MoveRightBind.text);
        keybinds["PushToTalkBind"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PushToTalkBind.text);
        keybinds["MuteUserVoiceBind"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), MuteUserVoiceBind.text);
    }

    void SaveKeybinds()
    {
        if (!DBmanager.LoggedIn)
        {
            Debug.LogError("No user logged in. Keybinds cannot be saved.");
            return;
        }

        string username = DBmanager.username;

        PlayerPrefs.SetString("MoveForwardBind", MoveForwardBind.text);
        PlayerPrefs.SetString("MoveLeftBind", MoveLeftBind.text);
        PlayerPrefs.SetString("MoveBackwardsBind", MoveBackwardsBind.text);
        PlayerPrefs.SetString("MoveRightBind", MoveRightBind.text);
        PlayerPrefs.SetString("PushToTalkBind", PushToTalkBind.text);
        PlayerPrefs.SetString("MuteUserVoiceBind", MuteUserVoiceBind.text);
        PlayerPrefs.Save();

        // Save to the database
        StartCoroutine(SendToDatabase(username));
    }

    void ResetKeybinds()
    {
        if (!DBmanager.LoggedIn)
        {
            Debug.LogError("No user logged in. Keybinds cannot be reset.");
            return;
        }

        MoveForwardBind.text = "W";
        MoveLeftBind.text = "A";
        MoveBackwardsBind.text = "S";
        MoveRightBind.text = "D";
        PushToTalkBind.text = "N";
        MuteUserVoiceBind.text = "M";

        SaveKeybinds();
    }

    IEnumerator SendToDatabase(string username)
    {
        // Prepare the data to send to the database
        WWWForm form = new WWWForm();
        form.AddField("Username", username);
        form.AddField("MoveForwardBind", MoveForwardBind.text);
        form.AddField("MoveLeftBind", MoveLeftBind.text);
        form.AddField("MoveBackwardsBind", MoveBackwardsBind.text);
        form.AddField("MoveRightBind", MoveRightBind.text);
        form.AddField("PushToTalkBind", PushToTalkBind.text);
        form.AddField("MuteUserVoiceBind", MuteUserVoiceBind.text);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/apg/save_keybinds.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Keybinds saved to database successfully!");
            }
        }
    }
}