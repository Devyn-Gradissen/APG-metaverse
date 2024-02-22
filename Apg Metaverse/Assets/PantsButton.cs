using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PantsButton : MonoBehaviour
{
    public SelectPantsColor selectPantsColor;

    [Header("Color Values")]
    public float redAmount;
    public float greenAmount;
    public float blueAmount;

    [Header("Color Sliders")]
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;

    public Image colorImage;

    private void Awake()
    {
        colorImage = GetComponent<Image>();
        redAmount = colorImage.color.r;
        greenAmount = colorImage.color.g;
        blueAmount = colorImage.color.b;
    }

    public void SetSliderValues()
    {
        redSlider.value = redAmount;
        greenSlider.value = greenAmount;
        blueSlider.value = blueAmount;

        selectPantsColor.redAmount = redAmount;
        selectPantsColor.greenAmount = greenAmount;
        selectPantsColor.blueAmount = blueAmount;
        selectPantsColor.SetPantsColor();
    }
}
