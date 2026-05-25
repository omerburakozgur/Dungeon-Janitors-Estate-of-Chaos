/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Singleton controller for the Player entity. Orchestrates sub-components and tool visibility.
/// </summary>
public class Player : SingletonManager<Player>
{
    [field: SerializeField] public PlayerMovement Movement { get; private set; }

    [Header("Equipment & Tools")]
    [Tooltip("Visual models, interaction colliders, or UI elements related to tools.")]
    [SerializeField] private GameObject[] toolObjects;

    protected override void Awake()
    {
        base.Awake();
        Movement = GetComponent<PlayerMovement>();
        if (Movement == null) Debug.LogError("[Player] Missing PlayerMovement component!");
    }

    /// <summary>
    /// Toggles the active state of all tool objects.
    /// </summary>
    /// <param name="isActive">True to show tools, false to hide.</param>
    public void SetToolsActive(bool isActive)
    {
        if (toolObjects == null || toolObjects.Length == 0) return;

        foreach (GameObject toolObj in toolObjects)
        {
            if (toolObj != null)
            {
                toolObj.SetActive(isActive);
            }
        }
    }

    /// <summary>
    /// Enables or disables player control components (Aiming and Movement).
    /// </summary>
    /// <param name="isActive">True to enable controls, false to lock them.</param>
    public void SetControlActive(bool isActive)
    {
        PlayerAim aim = GetComponent<PlayerAim>();
        if (aim != null) aim.enabled = isActive;

        if (Movement != null) Movement.enabled = isActive;
    }

    /// <summary>
    /// Locks movement but allows the player to keep looking around.
    /// </summary>
    /// <param name="isActive">True to enable movement, false to lock.</param>
    public void SetControlActiveExceptLook(bool isActive)
    {
        if (Movement != null) Movement.enabled = isActive;
    }
}