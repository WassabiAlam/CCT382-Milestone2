using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerMovementInput))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float sprintSpeed = 8.0f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed = 15f; 

    [Header("Jumping Settings")]
    [SerializeField] private float jumpHeight = 2.0f;

    [Header("Roll Settings")]
    [SerializeField] private float rollSpeed = 9.0f;
    [SerializeField] private float rollDuration = 0.6f; 
    private bool isRolling = false;
    private float rollTimer = 0f;
    private Vector3 rollDirection;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;
    private PlayerMovementInput inputScript;
    private Animator animator;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputScript = GetComponent<PlayerMovementInput>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        Vector2 input = inputScript.MoveInput;

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraRight * input.x + cameraForward * input.y);

        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        // --- ROLL LOGIC ---
        if (isRolling)
        {
            rollTimer -= Time.deltaTime;
            
            Vector3 rollVelocity = (rollDirection * rollSpeed) + (Vector3.up * verticalVelocity);
            controller.Move(rollVelocity * Time.deltaTime);

            if (rollTimer <= 0f)
            {
                isRolling = false;
                animator.SetBool("IsRolling", false);
            }
            return; 
        }

        if (controller.isGrounded && inputScript.RollTriggered)
        {
            isRolling = true;
            rollTimer = rollDuration;
            rollDirection = moveDirection.sqrMagnitude > 0.01f ? moveDirection.normalized : transform.forward;

            transform.rotation = Quaternion.LookRotation(rollDirection);
            animator.SetBool("IsRolling", true);
            return;
        }
        // ------------------

        if (controller.isGrounded && inputScript.JumpTriggered)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;

        float currentSpeed = inputScript.SprintHeld ? sprintSpeed : moveSpeed;
        Vector3 finalVelocity = (moveDirection * currentSpeed) + (Vector3.up * verticalVelocity);
        controller.Move(finalVelocity * Time.deltaTime);

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        animator.SetBool("IsGrounded", controller.isGrounded);
    }
}