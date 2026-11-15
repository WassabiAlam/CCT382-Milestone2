using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem trailParticles;
    private SpellData spellData;
    private Rigidbody rb;

    public void Initialize(SpellData data)
    {
        spellData = data;
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * spellData.projectileSpeed;

        // Trigger particles on spawn
        if (trailParticles != null)
        {
            trailParticles.Play();
        }

        // Auto-destroy after 5 seconds to prevent memory leaks
        Destroy(gameObject, 5f);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Ignore collisions with the Player so it doesn't self-destruct on spawn
        if (collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        // Try to find any component that can take damage dynamically via reflection
        MonoBehaviour[] scripts = collision.gameObject.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            var method = script.GetType().GetMethod("TakeDamage");
            if (method != null && spellData != null)
            {
                method.Invoke(script, new object[] { spellData.damage });
                break;
            }
        }

        // Play impact audio at collision point with randomized pitch
        if (spellData != null && spellData.impactSound != null)
        {
            AudioSource.PlayClipAtPoint(spellData.impactSound, transform.position, 1f);
        }

        // Destroy projectile on impact with walls, enemies, or objects
        Destroy(gameObject);
    }
}