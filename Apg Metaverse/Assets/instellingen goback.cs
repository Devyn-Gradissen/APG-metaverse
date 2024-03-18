using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoToMainMenu : MonoBehaviour
{
    // This function will be called when the button is clicked
    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
