/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Central manager for Enemy AI coordination.
/// Maintains a registry of all active enemies and executes their logic loop.
/// This allows for easy pausing and global AI optimization.
/// </summary>
public class EnemyAIManager : SingletonManager<EnemyAIManager>
{
    [Header("Runtime Data")]
    private List<EnemyBase> activeEnemies = new List<EnemyBase>();

    private Transform playerTransform;
    private GameManager gameManager;

    /// <summary>
    /// Caches local and global dependencies initializing management lifecycle.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        gameManager = GameManager.Instance;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    private void Update()
    {
        if (gameManager != null && gameManager.GetCurrentState() != GameState.Playing)
        {
            return;
        }

        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] != null)
            {
                activeEnemies[i].Tick();
            }
            else
            {
                activeEnemies.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Register an enemy base allowing Manager processing cycle integration.
    /// </summary>
    public void RegisterEnemy(EnemyBase enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
    }

    /// <summary>
    /// Remove specific matched enemy reference avoiding ghost processing memory leaks.
    /// </summary>
    public void UnregisterEnemy(EnemyBase enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    /// <summary>
    /// Acquire previously cached target player transforms bypassing redundant physics checks.
    /// </summary>
    public Transform GetPlayerTransform() => playerTransform;
}