using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PushtotalkController : MonoBehaviour
{
    public Toggle PushToTalk;

    public float PushToTalkValue;

    public void Start()
    {
        PushToTalkValue = PlayerPrefs.GetFloat("PushToTalk", PushToTalkValue);
        PushToTalk.isOn = PushToTalkValue > 0.5f;
        PushToTalk.onValueChanged.AddListener(delegate { ToggleValueChanged(PushToTalk.isOn); });
    }

    void ToggleValueChanged(bool isOn)
    {
        float value = isOn ? 1.0f : 0.0f;
        ChangeToggle(value);
    }

    public void ChangeToggle(float value)
    {
        PushToTalkValue = value;
        PlayerPrefs.SetFloat("PushToTalk", PushToTalkValue);
    }
}