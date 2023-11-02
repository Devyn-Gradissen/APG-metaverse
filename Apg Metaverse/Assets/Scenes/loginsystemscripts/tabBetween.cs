using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class tabBetween : MonoBehaviour
{
    public InputField nextField;
    InputField myField;

    // roept functie op bij start, controleer of er geen volgende inputfield is of wel.
    void Start()
    {
        if (nextField == null)
        {
            Destroy(this);
            return;
        }
        myField = GetComponent<InputField>();
    }

    // Update de functie en controleert of er een volgend inputfield is en de hotkey ervoor.
    void Update()
    {
        if (myField.isFocused && Input.GetKeyDown(KeyCode.Tab)) 
        {
            nextField.ActivateInputField();
        }
    }
}