using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PhotonAnimatorView))]
[RequireComponent(typeof(PhotonTransformView))]
public class PlayerController : MonoBehaviourPun
{
    public Animator animator;
    public float moveSpeed = 3f;
    public float runSpeed = 5f;
    public float mouseSensitivity = 2f;
    public Camera playerCamera;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private float verticalLookRotation = 0f;
    private float horizontalRotation = 0f;

    private GameManager gameManager;
    private bool isMovementEnabled = true;
    private Vector3 movementInput;

    void Start()
    {
        if (!photonView.IsMine)
        {
            this.enabled = false;
            return;
        }

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        PhysicMaterial material = new PhysicMaterial();
        material.dynamicFriction = 2.0f;
        material.staticFriction = 2.0f;
        capsuleCollider.material = material;

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.angularDrag = 5f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (photonView.IsMine && enabled && isMovementEnabled)
        {
            if (!gameManager.chatBox.isFocused)
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

                horizontalRotation += mouseX;
                transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);

                verticalLookRotation -= mouseY;
                verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);
                if (playerCamera != null && Mathf.Abs(mouseY) > 0.01f)
                {
                    playerCamera.transform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
                }

                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");

                bool isRunning = Input.GetKey(KeyCode.LeftShift) && verticalInput > 0;
                float currentSpeed = isRunning ? runSpeed : moveSpeed;

                bool isMoving = horizontalInput != 0 || verticalInput != 0;
                animator.SetBool("IsMoving", isMoving);
                animator.SetBool("IsRunning", isRunning);

                movementInput = (playerCamera.transform.forward * verticalInput + playerCamera.transform.right * horizontalInput).normalized;
                movementInput.y = 0f;
                movementInput *= currentSpeed;
            }
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine && enabled && isMovementEnabled)
        {
            if (!gameManager.chatBox.isFocused)
            {
                if (movementInput != Vector3.zero)
                {
                    rb.MovePosition(rb.position + movementInput * Time.fixedDeltaTime);
                }
            }
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
