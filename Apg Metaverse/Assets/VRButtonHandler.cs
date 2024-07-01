using UnityEngine;
using UnityEngine.SceneManagement;

public class VRSceneButtonHandler : MonoBehaviour
{
    public float interactDistance = 3f; // Max distance to interact with the button

    void Update()
    {
        // Get the position and forward direction of the VR headset (camera)
        Vector3 headsetPosition = Camera.main.transform.position;
        Vector3 headsetForward = Camera.main.transform.forward;

        Ray ray = new Ray(headsetPosition, headsetForward);

        RaycastHit hit;

        // Check if the ray hits any objects within the interact distance
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // Check if the hit object's name is "VRButton" (you can also use tag comparison)
            if (hit.collider.gameObject.name == "VRButton")
            {
                // Check if the B button on the VR controller is pressed
                if (Input.GetKeyDown(KeyCode.JoystickButton1)) // B button is typically button 1
                {
                    // Load the specified scene
                    SceneManager.LoadScene(3);
                }
            }
        }
    }
}
