/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Handles player interaction with a minecart seat, allowing them to board or unboard.
/// </summary>
public class MinecartInteractable : BaseInteractable
{
    [Header("Seat Reference")]
    public MinecartSeat mySeat;

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// Requests boarding or unboarding based on the seat's current occupancy and player state.
    /// </summary>
    public override void RequestInteract()
    {
        if (Player.Instance != null && !Player.Instance.GetComponent<CharacterController>().enabled)
        {
            if (mySeat != null && mySeat.CurrentPlayer != Player.Instance.gameObject) return;
        }

        if (mySeat != null && Player.Instance != null)
        {
            mySeat.ToggleSeat(Player.Instance.gameObject);
        }
    }

    public override void RequestHoldInteract() { }
    public override void RequestReleaseInteract() { }

    /// <summary>
    /// Updates the interaction prompt based on whether the seat is currently occupied.
    /// </summary>
    public override string GetInteractionPrompt()
    {
        return (mySeat != null && mySeat.IsOccupied) ? "[E] Ýn" : "[E] Bin";
    }
}