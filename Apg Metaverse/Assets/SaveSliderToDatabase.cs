using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SaveSliderValue : MonoBehaviour 
{ 
    // alle sliders en de link naar de script naar de database benoemen en koppelen in unity
    public Slider Helderheid;
    public Slider Volume;
    public Slider Kleurenblind;
    public string phpScriptURL;

    public void SaveSliderToDatabase() //zorgen dat het script gestart wordt
    {
        StartCoroutine(SendSliderValue());
    }

    IEnumerator SendSliderValue()
    {
        Debug.Log("HelderheidValue: " + Helderheid.value.ToString());
        Debug.Log("VolumeValue: " + Volume.value.ToString());
        Debug.Log("KleurenblindValue: " + Kleurenblind.value.ToString());

        // maak het form, zodat het naar de database gestuurt kan worden
        WWWForm form = new WWWForm();
        form.AddField("HelderheidValue", Helderheid.value.ToString());
        form.AddField("VolumeValue", Volume.value.ToString());
        form.AddField("KleurenblindValue", Kleurenblind.value.ToString());
      

        // stuur data naar php script gelinkt bovenaan
        using (UnityWebRequest www = UnityWebRequest.Post(phpScriptURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to send slider value to database: " + www.error); // als er fouten zijn worden die getoont
            }
            else
            {
                Debug.Log("Slider value saved successfully"); // als de slider het goed heeft opgeslagen dan komt dit in unity te staan
            }
        }
    }
}
