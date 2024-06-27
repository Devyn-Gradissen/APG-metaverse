using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviourPun
{
    public Animator animator;
    public float moveSpeed = 5f;
    public float runSpeed = 10f; // Speed when running
    public float mouseSensitivity = 2f; // Sensitivity of mouse movement
    public Camera playerCamera; // Assign the camera in the Unity editor

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private float verticalLookRotation = 0f;
    private float horizontalRotation = 0f;

    private GameManager gameManager; // Reference to the GameManager script
    private bool isMovementEnabled = true; // Flag to track if movement is enabled

    void Start()
    {
        if (!photonView.IsMine)
        {
            // Disable the script for non-local players
            this.enabled = false;
            return;
        }

        animator = gameObject.GetComponent<Animator>();

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
        if (photonView.IsMine && enabled && isMovementEnabled) // Check if movement and script are enabled
        {
            if (!gameManager.chatBox.isFocused) // Check if the chat input field is not focused
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

                // Movement input
                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");

                // Check if the Shift key is held down for running
                bool isRunning = Input.GetKey(KeyCode.LeftShift) && verticalInput > 0;
                float currentSpeed = isRunning ? runSpeed : moveSpeed;

                // Set the animator parameters based on the movement state
                bool isMoving = horizontalInput != 0 || verticalInput != 0;
                animator.SetBool("IsMoving", isMoving);
                animator.SetBool("IsRunning", isRunning);

                // Calculate movement direction based on camera orientation
                Vector3 movement = (playerCamera.transform.forward * verticalInput + playerCamera.transform.right * horizontalInput).normalized;
                movement.y = 0f; // Ensure no vertical movement
                movement *= currentSpeed * Time.deltaTime;

                // Move the player using physics
                if (movement != Vector3.zero)
                {
                    rb.MovePosition(transform.position + movement);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine && enabled && isMovementEnabled) // Check if movement and script are enabled
        {
            // Here you can handle any physics-based movement adjustments if needed, but the main input handling has been moved to Update
            if (!gameManager.chatBox.isFocused) // Check if the chat input field is not focused
            {
                // Detect collisions and adjust movement direction if necessary
                // (This can be moved to the FixedUpdate if you prefer to handle physics here)
            }
        }
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

    public void ToggleMovement(bool isEnabled)
    {
        isMovementEnabled = isEnabled;
    }

    public void SetGameManager(GameManager manager)
    {
        gameManager = manager;
    }
}
