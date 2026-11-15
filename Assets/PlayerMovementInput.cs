using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerMovementInput : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Drag PlayerControls.inputactions here.")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string jumpActionName = "Jump";
    [SerializeField] private string sprintActionName = "Sprint";
    [SerializeField] private string rollActionName = "Roll";

    [Header("Animator Parameters")]
    [SerializeField] private string moveXParam = "MoveX";
    [SerializeField] private string moveYParam = "MoveY";
    [SerializeField] private float sprintAnimationMultiplier = 2.0f;

    [Header("Smoothing")]
    [SerializeField] private float dampTime = 0.1f;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction rollAction;
    private Animator animator;

    public Vector2 MoveInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool RollTriggered { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (inputActions == null)
        {
            Debug.LogError($"{name}: No InputActionAsset assigned to PlayerMovementInput.");
            return;
        }

        var map = inputActions.FindActionMap(actionMapName, throwIfNotFound: false);
        if (map == null)
        {
            Debug.LogError($"{name}: Action map \"{actionMapName}\" not found.");
            return;
        }

        moveAction = map.FindAction(moveActionName, throwIfNotFound: false);
        jumpAction = map.FindAction(jumpActionName, throwIfNotFound: false);
        sprintAction = map.FindAction(sprintActionName, throwIfNotFound: false);
        rollAction = map.FindAction(rollActionName, throwIfNotFound: false);
    }

    private void OnEnable()
    {
        moveAction?.Enable();
        jumpAction?.Enable();
        sprintAction?.Enable();
        rollAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
        sprintAction?.Disable();
        rollAction?.Disable();
    }

    private void Update()
    {
        MoveInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        JumpTriggered = jumpAction != null && jumpAction.triggered;
        SprintHeld = sprintAction != null && sprintAction.IsPressed();
        RollTriggered = rollAction != null && rollAction.triggered;

        Vector2 animatorInput = MoveInput;
        if (SprintHeld)
        {
            animatorInput *= sprintAnimationMultiplier;
        }

        animator.SetFloat(moveXParam, animatorInput.x, dampTime, Time.deltaTime);
        animator.SetFloat(moveYParam, animatorInput.y, dampTime, Time.deltaTime);
    }
}