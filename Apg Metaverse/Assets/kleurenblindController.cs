using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class kleurenblindController : MonoBehaviour
{

    public Slider kleuren;



    public float kleurenValue;

  
    public void Start()
    {
        kleuren.value = PlayerPrefs.GetFloat("save", kleurenValue);

    }

    public void ChangeSlider(float value)
    {
        kleurenValue = value;
        PlayerPrefs.SetFloat("save", kleurenValue);
      
    }
}
