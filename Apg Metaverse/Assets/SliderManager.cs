using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    public Slider slider;

    public void SaveSliderValue()
    {
        StartCoroutine(SendSliderValue());
    }

    IEnumerator SendSliderValue()
    {
        // Replace 'your_php_script_url' with the URL to your PHP script
        string url = "http://localhost/school%20leerjaar%203/slider/save_slider.php";
        WWWForm form = new WWWForm();
        form.AddField("sliderValue", (int)slider.value);

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

