/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Manages the seating logic for minecarts, handling player parentage,
/// control toggles, and safe unboarding functionality.
/// </summary>
public class MinecartSeat : MonoBehaviour
{
    [Header("Seat Settings")]
    public Transform seatPosition;
    public Transform exitPosition;

    [Tooltip("Set to true in the dungeon to hide items, false in the hub.")]
    public bool disableToolsOnSit = false;

    public GameObject CurrentPlayer { get; private set; }
    public bool IsOccupied => CurrentPlayer != null;

    /// <summary>
    /// Boards the specified player into the seat, locking their controls and adjusting their transform.
    /// </summary>
    public void BoardPlayer(GameObject player)
    {
        if (IsOccupied) return;

        if (UIManager.Instance != null)
            UIManager.Instance.ToggleHoldProgressBarUI(true);
        else if (HubUIManager.Instance != null)
            HubUIManager.Instance.ToggleHoldProgressBarUI(true);

        CurrentPlayer = player;

        if (Player.Instance != null)
        {
            Player.Instance.SetControlActiveExceptLook(false);
            if (disableToolsOnSit) Player.Instance.SetToolsActive(false);
        }

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        player.transform.SetParent(seatPosition);
        player.transform.localPosition = Vector3.zero;
        player.transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Unboards the current player from the seat, restoring their controls.
    /// </summary>
    public void UnboardPlayer(bool forceUnboard = false)
    {
        if (!IsOccupied) return;

        if (!forceUnboard)
        {
            UnityEngine.Splines.SplineAnimate splineAnim = GetComponentInParent<UnityEngine.Splines.SplineAnimate>();
            if (splineAnim != null && splineAnim.IsPlaying)
            {
                Debug.Log("Vagon hareket halindeyken inemezsin!");
                return;
            }
        }

        if (UIManager.Instance != null)
            UIManager.Instance.ToggleHoldProgressBarUI(false);
        else if (HubUIManager.Instance != null)
            HubUIManager.Instance.ToggleHoldProgressBarUI(false);

        CurrentPlayer.transform.SetParent(null);
        CurrentPlayer.transform.position = exitPosition.position;
        CurrentPlayer.transform.rotation = Quaternion.Euler(0f, exitPosition.rotation.eulerAngles.y, 0f);

        CharacterController cc = CurrentPlayer.GetComponent<CharacterController>();
        if (cc) cc.enabled = true;

        if (Player.Instance != null)
        {
            Player.Instance.SetControlActive(true);
            if (disableToolsOnSit) Player.Instance.SetToolsActive(true);
        }

        CurrentPlayer = null;
    }

    /// <summary>
    /// Toggles the boarding state of the specified player.
    /// </summary>
    public void ToggleSeat(GameObject player)
    {
        if (IsOccupied && CurrentPlayer == player)
        {
            UnboardPlayer();
        }
        else if (!IsOccupied)
        {
            BoardPlayer(player);
        }
    }
}