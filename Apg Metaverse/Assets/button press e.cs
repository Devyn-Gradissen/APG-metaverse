using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject interactionText; // UI text element to display interaction prompt
    public GameObject floatingText; // Reference to the floating text GameObject
    public float interactDistance = 3f; // Max distance to interact with the object

    void Start()
    {
        // Set interaction text to inactive when the scene starts
        interactionText.SetActive(false);

        // Set floating text to inactive when the scene starts
        floatingText.SetActive(false);
    }

    void Update()
    {
        // Create a ray from the center of the screen
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        RaycastHit hit;

        // Check if the ray hits any objects within the interact distance
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.gameObject == gameObject)
            {
                // Show the interaction text
                interactionText.SetActive(true);

                // Show the floating text
                floatingText.SetActive(true);

                // Check if the 'E' key is pressed
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Load the specified scene ("SampleScene" in this case)
                    SceneManager.LoadScene("mainmenu");
                }
            }
            else
            {
                // Hide the interaction text if looking away from the object
                interactionText.SetActive(false);

                // Hide the floating text if looking away from the object
                floatingText.SetActive(false);
            }
        }
        else
        {
            // Hide the interaction text if not within interact distance
            interactionText.SetActive(false);

            // Hide the floating text if not within interact distance
            floatingText.SetActive(false);
        }
    }
}
