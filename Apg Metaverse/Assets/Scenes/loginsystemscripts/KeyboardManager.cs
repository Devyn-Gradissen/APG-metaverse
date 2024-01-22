using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyboardManager : MonoBehaviour
{
    public static KeyboardManager instance;
    public Button shiftButton;
    public Button SpaceButton;
    public Button DeleteButton;
    private Image shiftButtonImage;

    public TMP_InputField inputField;

    private bool isShifted = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        shiftButton.onClick.AddListener(Shifted);
        shiftButtonImage = shiftButton.gameObject.GetComponent<Image>();

        DeleteButton.onClick.AddListener(DeleteKey);
        SpaceButton.onClick.AddListener(Space);

    }

    private void Shifted()
    {
        isShifted = !isShifted;

        if (isShifted)
        {
            shiftButtonImage.color = Color.yellow;
        }
        else
        {
            shiftButtonImage.color = Color.white;
        }
    }

    private void Space()
    {
        inputField.text += " "; 
    }

    private void DeleteKey()
    {
        string currentText = instance.inputField.text;

        if (!string.IsNullOrEmpty(currentText))
        {
            instance.inputField.text = currentText.Substring(0, currentText.Length - 1);
        }
    }
}