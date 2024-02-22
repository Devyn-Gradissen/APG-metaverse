using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SelectSkinColor : MonoBehaviour
{
    [Header("Color Values")]
    public float redAmount;
    public float greenAmount;
    public float blueAmount;

    [Header("Color Slider")]
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;

    private Color currentSkinColor;

    //Grabs the material from the skin mesh renderer and changes the color properties of the material 
    public List<SkinnedMeshRenderer> rendererList = new List<SkinnedMeshRenderer>();

    public void UpdateSliders()
    {
        redAmount = redSlider.value;
        greenAmount = greenSlider.value;
        blueAmount = blueSlider.value;
        SetSkinColor();
    }

    public void SetSkinColor()
    {
        currentSkinColor = new Color(redAmount, greenAmount, blueAmount);

        for (int i = 0; i < rendererList.Count; i++)
        {
            rendererList[i].material.SetColor("_BaseColor", currentSkinColor);
        }
    }
}
