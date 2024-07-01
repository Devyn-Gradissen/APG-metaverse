using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class EyeColor : MonoBehaviour {
    public float redAmount;
    public float greenAmount;
    public float blueAmount;
    public float alphaAmount;

    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;

    private Color currentEyeColor;

    //Grabs the material from the skin mesh renderer and changes the color properties of the material 
    public List<SkinnedMeshRenderer> rendererlist = new List<SkinnedMeshRenderer>();

    public void UpdateSliders()
    {
        redAmount = redSlider.value;
        greenAmount = greenSlider.value;
        blueAmount = blueSlider.value;
        SetEyeColor();
    }

    public void SetEyeColor()
    {
        currentEyeColor = new Color(redAmount, greenAmount, blueAmount);

        for (int i = 0; i < rendererlist.Count; i++) 
        {
            rendererlist[i].material.SetColor("Eyes", currentEyeColor);
        }
    }
}
