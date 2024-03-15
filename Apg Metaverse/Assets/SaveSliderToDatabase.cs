using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SaveSliderValue : MonoBehaviour
{
    public Slider slider;
    public string phpScriptURL;

    public void SaveSliderToDatabase()
    {
        StartCoroutine(SendSliderValue());
    }

    IEnumerator SendSliderValue()
    {
        // Create form
        WWWForm form = new WWWForm();
        form.AddField("sliderValue", slider.value.ToString());

        // Send data to PHP script
        using (UnityWebRequest www = UnityWebRequest.Post(phpScriptURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to send slider value to database: " + www.error);
            }
            else
            {
                Debug.Log("Slider value saved successfully");
            }
        }
    }
}
