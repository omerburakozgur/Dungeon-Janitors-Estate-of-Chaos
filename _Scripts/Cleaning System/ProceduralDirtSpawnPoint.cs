/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Used during Level Design to test the position, rotation, and scale of dirt.
/// Disables its own visual projection upon game start to avoid mixing with actual procedural dirt.
/// </summary>
[RequireComponent(typeof(DecalProjector))]
public class ProceduralDirtSpawnPoint : MonoBehaviour
{
    public DecalProjector PreviewProjector { get; private set; }

    private void Awake()
    {
        PreviewProjector = GetComponent<DecalProjector>();

        if (PreviewProjector != null)
        {
            PreviewProjector.enabled = false;
        }
    }
}