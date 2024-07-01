using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SelectHairColor : MonoBehaviour
{
    [Header("Color Values")]
    public float redAmount;
    public float greenAmount;
    public float blueAmount;

    [Header("Color Sliders")]
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;

    private Color currentHairColor;

    //Grabs the material from the skin mesh renderer and changes the color properties of the material 
    public List<SkinnedMeshRenderer> rendererlist = new List<SkinnedMeshRenderer>();
    public void UpdateSliders()
    {
        redAmount = redSlider.value;
        greenAmount = greenSlider.value;
        blueAmount = blueSlider.value;
        SetHairColor();
    }
    public void SetHairColor()
    {
        currentHairColor = new Color(redAmount, greenAmount, blueAmount);

        for (int i = 0; i < rendererlist.Count; i++)
        {
            rendererlist[i].material.SetColor("_BaseColor", currentHairColor);
        }
    }
}
