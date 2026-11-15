using System;
using UnityEngine;

public static class KineticEvents
{
    // Passes current energy and max energy (0.0 to 1.0 normalized)
    public static event Action<float> OnEnergyChanged;

    // Passes state: Idle, Hovering, Holding, OutOfEnergy
    public static event Action<KineticState> OnStateChanged;

    // Passes world position & clip for 3D audio/VFX triggers
    public static event Action<Vector3, AudioClip> OnPlayActionSFX;
    public static event Action<Vector3> OnObjectLaunched;

    public static void RaiseEnergyChanged(float normalizedEnergy) => OnEnergyChanged?.Invoke(normalizedEnergy);
    public static void RaiseStateChanged(KineticState newState) => OnStateChanged?.Invoke(newState);
    public static void RaisePlayActionSFX(Vector3 pos, AudioClip clip) => OnPlayActionSFX?.Invoke(pos, clip);
    public static void RaiseObjectLaunched(Vector3 pos) => OnObjectLaunched?.Invoke(pos);
}

public enum KineticState
{
    Idle,
    TargetInRange,
    HoldingObject,
    Depleted
}