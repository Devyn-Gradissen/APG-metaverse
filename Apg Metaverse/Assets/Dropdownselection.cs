using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dropdownselection : MonoBehaviour
{
    public TextMeshProUGUI output;

    public void HandleInputData(int val)
    {
        if (val == 0)
        {
            output.text = "UIT";
        }
        if (val == 1)
        {
            output.text = "Trichromatopsie";
        }
        if (val == 2)
        {
            output.text = "Dichromatopsie";
        }
        if (val == 3)
        {
            output.text = "Monochromatopsie";
        }
        if (val == 4)
        {
            output.text = "Achromatopsie";
        }
    }
}
