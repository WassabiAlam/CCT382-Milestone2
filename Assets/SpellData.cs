using UnityEngine;

[CreateAssetMenu(fileName = "NewSpellData", menuName = "Gameplay/Spell Data")]
public class SpellData : ScriptableObject
{
    [Header("Identity")]
    public string spellName = "Energy Blast";
    public Sprite spellIcon;

    [Header("Combat Stats")]
    public float damage = 40f;
    public float energyCost = 25f;
    public float cooldown = 1.2f;
    public float projectileSpeed = 25f;

    [Header("Audio Configuration")]
    public AudioClip castSound;
    public AudioClip impactSound;
    [Range(0.5f, 1.5f)] public float minPitch = 0.9f;
    [Range(0.5f, 1.5f)] public float maxPitch = 1.1f;
}