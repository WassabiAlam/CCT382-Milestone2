using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class KineticInteractable : MonoBehaviour
{
    [Header("Shader Graph Telegraphing")]
    [Tooltip("Exact Reference name of the Float property in your Shader Graph")]
    [SerializeField] private string highlightPropertyName = "_HighlightIntensity";
    [SerializeField] private float hoverGlowIntensity = 2.5f;
    [SerializeField] private float grabbedGlowIntensity = 5.0f;

    [Header("Physics & Impact Feel")]
    [Tooltip("Minimum collision speed required to trigger an impact sound/effect")]
    [SerializeField] private float minImpactVelocity = 4.0f;
    [SerializeField] private AudioClip heavyImpactSFX;
    [SerializeField] private ParticleSystem impactSparkPrefab;

    private Renderer objectRenderer;
    private Material instanceMaterial;
    private int highlightPropertyID;
    private bool isHeld;

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        highlightPropertyID = Shader.PropertyToID(highlightPropertyName);

        if (objectRenderer != null)
        {
            // Creates a safe per-object material instance so glowing one crate doesn't glow all crates
            instanceMaterial = objectRenderer.material;
            SetGlow(0f);
        }
    }

    /// <summary>
    /// Called by TelekinesisController when the player's reticle hovers over or leaves this object.
    /// </summary>
    public void SetHighlighted(bool isHighlighted)
    {
        if (isHeld) return; // Keep grabbed intensity if currently held
        SetGlow(isHighlighted ? hoverGlowIntensity : 0f);
    }

    /// <summary>
    /// Called when the object is grabbed or released to boost/reset the Shader Graph glow.
    /// </summary>
    public void SetGrabbedState(bool grabbed)
    {
        isHeld = grabbed;
        SetGlow(grabbed ? grabbedGlowIntensity : 0f);
    }

    private void SetGlow(float intensity)
    {
        if (instanceMaterial != null && instanceMaterial.HasProperty(highlightPropertyID))
        {
            instanceMaterial.SetFloat(highlightPropertyID, intensity);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Trigger audio/VFX when the object is thrown hard into a wall, floor, or hazard
        if (!isHeld && collision.relativeVelocity.magnitude >= minImpactVelocity)
        {
            Vector3 hitPoint = collision.contacts[0].point;

            if (heavyImpactSFX != null)
            {
                KineticEvents.RaisePlayActionSFX(hitPoint, heavyImpactSFX);
            }

            if (impactSparkPrefab != null)
            {
                Instantiate(impactSparkPrefab, hitPoint, Quaternion.LookRotation(collision.contacts[0].normal));
            }
        }
    }

    private void OnDestroy()
    {
        // Clean up the instanced material to prevent memory leaks
        if (instanceMaterial != null)
        {
            Destroy(instanceMaterial);
        }
    }
}