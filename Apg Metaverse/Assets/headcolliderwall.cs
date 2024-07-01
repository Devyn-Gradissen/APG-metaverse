using UnityEngine;

public class HeadCollisionHandler : MonoBehaviour
{
    private void OnCollisionStay(Collision collision)
    {
        // If collision occurs with a wall
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Determine the corrective action to prevent penetration
            // For example, move the player's head back along the collision normal
            Vector3 normal = collision.GetContact(0).normal;
            transform.position -= normal * 0.5f;
        }
    }
}
