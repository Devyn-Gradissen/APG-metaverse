using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class helderheidController : MonoBehaviour
{

    public Slider helderheid;



    public float helderheidValue;

  
    public void Start()
    {
        helderheid.value = PlayerPrefs.GetFloat("helderheid", helderheidValue);

    }

    public void ChangeSlider(float value)
    {
        helderheidValue = value;
        PlayerPrefs.SetFloat("helderheid", helderheidValue);
      
    }
}
