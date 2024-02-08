using UnityEngine;
using UnityEngine.SceneManagement;

public class CreationMenu : MonoBehaviour {
    public void GoToScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}

