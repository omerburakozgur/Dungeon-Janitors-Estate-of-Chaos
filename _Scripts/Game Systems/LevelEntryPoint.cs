/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Evaluates and handles level entry positioning for player instances using pre-placed
/// logical spline minecarts or default fallback flat landing coordinates accurately.
/// </summary>
public class LevelEntryPoint : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Sahnede onceden yerlestirilmis gelis vagonlari (MinecartArrival scriptleri)")]
    [SerializeField] private List<MinecartArrival> prePlacedArrivalCarts;
    [SerializeField] private bool autoSpawnOnStart = true;

    [Header("Fallback Settings")]
    [Tooltip("Eger vagon yoksa oyuncunun dogacagi duz noktalar")]
    [SerializeField] private List<Transform> fallbackSpawnPoints;

    private void Start()
    {
        if (autoSpawnOnStart) SpawnPlayer();
    }

    /// <summary>
    /// Executes player placement prioritizing animated minecart entry parameters automatically.
    /// Safely falls back to transform points if cinematic tools aren't positioned correctly.
    /// </summary>
    public void SpawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        if (prePlacedArrivalCarts != null && prePlacedArrivalCarts.Count > 0)
        {
            int playerCartIndex = Random.Range(0, prePlacedArrivalCarts.Count);

            for (int i = 0; i < prePlacedArrivalCarts.Count; i++)
            {
                MinecartArrival arrivalScript = prePlacedArrivalCarts[i];
                if (arrivalScript != null)
                {
                    bool isPlayerCart = (i == playerCartIndex);
                    arrivalScript.StartArrivalSequence(isPlayerCart ? player : null);
                }
            }
            return;
        }

        if (fallbackSpawnPoints != null && fallbackSpawnPoints.Count > 0)
        {
            Transform fallbackPoint = fallbackSpawnPoints[Random.Range(0, fallbackSpawnPoints.Count)];
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            player.transform.position = fallbackPoint.position;
            player.transform.rotation = fallbackPoint.rotation;
            if (cc != null) cc.enabled = true;
        }
    }
}