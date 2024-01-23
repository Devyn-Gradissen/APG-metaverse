using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class KeyboardInputSort : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        KeyboardManager.instance.inputField = GetComponent<TMP_InputField>();
    }
}