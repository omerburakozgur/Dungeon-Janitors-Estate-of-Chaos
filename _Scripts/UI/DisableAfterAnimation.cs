/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Simple utility component to disable a GameObject via an Animation Event.
/// </summary>
public class DisableAfterAnimation : MonoBehaviour
{
    /// <summary>
    /// Targeted by Animation Events to disable the parent GameObject upon completion.
    /// </summary>
    public void FinishAnimation()
    {
        gameObject.SetActive(false);
    }
}