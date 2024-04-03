using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SaveSliderValue : MonoBehaviour
{
    public Slider Helderheid;
    public Slider Volume;
    public Dropdown Kleurenblind;
    public Dropdown PushToTalk;
    public string phpScriptURL;

    // Enum to represent options for PushToTalk dropdown
    public enum PushToTalkOption
    {
        On,
        Off
    }

    // Enum to represent options for Kleurenblind dropdown
    public enum KleurenblindOption
    {
        Option1,
        Option2,
        Option3,
        Option4,
        Option5
    }

    public void SaveDataToDatabase()
    {
        StartCoroutine(SendDataToDatabase());
    }

    IEnumerator SendDataToDatabase()
    {
        Debug.Log("Helderheid Value: " + Helderheid.value.ToString());
        Debug.Log("Volume Value: " + Volume.value.ToString());
        Debug.Log("Kleurenblind Value: " + ((KleurenblindOption)Kleurenblind.value).ToString());
        Debug.Log("PushToTalk Value: " + ((PushToTalkOption)PushToTalk.value).ToString());

        WWWForm form = new WWWForm();
        form.AddField("HelderheidValue", Helderheid.value.ToString());
        form.AddField("VolumeValue", Volume.value.ToString());
        form.AddField("KleurenblindValue", ((KleurenblindOption)Kleurenblind.value).ToString());
        form.AddField("PushToTalkValue", ((PushToTalkOption)PushToTalk.value).ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(phpScriptURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to send data to database: " + www.error);
            }
            else
            {
                Debug.Log("Data saved successfully");
            }
        }
    }
}
