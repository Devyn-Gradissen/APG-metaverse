using UnityEngine;
using UnityEngine.InputSystem;

public class VRplayerController : MonoBehaviour
{
    public Rigidbody rb;
    public float movespeed = 5f;

    public InputAction playerControls;

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    Vector2 moveDirection = Vector2.zero;

    void Update()
    {
        moveDirection = playerControls.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(moveDirection.x * movespeed, rb.velocity.y, moveDirection.y * movespeed);
    }
}
