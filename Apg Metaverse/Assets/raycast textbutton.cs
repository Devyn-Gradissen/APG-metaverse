using UnityEngine;

public class LayerBasedRaycasting : MonoBehaviour
{
    public LayerMask interactiveObjectsLayer; // Layer mask for interactive objects
    public LayerMask textObjectsLayer; // Layer mask for text objects
    public float maxDistance = 10f; // Maximum raycast distance

    void Update()
    {
        // Define raycast parameters
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        // Perform raycast for interactive objects layer
        RaycastHit interactiveHit;
        if (Physics.Raycast(ray, out interactiveHit, maxDistance, interactiveObjectsLayer))
        {
            // Handle raycast hit for interactive objects
            Debug.Log("Raycast hit on InteractiveObjects layer: " + interactiveHit.collider.gameObject.name);
            // Perform interaction logic for interactive objects
        }

        // Perform raycast for text objects layer
        RaycastHit textHit;
        if (Physics.Raycast(ray, out textHit, maxDistance, textObjectsLayer))
        {
            // Handle raycast hit for text objects
            Debug.Log("Raycast hit on TextObjects layer: " + textHit.collider.gameObject.name);
            // Perform interaction logic for text objects
        }
    }
}
