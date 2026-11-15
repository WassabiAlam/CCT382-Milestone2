using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask targetLayerMask; // Select what layers can be hit (e.g., Enemies, Interactables)

    [Header("Input / Trigger")]
    [SerializeField] private InputActionAsset inputActions;
    private InputAction attackAction;

    void Awake()
    {
        // Optional: If you want to tie this to an input action like "Attack" or "Interact"
        if (inputActions != null)
        {
            var actionMap = inputActions.FindActionMap("Player", false);
            if (actionMap != null)
            {
                attackAction = actionMap.FindAction("Attack", false); // Make sure "Attack" or similar exists in your input map, or use mouse click
            }
        }
    }

    void OnEnable()
    {
        attackAction?.Enable();
    }

    void OnDisable()
    {
        attackAction?.Disable();
    }

    void Update()
    {
        // FIXED: Replaced legacy Input.GetMouseButtonDown(0) with the new Input System's Mouse check
        bool mouseClicked = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool attackPressed = (attackAction != null && attackAction.WasPressedThisFrame()) || mouseClicked;

        if (attackPressed)
        {
            PerformRaycastInteraction();
            // Alternatively, you can test Physics.OverlapSphere here for a hitbox:
            // PerformHitboxInteraction();
        }
    }

    private void PerformRaycastIdentifier() { }

    private void PerformRaycastInteraction()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;
        Vector3 rayDirection = transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, interactionRange, targetLayerMask))
        {
            Debug.Log($"[Interaction] Successfully hit target: {hit.collider.gameObject.name} on layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}", hit.collider.gameObject);

            // Trigger the visual hit flash on the target object
            HitFlashController flashController = hit.collider.GetComponent<HitFlashController>();
            if (flashController != null)
            {
                flashController.TriggerFlash();
            }
        }
        else
        {
            Debug.Log("[Interaction] Raycast fired, but hit nothing within range.");
        }
    }

    // --- VISUAL DEBUGGING GIZMOS ---
    private void OnDrawGizmos()
    {
        // 1. Draw the Raycast line in the Scene view
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayOrigin, transform.forward * interactionRange);

        // 2. (Optional) Draw the OverlapSphere hitbox preview if you use it
        // Vector3 centerPoint = transform.position + transform.forward * 1.5f;
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawWireSphere(centerPoint, 1.2f);
    }
}