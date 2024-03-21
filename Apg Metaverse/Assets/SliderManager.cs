using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    public Slider Helderheid;
    public Slider Volume;
    public Slider Kleurenblind;

    public void SaveSliderValue()
    {
        StartCoroutine(SendSliderValue());
    }

    IEnumerator SendSliderValue()
    {
        // Replace 'your_php_script_url' with the URL to your PHP script
        string url = "http://localhost/school%20leerjaar%203/slider/save_slider.php";
        WWWForm form = new WWWForm();
        form.AddField("HelderheidValue", (int)Helderheid.value);
        form.AddField("VolumeValue", (int)Volume.value);
        form.AddField("KleurenblindValue", (int)Kleurenblind.value);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Slider value saved successfully");
            }
        }
    }
}

