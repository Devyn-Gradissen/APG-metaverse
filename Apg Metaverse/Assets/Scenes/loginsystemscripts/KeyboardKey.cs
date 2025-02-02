using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyboardKey : MonoBehaviour
{
    public string character;
    public string shiftCharacter;

    public TextMeshProUGUI keyLabel;

    private Button thisKey;

    private bool isShifted = false;

    private void Start()
    {
        KeyboardManager.instance.shiftButton.onClick.AddListener(HandleShift);
        thisKey = GetComponent<Button>();
        thisKey.onClick.AddListener(TypeKey);
        character = keyLabel.text;
        shiftCharacter = keyLabel.text.ToUpper();

        string numbers = "1234567890";
        if (numbers.Contains(keyLabel.text))
        {
            shiftCharacter = GetShiftCharacter();
        }

        string symbols = ",.";
        if (symbols.Contains(keyLabel.text))
        {
            shiftCharacter = GetShiftCharacter();
        }
    }

    private string GetShiftCharacter()
    {
        switch (keyLabel.text)
        {
            case "1":
                return "!";
            case "2":
                return "@";
            case "3":
                return "#";
            case "4":
                return "$";
            case "5":
                return "%";
            case "6":
                return "^";
            case "7":
                return "&";
            case "8":
                return "*";
            case "9":
                return "(";
            case "0":
                return ")";
            case ".":
                return ">";
            case ",":
                return "<";
            default:
                break;
        }
        return string.Empty;
    }

    private void HandleShift()
    {
        isShifted = !isShifted;

        if (isShifted == true)
        {
            keyLabel.text = shiftCharacter;
        }
        else
        {
            keyLabel.text = character;
        }
    }

    private void TypeKey()
    {
        if (isShifted == true)
        {
            KeyboardManager.instance.inputField.text += shiftCharacter;
        }
        else
        {
            KeyboardManager.instance.inputField.text += character;
        }
    }


}