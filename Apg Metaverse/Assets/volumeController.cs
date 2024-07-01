using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class volumeController : MonoBehaviour
{

    public Slider volume;



    public float volumeValue;

  
    public void Start()
    {
        volume.value = PlayerPrefs.GetFloat("volume", volumeValue);

    }

    public void ChangeSlider(float value)
    {
        volumeValue = value;
        PlayerPrefs.SetFloat("volume", volumeValue);
      
    }
}
