using UnityEngine;
using UnityEngine.UI;

public class HeroSpellController : MonoBehaviour
{
    [Header("Data Architecture")]
    [SerializeField] private SpellData currentSpell;

    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ParticleSystem castVFX;

    [Header("HUD References")]
    [SerializeField] private Image cooldownRadialImage;
    [SerializeField] private Slider energySlider;

    private float currentCooldownTimer;
    private float maxEnergy = 100f;
    private float currentEnergy;

    public SpellData CurrentSpell => currentSpell;
    public float EnergyNormalized => currentEnergy / maxEnergy;
    public float CooldownNormalized => currentCooldownTimer > 0f ? currentCooldownTimer / currentSpell.cooldown : 0f;

    void Start()
    {
        currentEnergy = maxEnergy;
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        HandleCooldowns();
        HandleEnergyRegen();
        HandleInput();
        UpdateHUD();
    }

    void HandleInput()
    {
        // Check for left-click using the new Input System
        if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Left click registered by controller!");
            TryCastSpell();
        }
    }

    void TryCastSpell()
    {
        if (currentSpell == null)
        {
            Debug.LogError("FAIL: Current Spell ScriptableObject slot is empty on the Player inspector!");
            return;
        }

        if (currentCooldownTimer > 0f)
        {
            Debug.Log("FAIL: Spell is still on cooldown.");
            return;
        }

        if (currentEnergy < currentSpell.energyCost)
        {
            Debug.Log($"FAIL: Not enough energy! Current: {currentEnergy}, Required: {currentSpell.energyCost}");
            return;
        }

        ExecuteSpellCast();
    }

    void ExecuteSpellCast()
    {
        Debug.Log("SUCCESS: Executing spell cast and spawning projectile!");

        currentEnergy -= currentSpell.energyCost;
        currentCooldownTimer = currentSpell.cooldown;

        if (castVFX != null)
        {
            castVFX.Play();
        }

        if (currentSpell.castSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(currentSpell.minPitch, currentSpell.maxPitch);
            audioSource.PlayOneShot(currentSpell.castSound);
        }

        if (projectilePrefab == null)
        {
            Debug.LogError("FAIL: Projectile Prefab slot is empty on the HeroSpellController inspector!");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError("FAIL: Fire Point transform slot is empty on the HeroSpellController inspector!");
            return;
        }

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile pComponent = proj.GetComponent<Projectile>();
        if (pComponent != null)
        {
            pComponent.Initialize(currentSpell);
        }
        else
        {
            Debug.LogError("FAIL: The spawned prefab is missing the 'Projectile.cs' script!");
        }
    }

    void HandleCooldowns()
    {
        if (currentSpell != null && currentCooldownTimer > 0f)
        {
            currentCooldownTimer -= Time.deltaTime;
        }
    }

    void HandleEnergyRegen()
    {
        if (currentEnergy < maxEnergy && currentCooldownTimer <= 0f)
        {
            currentEnergy += Time.deltaTime * 20f;
            currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
        }
    }

    void UpdateHUD()
    {
        if (cooldownRadialImage != null && currentSpell != null)
        {
            cooldownRadialImage.fillAmount = CooldownNormalized;
        }

        if (energySlider != null)
        {
            energySlider.value = EnergyNormalized;
        }
    }
}