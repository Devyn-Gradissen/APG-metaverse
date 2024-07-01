using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PhotonAnimatorView))]
[RequireComponent(typeof(PhotonTransformView))]
public class PlayerController : MonoBehaviourPun
{
    public Animator animator; // De Animator component voor de speler
    public float moveSpeed = 3f; // Loopsnelheid
    public float runSpeed = 5f; // Ren snelheid
    public float mouseSensitivity = 2f; // Gevoeligheid van muisbeweging
    public Camera playerCamera; // De camera die de speler volgt

    private Rigidbody rb; // De Rigidbody component voor fysica
    private CapsuleCollider capsuleCollider; // De CapsuleCollider component
    private float verticalLookRotation = 0f; // Verticale rotatie voor de camera
    private float horizontalRotation = 0f; // Horizontale rotatie voor de speler

    private GameManager gameManager; // Referentie naar de GameManager script
    private bool isMovementEnabled = true; // Vlag om te controleren of beweging is ingeschakeld
    private Vector3 movementInput; // Bewegingsinput opgeslagen als Vector3

    void Start()
    {
        // Controleer of dit object door de lokale speler wordt bestuurd
        if (!photonView.IsMine)
        {
            this.enabled = false;
            return;
        }

        animator = GetComponent<Animator>(); // Haal de Animator component op

        rb = GetComponent<Rigidbody>(); // Haal de Rigidbody component op
        capsuleCollider = GetComponent<CapsuleCollider>(); // Haal de CapsuleCollider component op

        // Maak een fysisch materiaal met verhoogde wrijving
        PhysicMaterial material = new PhysicMaterial();
        material.dynamicFriction = 2.0f;
        material.staticFriction = 2.0f;
        capsuleCollider.material = material; // Wijs het materiaal toe aan de CapsuleCollider

        // Beperk rotatie langs X en Z assen
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.angularDrag = 5f; // Verhoogde angulaire weerstand
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Schakel interpolatie in voor soepele beweging

        Cursor.lockState = CursorLockMode.Locked; // Verberg de muiscursor en vergrendel deze in het midden van het scherm
        Cursor.visible = false;
    }

    void Update()
    {
        // Controleer of dit object door de lokale speler wordt bestuurd en beweging is ingeschakeld
        if (photonView.IsMine && enabled && isMovementEnabled)
        {
            // Controleer of de chatbox niet is gefocust
            if (!gameManager.chatBox.isFocused)
            {
                // Invoer voor camera rotatie
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

                // Roteer de speler horizontaal
                horizontalRotation += mouseX;
                transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);

                // Roteer de camera verticaal
                verticalLookRotation -= mouseY;
                verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f); // Beperk de verticale rotatie om omdraaien te voorkomen
                if (playerCamera != null && Mathf.Abs(mouseY) > 0.01f)
                {
                    playerCamera.transform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
                }

                // Invoer voor beweging
                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");

                // Controleer of de Shift-toets ingedrukt is voor rennen
                bool isRunning = Input.GetKey(KeyCode.LeftShift) && verticalInput > 0;
                float currentSpeed = isRunning ? runSpeed : moveSpeed;

                // Zet de animator parameters gebaseerd op de bewegingsstatus
                bool isMoving = horizontalInput != 0 || verticalInput != 0;
                animator.SetBool("IsMoving", isMoving);
                animator.SetBool("IsRunning", isRunning);

                // Bereken de bewegingsrichting gebaseerd op de camera oriëntatie
                movementInput = (playerCamera.transform.forward * verticalInput + playerCamera.transform.right * horizontalInput).normalized;
                movementInput.y = 0f; // Zorg ervoor dat er geen verticale beweging is
                movementInput *= currentSpeed; // Schaal de beweging met de huidige snelheid
            }
        }
    }

    void FixedUpdate()
    {
        // Controleer of dit object door de lokale speler wordt bestuurd en beweging is ingeschakeld
        if (photonView.IsMine && enabled && isMovementEnabled)
        {
            // Controleer of de chatbox niet is gefocust
            if (!gameManager.chatBox.isFocused)
            {
                // Beweeg de speler met behulp van fysica
                if (movementInput != Vector3.zero)
                {
                    rb.MovePosition(rb.position + movementInput * Time.fixedDeltaTime);
                }
            }
        }
    }

    public void ToggleMovement(bool isEnabled)
    {
        isMovementEnabled = isEnabled; // Schakel beweging in of uit
    }

    public void SetGameManager(GameManager manager)
    {
        gameManager = manager; // Stel de GameManager in
    }
}
