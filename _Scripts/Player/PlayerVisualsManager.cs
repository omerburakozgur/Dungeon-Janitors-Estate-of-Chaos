/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Orchestrates visual states (FPS/TPS) and synchronizes animations for the player entity.
/// </summary>
public class PlayerVisualsManager : MonoBehaviour
{
    [Header("Model Objects")]
    public GameObject tpsModel;
    public GameObject fpsLegs;
    public GameObject fpsArms;

    [Header("Renderers (For Shadows)")]
    [Tooltip("TPS model's Skinned Mesh Renderer (used to toggle shadow casting).")]
    public SkinnedMeshRenderer tpsRenderer;

    [Header("Animators")]
    public Animator tpsAnimator;
    public Animator fpsLegsAnimator;
    public Animator fpsArmsAnimator;

    [Header("Movement References")]
    public CharacterController characterController;
    public Transform playerCamera;

    [Header("Animator Parameters")]
    public string velocityXParam = "VelocityX";
    public string velocityZParam = "VelocityZ";
    public float animationSmoothTime = 0.1f;

    [Header("Footstep Scripts")]
    public PlayerFootsteps tpsFootsteps;
    public PlayerFootsteps fpsLegsFootsteps;

    private float currentVelocityX;
    private float currentVelocityZ;

    private void Start()
    {
        SetFPSView();
    }

    private void Update()
    {
        if (characterController != null)
        {
            Vector3 localVelocity = transform.InverseTransformDirection(characterController.velocity);
            currentVelocityX = Mathf.Lerp(currentVelocityX, localVelocity.x, animationSmoothTime);
            currentVelocityZ = Mathf.Lerp(currentVelocityZ, localVelocity.z, animationSmoothTime);
        }

        UpdateAnimators();
    }

    private void UpdateAnimators()
    {
        if (tpsAnimator != null && tpsAnimator.gameObject.activeInHierarchy)
        {
            tpsAnimator.SetFloat(velocityXParam, currentVelocityX);
            tpsAnimator.SetFloat(velocityZParam, currentVelocityZ);
        }

        if (fpsLegsAnimator != null && fpsLegsAnimator.gameObject.activeInHierarchy)
        {
            fpsLegsAnimator.SetFloat(velocityXParam, currentVelocityX);
            fpsLegsAnimator.SetFloat(velocityZParam, currentVelocityZ);
        }

        if (fpsArmsAnimator != null && fpsArmsAnimator.gameObject.activeInHierarchy)
        {
            fpsArmsAnimator.SetFloat(velocityXParam, currentVelocityX);
            fpsArmsAnimator.SetFloat(velocityZParam, currentVelocityZ);
        }
    }

    /// <summary>
    /// Switches the visuals to FPS mode.
    /// </summary>
    public void SetFPSView()
    {
        if (tpsRenderer != null) tpsRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        if (fpsLegs != null) fpsLegs.SetActive(true);
        if (fpsArms != null) fpsArms.SetActive(true);

        if (fpsLegsFootsteps != null) fpsLegsFootsteps.canPlayAudio = true;
        if (tpsFootsteps != null) tpsFootsteps.canPlayAudio = false;
    }

    /// <summary>
    /// Switches the visuals to TPS mode (e.g., for cutscenes or minecart sequences).
    /// </summary>
    public void SetTPSView()
    {
        if (tpsRenderer != null) tpsRenderer.shadowCastingMode = ShadowCastingMode.On;
        if (fpsLegs != null) fpsLegs.SetActive(false);
        if (fpsArms != null) fpsArms.SetActive(false);

        if (fpsLegsFootsteps != null) fpsLegsFootsteps.canPlayAudio = false;
        if (tpsFootsteps != null) tpsFootsteps.canPlayAudio = true;
    }
}