//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class SettingsVisualUpdater : MonoBehaviour
//{
    //public Slider HelderheidSlider;
    //public Slider VolumeSlider;
    //public Toggle PushToTalkToggle;
    //public TextMeshProUGUI KleurenblindTextOutput;

    //private void Start()
    //{
    //    LoadUserSettings(DBmanager.username);
    //}

    //private void LoadUserSettings(string username)
    //{
    //    UserSettings userSettings = GetUserSettings(username);
    //
    //   if (userSettings != null)
    //    {
    //        HelderheidSlider.value = userSettings.HelderheidValue;
    //        VolumeSlider.value = userSettings.VolumeValue;
    //        PushToTalkToggle.isOn = userSettings.PushToTalkValue;
    //        UpdateKleurenblindDropdown(userSettings.KleurenblindValue);
    //    }
    //    else
    //    {
    //        HelderheidSlider.value = 0.5f;
    //        VolumeSlider.value = 0.5f; 
    //        PushToTalkToggle.isOn = true; 
    //        UpdateKleurenblindDropdown(0);
    //    }
    //}

    //private void UpdateKleurenblindDropdown(string kleurenblindText)
    //{
    //    switch (kleurenblindText)
    //    {
    //        case "UIT":
    //            break;
    //        case "Trichromatopsie":
    //            break;
    //        case "Dichromatopsie":
    //            break;
    //        case "Monochromatopsie":
    //            break;
    //        case "Achromatopsie":
    //            break;
    //        default:
    //            break;
    //    }
    //}

    //private UserSettings GetUserSettings(string username)
    //{
    //    return null;
    //}
//}

//public class UserSettings
//{
//    public float HelderheidValue;
//    public float VolumeValue;
//    public bool PushToTalkValue;
//    public string KleurenblindValue;
//}