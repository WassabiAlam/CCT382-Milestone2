using UnityEngine;
using UnityEngine.InputSystem; // Required for the New Input System

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    private CharacterController controller;
    private Transform mainCameraTransform;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Gravity Settings")]
    [SerializeField] private float gravity = -9.81f;
    private float verticalVelocity;

    // Track input values from the New Input System
    private Vector2 moveInput;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    // This method will be called by the Player Input component
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        Vector3 moveDirection = Vector3.zero;

        // 1. Calculate movement direction using the New Input System values
        if (moveInput.magnitude >= 0.1f)
        {
            // Flatten camera directions onto the XZ plane
            Vector3 camForward = mainCameraTransform.forward;
            Vector3 camRight = mainCameraTransform.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            // Compute actual world direction based on camera view
            moveDirection = (camForward * moveInput.y) + (camRight * moveInput.x);

            // Rotate the player smoothly to face the direction they are moving
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 2. Apply Gravity safely
        if (controller.isGrounded)
        {
            verticalVelocity = -2f; // Stuck to the ground
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Combine horizontal movement and vertical gravity movement
        Vector3 finalMovement = (moveDirection * moveSpeed) + (Vector3.up * verticalVelocity);

        // 3. Move the character
        controller.Move(finalMovement * Time.deltaTime);
    }
}
