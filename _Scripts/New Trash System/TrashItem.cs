/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Represents a collectable trash item in the world, referencing its configuration data.
/// </summary>
public class TrashItem : MonoBehaviour
{
    public TrashDataSO trashData;
    public GameObject floatingTextPrefab;

    /// <summary>
    /// Spawns a floating text prompt at the specified position upon collection.
    /// </summary>
    /// <param name="position">The world position to spawn the text.</param>
    public void SpawnFloatingText(Vector3 position)
    {
        if (floatingTextPrefab != null)
        {
            GameObject textObj = Instantiate(floatingTextPrefab, position, Quaternion.identity);
            if (textObj.TryGetComponent<FloatingText>(out var floatText))
            {
                floatText.Setup("Collected!", Color.green);
            }
        }
    }
}