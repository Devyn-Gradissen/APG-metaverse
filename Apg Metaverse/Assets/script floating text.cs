using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public GameObject textObject; // Reference to the Text GameObject
    public float interactDistance = 5f; // Max distance to interact with the object
    public LayerMask interactLayer; // Layer mask to specify which objects can be interacted with

    void Update()
    {
        // Create a ray from the center of the screen
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        RaycastHit hit;

        // Check if the ray hits any objects within the interact layer
        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            if (hit.collider.gameObject == gameObject)
            {
                // Show the floating text
                textObject.SetActive(true);
            }
            else
            {
                // Hide the floating text if looking away from the object
                textObject.SetActive(false);
            }
        }
        else
        {
            // Hide the floating text if not within interact distance
            textObject.SetActive(false);
        }
    }
}
