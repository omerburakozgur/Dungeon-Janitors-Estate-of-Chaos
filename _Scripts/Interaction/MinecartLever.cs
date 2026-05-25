/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the minecart departure lever, requiring a hold interaction
/// to initiate the departure sequence.
/// </summary>
public class MinecartLever : BaseInteractable
{
    [Header("Seat Requirement")]
    [Tooltip("The seat that must be occupied by the player to interact with this lever.")]
    [SerializeField] private MinecartSeat requiredSeat;

    [Header("Hold Settings")]
    [SerializeField] private float requiredHoldTime = 3f;
    [SerializeField] private string promptText = "[Basılı Tut] Motoru Çalıştır";
    [SerializeField] private bool disableAfterUse = true;

    [Header("Broadcasting On")]
    [Tooltip("Event channel triggered when the hold duration is successfully completed.")]
    [SerializeField] private VoidEventChannelSO onHoldCompleteEvent;

    [Tooltip("Event channel broadcasting the hold progress [0..1] to the UI.")]
    [SerializeField] private FloatEventChannelSO onHoldProgressEvent;

    private Coroutine holdRoutine;
    private bool isHolding = false;
    private bool isUsed = false;

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// Gets the interaction prompt if the lever is ready and the required seat is occupied.
    /// </summary>
    public override string GetInteractionPrompt()
    {
        if (isUsed) return "";
        if (requiredSeat != null && !requiredSeat.IsOccupied) return "";

        return promptText;
    }

    public override void RequestInteract() { }

    /// <summary>
    /// Begins the hold interaction coroutine if the conditions are met.
    /// </summary>
    public override void RequestHoldInteract()
    {
        if (requiredSeat != null && !requiredSeat.IsOccupied) return;

        if (!isHolding && !isUsed) holdRoutine = StartCoroutine(HoldRoutine());
    }

    /// <summary>
    /// Cancels the hold interaction and resets progress if released early.
    /// </summary>
    public override void RequestReleaseInteract()
    {
        if (isHolding && !isUsed)
        {
            isHolding = false;
            if (holdRoutine != null) StopCoroutine(holdRoutine);

            if (onHoldProgressEvent != null) onHoldProgressEvent.Raise(0f);
        }
    }

    private IEnumerator HoldRoutine()
    {
        isHolding = true;
        float timer = 0f;

        while (timer < requiredHoldTime)
        {
            timer += Time.deltaTime;
            if (onHoldProgressEvent != null) onHoldProgressEvent.Raise(timer / requiredHoldTime);
            yield return null;
        }

        isHolding = false;

        if (onHoldCompleteEvent != null) onHoldCompleteEvent.Raise();

        if (disableAfterUse) isUsed = true;
    }
}