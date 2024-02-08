using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f; // Sensitivity of mouse movement
    public Camera playerCamera; // Assign the camera in the Unity editor
    
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private float verticalLookRotation = 0f;
    private float horizontalRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        // Create a physics material with increased friction
        PhysicMaterial material = new PhysicMaterial();
        material.dynamicFriction = 2.0f; // Increase friction further
        material.staticFriction = 2.0f; // Increase friction further

        // Assign the physics material to the character's collider
        capsuleCollider.material = material;

        // Freeze rotation along X and Z axes
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Increase angular drag
        rb.angularDrag = 5f; // Example value, adjust as needed

        // Hide the mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Camera rotation input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the player horizontally
        horizontalRotation += mouseX;
        transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);

        // Rotate the camera vertically
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f); // Clamp vertical rotation to prevent flipping
        if (playerCamera != null) // Check if the camera is assigned
        {
            // Rotate the camera vertically only if there's mouse movement
            if (Mathf.Abs(mouseY) > 0.01f)
                playerCamera.transform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
        }
    }

    void FixedUpdate()
    {
        // Movement input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement direction based on camera orientation
        Vector3 movement = (playerCamera.transform.forward * verticalInput + playerCamera.transform.right * horizontalInput).normalized;
        movement.y = 0f; // Ensure no vertical movement
        movement *= moveSpeed * Time.fixedDeltaTime;

        // Detect collisions and adjust movement direction
        AdjustMovementDirection(ref movement);

        // Move the player using physics
        rb.MovePosition(transform.position + movement);
    }

    void AdjustMovementDirection(ref Vector3 movement)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, movement.normalized, out hit, capsuleCollider.radius + 0.1f))
        {
            // If the raycast hits a wall, adjust movement direction to prevent climbing
            movement = Vector3.ProjectOnPlane(movement, hit.normal);
        }
    }
}
