using UnityEngine;

[CreateAssetMenu(fileName = "NewKineticConfig", menuName = "Portfolio/Kinetic Mechanic Config")]
public class KineticConfigSO : ScriptableObject
{
    [Header("Targeting")]
    public float maxGrabRange = 18f;
    public LayerMask grabbableLayer;

    [Header("Physics & Movement")]
    public float holdDistance = 3.5f;
    public float holdHeightOffset = 1.4f;
    public float pullSmoothSpeed = 12f;
    public float launchForce = 35f;
    public float objectSpinSpeed = 45f;

    [Header("Energy & Cooldowns")]
    public float maxEnergy = 100f;
    public float drainPerSecond = 20f;
    public float launchEnergyCost = 25f;
    public float regenRate = 15f;
    public float regenDelay = 1.2f;

    [Header("Audio")]
    public AudioClip grabSFX;
    public AudioClip launchSFX;
    public AudioClip outOfEnergySFX;
    [Range(0f, 0.3f)] public float pitchVariance = 0.12f;
}