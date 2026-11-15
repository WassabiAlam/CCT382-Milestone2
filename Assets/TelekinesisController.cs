using UnityEngine;

public class TelekinesisController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private KineticConfigSO config;
    [SerializeField] private Transform holdAnchor;
    [SerializeField] private LineRenderer tetherLine;

    private Camera mainCam;
    private Rigidbody heldBody;
    private KineticInteractable currentTarget;
    private KineticState currentState = KineticState.Idle;

    private float currentEnergy;
    private float regenTimer;

    private void Awake()
    {
        mainCam = Camera.main;
        currentEnergy = config.maxEnergy;
        if (tetherLine != null) tetherLine.enabled = false;
    }

    private void Update()
    {
        HandleEnergy();

        if (heldBody == null)
        {
            ScanForTarget();
            if (Input.GetMouseButtonDown(1) && currentTarget != null && currentEnergy > 10f)
            {
                GrabObject(currentTarget);
            }
        }
        else
        {
            UpdateTetherVisuals();

            // Left click launches the held object
            if (Input.GetMouseButtonDown(0))
            {
                LaunchObject();
            }
            // Release right click to drop
            else if (Input.GetMouseButtonUp(1) || currentEnergy <= 0f)
            {
                ReleaseObject();
            }
        }
    }

    private void FixedUpdate()
    {
        if (heldBody != null)
        {
            Vector3 targetPos = transform.position + (mainCam.transform.forward * config.holdDistance) + (Vector3.up * config.holdHeightOffset);
            Vector3 direction = targetPos - heldBody.position;
            heldBody.linearVelocity = direction * config.pullSmoothSpeed; // Use .velocity in Unity 2022 or older
            heldBody.transform.Rotate(Vector3.one * (config.objectSpinSpeed * Time.fixedDeltaTime));
        }
    }

    private void ScanForTarget()
    {
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, config.maxGrabRange, config.grabbableLayer))
        {
            if (hit.collider.TryGetComponent(out KineticInteractable interactable))
            {
                if (currentTarget != interactable)
                {
                    ClearHover();
                    currentTarget = interactable;
                    currentTarget.SetHighlighted(true);
                    SetState(KineticState.TargetInRange);
                }
                return;
            }
        }

        ClearHover();
    }

    private void ClearHover()
    {
        if (currentTarget != null)
        {
            currentTarget.SetHighlighted(false);
            currentTarget = null;
        }
        if (currentState == KineticState.TargetInRange)
        {
            SetState(KineticState.Idle);
        }
    }

    private void GrabObject(KineticInteractable target)
    {
        heldBody = target.GetComponent<Rigidbody>();
        heldBody.useGravity = false;
        heldBody.linearDamping = 4f; // Use .drag in Unity 2022 or older

        if (tetherLine != null) tetherLine.enabled = true;
        SetState(KineticState.HoldingObject);
        KineticEvents.RaisePlayActionSFX(heldBody.position, config.grabSFX);
    }

    private void LaunchObject()
    {
        if (currentEnergy < config.launchEnergyCost)
        {
            KineticEvents.RaisePlayActionSFX(transform.position, config.outOfEnergySFX);
            return;
        }

        Rigidbody bodyToLaunch = heldBody;
        Vector3 launchPos = bodyToLaunch.position;
        ReleaseObject();

        currentEnergy = Mathf.Max(0f, currentEnergy - config.launchEnergyCost);
        regenTimer = config.regenDelay;
        KineticEvents.RaiseEnergyChanged(currentEnergy / config.maxEnergy);

        bodyToLaunch.AddForce(mainCam.transform.forward * config.launchForce, ForceMode.Impulse);
        KineticEvents.RaiseObjectLaunched(launchPos);
        KineticEvents.RaisePlayActionSFX(launchPos, config.launchSFX);
    }

    private void ReleaseObject()
    {
        if (heldBody != null)
        {
            heldBody.useGravity = true;
            heldBody.linearDamping = 0.05f;
            heldBody = null;
        }

        if (tetherLine != null) tetherLine.enabled = false;
        ClearHover();
        SetState(KineticState.Idle);
    }

    private void HandleEnergy()
    {
        if (heldBody != null)
        {
            currentEnergy = Mathf.Max(0f, currentEnergy - config.drainPerSecond * Time.deltaTime);
            regenTimer = config.regenDelay;
            KineticEvents.RaiseEnergyChanged(currentEnergy / config.maxEnergy);
        }
        else if (currentEnergy < config.maxEnergy)
        {
            if (regenTimer > 0f)
            {
                regenTimer -= Time.deltaTime;
            }
            else
            {
                currentEnergy = Mathf.Min(config.maxEnergy, currentEnergy + config.regenRate * Time.deltaTime);
                KineticEvents.RaiseEnergyChanged(currentEnergy / config.maxEnergy);
            }
        }
    }

    private void UpdateTetherVisuals()
    {
        if (tetherLine != null && holdAnchor != null && heldBody != null)
        {
            tetherLine.SetPosition(0, holdAnchor.position);
            tetherLine.SetPosition(1, heldBody.position);
        }
    }

    private void SetState(KineticState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            KineticEvents.RaiseStateChanged(currentState);
        }
    }
}